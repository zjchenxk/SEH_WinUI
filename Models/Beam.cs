using SQLite;

namespace SEH.Models
{
    /// <summary>
    /// 减时组合类
    /// </summary>
    [Table("Beam")]
    public class Beam
    {
        /// <summary>
        /// 组合主键
        /// </summary>
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; } = "";

        /// <summary>
        /// 节主键
        /// </summary>
        [Column("measure_id")]
        public string MeasureId { get; set; } = "";

        /// <summary>
        /// 行主键
        /// </summary>
        [Column("line_id")]
        public string LineId { get; set; } = "";

        /// <summary>
        /// 乐谱主键
        /// </summary>
        [Column("score_id")]
        public string ScoreId { get; set; } = "";

        /// <summary>
        /// 组合号
        /// </summary>
        [Column("number")]
        public int Number { get; set; }

        /// <summary>
        /// 组合名称
        /// </summary>
        [Column("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// 组合时值，如：0.5-八分音符，0.25-十六分音符，0.125-三十二分音符，默认0.5
        /// </summary>
        [Column("duration")]
        public float Duration { get; set; }

    }
}
