using System.Threading.Tasks;

namespace SEH.Services.Interfaces
{
    public interface IAudioService
    {
        /// <summary>
        /// 初始化 MIDI 合成器
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// 播放指定音符
        /// </summary>
        /// <param name="pitch">简谱音高 (如 "1", "+2", "-3", "0")</param>
        /// <param name="durationMs">持续时间(毫秒)</param>
        /// <param name="velocity">力度 (0-127)</param>
        Task PlayNoteAsync(string pitch, double durationMs, int velocity = 100);

        /// <summary>
        /// 停止所有声音并释放资源
        /// </summary>
        void Dispose();
    }
}
