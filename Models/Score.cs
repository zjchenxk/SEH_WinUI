using SQLite;
using System.Collections.Generic;

namespace SEH.Models
{
    /// <summary>
    /// 乐谱类
    /// </summary>
    [Table("Score")]
    public class Score
    {
        /// <summary>
        /// 乐谱主键
        /// </summary>
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; } = "";

        /// <summary>
        /// 类别主键
        /// </summary>
        [Column("category_id")]
        public string CategoryId { get; set; } = "";

        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(30)]
        [Column("title")]
        public string Title { get; set; } = "";

        /// <summary>
        /// 副标题
        /// </summary>
        [MaxLength(30)]
        [Column("sub_title")]
        public string Subtitle { get; set; } = "";

        /// <summary>
        /// 作曲人
        /// </summary>
        [MaxLength(20)]
        [Column("composer")]
        public string? Composer { get; set; }

        /// <summary>
        /// 作词人
        /// </summary>
        [MaxLength(20)]
        [Column("lyricist")]
        public string? Lyricist { get; set; }

        /// <summary>
        /// 调号（如：C、D等）
        /// </summary>
        [MaxLength(1)]
        [Column("key_signature")]
        public string KeySignature { get; set; } = "";

        /// <summary>
        /// 每小节拍数（拍号分子，如：2、3、4、5、6、7、9、12）
        /// </summary>
        [Column("measure_beat_count")]
        public int MeasureBeatCount { get; set; }

        /// <summary>
        /// 每拍时值（拍号分母，如：2、4、8）
        /// </summary>
        [Column("beat_duration")]
        public int BeatDuration { get; set; }

        /// <summary>
        /// 速度（如：80、120等）
        /// </summary>
        [Column("tempo")]
        public int Tempo { get; set; }

        /// <summary>
        /// 纸张尺寸（默认A4）
        /// </summary>
        [Column("paper_size")]
        public string PaperSize { get; set; } = "";

        /// <summary>
        /// 页面方向（1-纵向，2-横向，默认为1）
        /// </summary>
        [Column("direction")]
        public int Direction { get; set; }

        /// <summary>
        /// 页面左边距（单位：像素，默认为20）
        /// </summary>
        [Column("left_margin")]
        public int LeftMargin { get; set; }

        /// <summary>
        /// 页面上边距（单位：像素，默认为20）
        /// </summary>
        [Column("top_margin")]
        public int TopMargin { get; set; }

        /// <summary>
        /// 页面右边距（单位：像素，默认为20）
        /// </summary>
        [Column("right_margin")]
        public int RightMargin { get; set; }

        /// <summary>
        /// 页面下边距（单位：像素，默认为20）
        /// </summary>
        [Column("bottom_margin")]
        public int BottomMargin { get; set; }

        /// <summary>
        /// 行集合
        /// </summary>
        [Ignore]
        public List<Line>? Lines { get; set; }

        /// <summary>
        /// 连音线集合
        /// </summary>
        [Ignore]
        public List<Slur>? Slurs { get; set; }
    }
}
