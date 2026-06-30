using SQLite;
using System.Collections.Generic;

namespace SEH.Models
{
    /// <summary>
    /// 小节类
    /// </summary>
    [Table("Measure")]
    public class Measure
    {
        /// <summary>
        /// 节主键
        /// </summary>
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; }

        /// <summary>
        /// 行主键
        /// </summary>
        [Column("line_id")]
        public string LineId { get; set; }

        /// <summary>
        /// 简谱主键
        /// </summary>
        [Column("score_id")]
        public string ScoreId { get; set; }

        /// <summary>
        /// 节号
        /// </summary>
        [Column("number")]
        public int Number { get; set; }

        /// <summary>
        /// 左小节线类型（1-小节线，2-反复起始线）
        /// </summary>
        [Column("left_line")]
        public int LeftLine { get; set; }

        /// <summary>
        /// 右小节线类型（1-小节线，2-虚小节线，3-段落线，4-反复终止线，5-终止线）
        /// </summary>
        [Column("right_line")]
        public int RightLine { get; set; }

        /// <summary>
        /// 小节宽度
        /// </summary>
        [Ignore]
        public double Width { get; set; }

        /// <summary>
        /// 音符集合
        /// </summary>
        [Ignore]
        public List<Note>? Notes { get; set; }

        /// <summary>
        /// 连尾组合集合
        /// </summary>
        [Ignore]
        public List<Beam>? Beams { get; set; }
    }
}
