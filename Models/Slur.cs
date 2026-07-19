using SQLite;

namespace SEH.Models
{
    /// <summary>
    /// 连音线对象
    /// </summary>
    [Table("Slur")]
    public class Slur
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
        /// 连音线开始的行主键
        /// </summary>
        [Column("start_line_id")]
        public string StartLineId { get; set; } = "";

        /// <summary>
        /// 连音线开始的小节主键
        /// </summary>
        [Column("start_measure_id")]
        public string StartMeasureId { get; set; } = "";

        /// <summary>
        /// 连音线开始的音符主键
        /// </summary>
        [Column("start_note_id")]
        public string StartNoteId { get; set; } = "";

        /// <summary>
        /// 行号
        /// </summary>
        [Column("number")]
        public int Number { get; set; }

        /// <summary>
        /// 连音线名称
        /// </summary>
        [Column("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// 连音线结束的行主键
        /// </summary>
        [Column("end_line_id")]
        public string EndLineId { get; set; } = "";

        /// <summary>
        /// 连音线结束的小节主键
        /// </summary>
        [Column("end_measure_id")]
        public string EndMeasureId { get; set; } = "";

        /// <summary>
        /// 连音线结束的音符主键
        /// </summary>
        [Column("end_note_id")]
        public string EndNoteId { get; set; } = "";

        /// <summary>
        /// 嵌套层级，默认为0级
        /// </summary>
        [Ignore]
        public int Level { get; set; } = 0;
    }
}
