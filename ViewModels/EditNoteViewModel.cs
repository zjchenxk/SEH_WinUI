using CommunityToolkit.Mvvm.ComponentModel;
using SEH.Models;
using SEH.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SEH.ViewModels
{
    /// <summary>
    /// 编辑音符 ViewModel
    /// </summary>
    public partial class EditNoteViewModel : ObservableValidator
    {
        /// <summary>
        /// 音符ID
        /// </summary>
        [ObservableProperty]
        private string _id = Guid.NewGuid().ToString();

        /// <summary>
        /// 音符
        /// </summary>
        [Required(ErrorMessage = "音符不能为空！")]
        [ObservableProperty]
        private string _pitch = "1"; // 默认中音1

        /// <summary>
        /// 音符错误信息
        /// </summary>
        [ObservableProperty]
        private string? _pitchError = "";

        /// <summary>
        /// 时值
        /// </summary>
        [Required(ErrorMessage = "时值不能为空！")]
        [Range(0.125, 4, ErrorMessage = "时值必须在0.125到4之间")]
        [CustomValidation(typeof(EditNoteViewModel), nameof(ValidatedurationDuration))]
        [ObservableProperty]
        private string _duration = "1"; // 默认四分音符

        /// <summary>
        /// 时值错误信息
        /// </summary>
        [ObservableProperty]
        private string? _durationError = "";

        /// <summary>
        /// 附点数量
        /// </summary>
        [Required(ErrorMessage = "附点数量不能为空！")]
        [ObservableProperty]
        private string _dots = "0";

        /// <summary>
        /// 附点数量错误信息
        /// </summary>
        [ObservableProperty]
        private string? _dotsError = "";

        /// <summary>
        /// 延长号标记（1-有，0-无）
        /// </summary>
        [ObservableProperty]
        private string _fermata = "0";

        /// <summary>
        /// 延长号标记错误信息
        /// </summary>
        [ObservableProperty]
        private string? _fermataError = "";

        /// <summary>
        /// 连音线标记（1-开始，0-结束）
        /// </summary>
        [ObservableProperty]
        private string _slur = "";

        /// <summary>
        /// 连音线标记错误信息
        /// </summary>
        [ObservableProperty]
        private string? _slurError = "";

        /// <summary>
        /// 演奏方法
        /// </summary>
        [ObservableProperty]
        private string _articulation = "";

        /// <summary>
        /// 演奏方法错误信息
        /// </summary>
        [ObservableProperty]
        private string? _articulationError = "";

        /// <summary>
        /// 圆括号标记录（1-左括号，0-右括号）
        /// </summary>
        [ObservableProperty]
        private string _paren = "";

        /// <summary>
        /// 圆括号标记错误信息
        /// </summary>
        [ObservableProperty]
        private string? _parenError = "";

        /// <summary>
        /// 歌词
        /// </summary>
        [ObservableProperty]
        private string _lyrics = "";

        /// <summary>
        /// 歌词错误信息
        /// </summary>
        [ObservableProperty]
        private string? _lyricsError = "";

        /// <summary>
        /// 歌词2
        /// </summary>
        [ObservableProperty]
        private string _lyrics2 = "";

        /// <summary>
        /// 歌词2错误信息
        /// </summary>
        [ObservableProperty]
        private string? _lyrics2Error = "";

        /// <summary>
        /// 歌词3
        /// </summary>
        [ObservableProperty]
        private string _lyrics3 = "";

        /// <summary>
        /// 歌词3错误信息
        /// </summary>
        [ObservableProperty]
        private string? _lyrics3Error = "";

        /// <summary>
        /// 组合集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Beam> _beams = [];

        /// <summary>
        /// 选中组合
        /// </summary>
        [ObservableProperty]
        private Beam? _selectedBeam = null;

        /// <summary>
        /// 选中组合错误信息
        /// </summary>
        [ObservableProperty]
        private string? _selectedBeamError = "";

        /// <summary>
        /// 构造方法
        /// </summary>
        public EditNoteViewModel()
        {
            this.ErrorsChanged += EditNoteViewModel_ErrorsChanged;
        }

        private void EditNoteViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pitch))//当 Pitch 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Pitch));
                PitchError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(Duration))//当 Duration 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Duration));
                DurationError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(Dots))//当 Dots 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Dots));
                DotsError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(Fermata))//当 Fermata 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Fermata));
                FermataError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(Slur))//当 Slur 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Slur));
                SlurError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(Articulation))//当 Articulation 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Articulation));
                ArticulationError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(Paren))//当 Paren 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Paren));
                ParenError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(Lyrics))//当 Lyrics 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Lyrics));
                LyricsError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(Lyrics2))//当 Lyrics2 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Lyrics2));
                Lyrics2Error = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(Lyrics3))//当 Lyrics3 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Lyrics3));
                Lyrics3Error = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(SelectedBeam))//当 SelectedBeam 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(SelectedBeam));
                SelectedBeamError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="beams"></param>
        /// <param name="note"></param>
        public void Initialize(List<Beam>? beams = null, Note? note = null)
        {
            if (beams != null)
            {
                foreach (var beam in beams)
                {
                    Beams.Add(beam);
                }
            }

            if (note != null)
            {
                Id = note.Id;
                Pitch = note.Pitch;
                Duration = note.Duration.ToString();
                Dots = note.Dots.ToString();
                Fermata = note.Fermata.ToString();
                Slur = (note.Slur?.ToString()) ?? "";
                Articulation = note.Articulation ?? "";
                Paren = (note.Paren?.ToString()) ?? "";
                Lyrics = note.Lyrics ?? "";
                Lyrics2 = note.Lyrics2 ?? "";
                Lyrics3 = note.Lyrics3 ?? "";
                SelectedBeam = note.Beam;
            }
        }

        partial void OnPitchChanged(string value)
        {
            ValidateProperty(value, nameof(Pitch));
        }

        partial void OnDurationChanged(string value)
        {
            ValidateProperty(value, nameof(Duration));
        }

        partial void OnDotsChanged(string value)
        {
            ValidateProperty(value, nameof(Dots));
        }

        partial void OnFermataChanged(string value)
        {
            ValidateProperty(value, nameof(Fermata));
        }

        partial void OnSlurChanged(string value)
        {
            ValidateProperty(value, nameof(Slur));
        }

        partial void OnArticulationChanged(string value)
        {
            ValidateProperty(value, nameof(Articulation));
        }

        partial void OnParenChanged(string value)
        {
            ValidateProperty(value, nameof(Paren));
        }

        partial void OnLyricsChanged(string value)
        {
            ValidateProperty(value, nameof(Lyrics));
        }

        partial void OnLyrics2Changed(string value)
        {
            ValidateProperty(value, nameof(Lyrics2));
        }

        partial void OnLyrics3Changed(string value)
        {
            ValidateProperty(value, nameof(Lyrics3));
        }

        partial void OnSelectedBeamChanged(Beam? value)
        {
            ValidateProperty(value, nameof(SelectedBeam));
        }

        /// <summary>
        /// 自定义验证时值
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ValidationResult? ValidatedurationDuration(string duration, ValidationContext context)
        {
            //1.检查是否为空
            if (string.IsNullOrWhiteSpace(duration))
            {
                return new("时值不能为空！");
            }

            //2.如果当前音符加入减时组合，则时值必须是八分音符、十六分音符和三十二分音符
            var instance = (EditNoteViewModel)context.ObjectInstance;
            if (instance.SelectedBeam != null && (duration == "4" || duration == "2" || duration == "1"))
            {
                return new("时值不能为全音符、二分音符或四分音符！");
            }

            //3.验证通过
            return ValidationResult.Success;
        }

        /// <summary>
        /// 验证数据
        /// </summary>
        public bool ValidateProperties()
        {
            ValidateAllProperties();

            return !HasErrors;
        }

        public Note GetNote()
        {
            return new Note
            {
                Id = this.Id,
                Pitch = this.Pitch,
                Duration = float.Parse(this.Duration),
                Dots = int.Parse(this.Dots),
                Fermata = int.Parse(this.Fermata),
                Slur = string.IsNullOrWhiteSpace(this.Slur) ? null : int.Parse(this.Slur),
                Articulation = this.Articulation,
                Paren = string.IsNullOrWhiteSpace(this.Paren) ? null : int.Parse(this.Paren),
                Lyrics = this.Lyrics,
                Lyrics2 = this.Lyrics2,
                Lyrics3 = this.Lyrics3,
                BeamId = SelectedBeam?.Id,
                Beam = SelectedBeam,
            };
        }
    }
}
