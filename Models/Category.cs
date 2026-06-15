using SQLite;

namespace SEH.Models
{
    /// <summary>
    /// 类别类
    /// </summary>
    [Table("Category")]
    public class Category
    {
        /// <summary>
        /// 类别主键
        /// </summary>
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        [MaxLength(50), Unique]
        [Column("name")]
        public string Name { get; set; }
    }
}
