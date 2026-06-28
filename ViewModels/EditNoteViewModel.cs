using CommunityToolkit.Mvvm.ComponentModel;
using SEH.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace SEH.ViewModels
{
    public partial class EditNoteViewModel : ObservableValidator
    {
        /// <summary>
        /// 音高
        /// </summary>
        [ObservableProperty]
        private string _pitch = "1"; // 默认中音1

        /// <summary>
        /// 时值
        /// </summary>
        [ObservableProperty]
        [Range(0.125, 4, ErrorMessage = "时值必须在0.125到4之间")]
        private float _duration = 1; // 默认四分音符

        /// <summary>
        /// 附点数量
        /// </summary>
        [ObservableProperty]
        private int _dots = 0;

        /// <summary>
        /// 连音线标记，如：1-开始，0-表示结束
        /// </summary>
        [ObservableProperty]
        private int? _slur = null;

        /// <summary>
        /// 演奏方法
        /// </summary>
        [ObservableProperty]
        private string? _articulation = null;

        /// <summary>
        /// 延长号标记，如：1-有，0-无
        /// </summary>
        [ObservableProperty]
        private int _fermata = 0;

        /// <summary>
        /// 歌词
        /// </summary>
        [ObservableProperty]
        private string? _lyrics = null;

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
                Id = Guid.NewGuid().ToString(),
                Pitch = this.Pitch,
                Duration = this.Duration,
                Dots = this.Dots,
                Slur = this.Slur,
                Articulation = this.Articulation,
                Fermata = this.Fermata,
                Lyrics = this.Lyrics,
            };
        }
    }
}
