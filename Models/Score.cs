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
        public string Id { get; set; }

        /// <summary>
        /// 类别主键
        /// </summary>
        [Column("category_id")]
        public string CategoryId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(100)]
        [Column("title")]
        public string Title { get; set; }

        /// <summary>
        /// 作曲人
        /// </summary>
        [MaxLength(50)]
        [Column("composer")]
        public string? Composer { get; set; }

        /// <summary>
        /// 作词人
        /// </summary>
        [MaxLength(50)]
        [Column("lyricist")]
        public string? Lyricist { get; set; }

        /// <summary>
        /// 调号（如：C、D等）
        /// </summary>
        [MaxLength(1)]
        [Column("key_signature")]
        public string KeySignature { get; set; }

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
        /// 每行小节数量（如：6,5,4,3,2,1，默认为4）
        /// </summary>
        [Column("line_measure_count")]
        public int LineMeasureCount { get; set; }

        /// <summary>
        /// 页面方向（1-纵向，2-横向，默认为1）
        /// </summary>
        [Column("direction")]
        public int Direction { get; set; }

        /// <summary>
        /// 行集合
        /// </summary>
        [Ignore]
        public List<Line>? Lines { get; set; }
    }
}
