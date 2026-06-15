using SQLite;

namespace SEH.Models
{
    /// <summary>
    /// 连尾组合音符类
    /// </summary>
    [Table("Beam_Note")]
    public class BeamNote
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; }

        /// <summary>
        /// 组合主键
        /// </summary>
        [Column("beam_id")]
        public string BeamId { get; set; }

        /// <summary>
        /// 音符主键
        /// </summary>
        [Column("note_id")]
        public string NoteId { get; set; }
    }
}
