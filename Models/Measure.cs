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
        /// 乐谱主键
        /// </summary>
        [Column("score_id")]
        public string ScoreId { get; set; }

        /// <summary>
        /// 节号
        /// </summary>
        [Column("number")]
        public int Number { get; set; }

        /// <summary>
        /// 终止线标记，1-是，0-否
        /// </summary>
        [Column("final_line")]
        public int FinalLine { get; set; }

        /// <summary>
        /// 虚小节线标记，1-是，0-否
        /// </summary>
        [Column("dashed_line")]
        public int DashedLine { get; set; }

        /// <summary>
        /// 段落线标记，1-是，0-否
        /// </summary>
        [Column("paragraph_line")]
        public int ParagraphLine { get; set; }

        /// <summary>
        /// 反复起始线标记，1-是，0-否
        /// </summary>
        [Column("repeat_start_line")]
        public int RepeatStartLine { get; set; }

        /// <summary>
        /// 反复终止线标记，1-是，0-否
        /// </summary>
        [Column("repeat_final_line")]
        public int RepeatFinalLine { get; set; }

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
