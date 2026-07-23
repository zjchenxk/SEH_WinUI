using SEH.Services.Interfaces;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Midi;

namespace SEH.Services
{
    public partial class MidiAudioService : IAudioService, IDisposable
    {
        private MidiSynthesizer? _synthesizer = null;
        private bool _isInitialized = false;

        //MIDI 通道 (0-15)，默认使用通道0
        private const byte _channel = 0;

        /// <summary>
        /// 初始化 MIDI 合成器
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            try
            {
                //使用 Windows 内置的 MIDI 合成器
                //这是最简单的方案，不需要外接硬件或复杂配置
                _synthesizer = await MidiSynthesizer.CreateAsync();

                if (_synthesizer != null)
                {
                    _isInitialized = true;

                    //可选：设置默认乐器为钢琴 (0)。MIDI Program Change
                    //0 = Acoustic Grand Piano
                    var programChange = new MidiProgramChangeMessage(_channel, 0);
                    _synthesizer.SendMessage(programChange);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"MIDI 初始化失败: {ex.Message}");

                _isInitialized = false;
            }
        }

        /// <summary>
        /// 播放音符
        /// </summary>
        /// <param name="pitch"></param>
        /// <param name="durationMs"></param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task PlayNoteAsync(string pitch, double durationMs, int velocity = 100)
        {
            if (!_isInitialized || _synthesizer == null)
            {
                await InitializeAsync();
                if (!_isInitialized) return;
            }

            //1.转换音高
            int noteNumber = ConvertPitchToMidiNote(pitch);

            //如果是休止符或无法识别，只等待时长，不发声
            if (noteNumber == -1)
            {
                await Task.Delay((int)durationMs);
                return;
            }

            //2.发送 NoteOn 消息
            var noteOnMessage = new MidiNoteOnMessage(_channel, (byte)noteNumber, (byte)velocity);
            _synthesizer?.SendMessage(noteOnMessage);

            //3.等待音符时值
            await Task.Delay((int)durationMs);

            //4.发送 NoteOff 消息
            var noteOffMessage = new MidiNoteOffMessage(_channel, (byte)noteNumber, (byte)velocity);
            _synthesizer?.SendMessage(noteOffMessage);
        }

        /// <summary>
        /// 将简谱音高字符串转换为 MIDI Note Number
        /// 基准：中央 C (1) = MIDI 60 (C4)
        /// </summary>
        private int ConvertPitchToMidiNote(string pitch)
        {
            if (string.IsNullOrWhiteSpace(pitch) || pitch == "0" || pitch == "X")
            {
                return -1; //休止符或噪音符，暂不处理
            }

            //解析八度偏移
            int octaveOffset = 0;
            string cleanPitch = pitch;

            if (pitch.StartsWith("+"))//高音：+1, ++1, +++1
            {
                octaveOffset = pitch.Count(c => c == '+');
                cleanPitch = pitch.Substring(octaveOffset);
            }
            else if (pitch.StartsWith("-"))//低音：-1, --1, ---1
            {
                //注意：简谱中 "-1" 是低音，MIDI中 "-1" 减八度
                //此时 pitch 可能是 "-1" (低音) 或 "--1" (倍低音)
                //但为了和简谱逻辑一致：
                //"-" 开头且后面跟数字 -> 低音
                //"--" 开头 -> 倍低音

                //简单解析逻辑：
                //如果 pitch 是 "-1"，它是低音 (offset -1)
                //如果 pitch 是 "--1"，它是倍低音 (offset -2)
                if (pitch.Length > 1 && char.IsDigit(pitch[pitch.Length - 1]))
                {
                    //计算前面有多少个 '-'
                    int dashCount = pitch.Count(c => c == '-');
                    octaveOffset = -dashCount;
                    cleanPitch = pitch.Substring(dashCount);
                }
            }

            //解析音名 (1-7)
            if (!int.TryParse(cleanPitch, out int scale)) return -1;

            //MIDI 音高计算
            //C4 (Middle C) = 60
            //简谱 "1" 默认为 C4
            //简谱音阶对应半音偏移量:
            //1(C) -> 0
            //2(D) -> 2
            //3(E) -> 4
            //4(F) -> 5
            //5(G) -> 7
            //6(A) -> 9
            //7(B) -> 11
            int[] semitoneOffsets = { 0, 2, 4, 5, 7, 9, 11 };

            if (scale < 1 || scale > 7) return -1;

            //基准 MIDI 60 (C4) + 音阶偏移 + 八度偏移
            int midiNote = 60 + semitoneOffsets[scale - 1] + (octaveOffset * 12);

            //校验 MIDI 范围 (0-127)
            if (midiNote < 0 || midiNote > 127) return -1;

            return midiNote;
        }

        public void Dispose()
        {
            if (_synthesizer != null)
            {
                _synthesizer.Dispose();
                _synthesizer = null;
                _isInitialized = false;
            }
        }
    }
}
