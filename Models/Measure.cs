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
