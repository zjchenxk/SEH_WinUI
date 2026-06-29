using CommunityToolkit.Mvvm.ComponentModel;
using SEH.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

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
        /// 时值
        /// </summary>
        [Required(ErrorMessage = "时值不能为空！")]
        [Range(0.125, 4, ErrorMessage = "时值必须在0.125到4之间")]
        [ObservableProperty]
        private string _duration = "1"; // 默认四分音符

        /// <summary>
        /// 附点数量
        /// </summary>
        [Required(ErrorMessage = "附点数量不能为空！")]
        [ObservableProperty]
        private string _dots = "0";

        /// <summary>
        /// 连音线标记，如：1-开始，0-表示结束
        /// </summary>
        [ObservableProperty]
        private string _slur = "";

        /// <summary>
        /// 演奏方法
        /// </summary>
        [ObservableProperty]
        private string _articulation = "";

        /// <summary>
        /// 延长号标记，如：1-有，0-无
        /// </summary>
        [ObservableProperty]
        private string _fermata = "0";

        /// <summary>
        /// 歌词
        /// </summary>
        [ObservableProperty]
        private string _lyrics = "";

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
        /// 验证数据
        /// </summary>
        public bool ValidateProperties()
        {
            ValidateAllProperties();

            return !HasErrors;
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
                Slur = note.Slur == null ? "" : note.Slur.ToString();
                Articulation = note.Articulation;
                Fermata = note.Fermata.ToString();
                Lyrics = note.Lyrics;
                SelectedBeam = note.Beam;
            }
        }

        public Note GetNote()
        {
            return new Note
            {
                Id = this.Id,
                Pitch = this.Pitch,
                Duration = float.Parse(this.Duration),
                Dots = int.Parse(this.Dots),
                Slur = string.IsNullOrWhiteSpace(this.Slur) ? null : int.Parse(this.Slur),
                Articulation = this.Articulation,
                Fermata = int.Parse(this.Fermata),
                Lyrics = this.Lyrics,
                BeamId = SelectedBeam?.Id,
                Beam = SelectedBeam,
            };
        }
    }
}
