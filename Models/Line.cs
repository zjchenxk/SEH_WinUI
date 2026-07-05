using SQLite;
using System.Collections.Generic;

namespace SEH.Models
{
    /// <summary>
    /// 行对象
    /// </summary>
    [Table("Line")]
    public class Line
    {
        /// <summary>
        /// 行主键
        /// </summary>
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; } = "";

        /// <summary>
        /// 乐谱主键
        /// </summary>
        [Column("score_id")]
        public string ScoreId { get; set; } = "";

        /// <summary>
        /// 行号
        /// </summary>
        [Column("number")]
        public int Number { get; set; }

        /// <summary>
        /// 音符占位宽度
        /// </summary>
        [Ignore]
        public double NoteWidth { get; set; }

        /// <summary>
        /// 小节集合
        /// </summary>
        [Ignore]
        public List<Measure>? Measures { get; set; }
    }
}
