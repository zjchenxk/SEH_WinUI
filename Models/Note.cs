using SQLite;

namespace SEH.Models
{
    /// <summary>
    /// 音符类
    /// </summary>
    [Table("Note")]
    public class Note
    {
        /// <summary>
        /// 音符主键
        /// </summary>
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; }

        /// <summary>
        /// 节主键
        /// </summary>
        [Column("measure_id")]
        public string MeasureId { get; set; }

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
        /// 音符号
        /// </summary>
        [Column("number")]
        public int Number { get; set; }

        /// <summary>
        /// 音高，如：
        /// 中音：1、2、3、4、5、6、7
        /// 低音：-1、-2、-3、-4、-5、-6、-7
        /// 倍低：--1、--2、--3、--4、--5、--6、--7
        /// 超低：---1、---2、---3、---4、---5、---6、---7
        /// 高音：+1、+2、+3、+4、+5、+6、+7
        /// 倍高：++1、++2、++3、++4、++5、++6、++7
        /// 超高：+++1、+++2、+++3、+++4、+++5、+++6、+++7
        /// </summary>
        [Column("pitch")]
        public string Pitch { get; set; }

        /// <summary>
        /// 时值，如：4-全音符，3-附点二分音符，2-二分音符，1-四分音符，0.5-八分音符，0.25-十六分音符，0.125-三十二分音符
        /// </summary>
        [Column("duration")]
        public float Duration { get; set; }

        /// <summary>
        /// 附点数量
        /// </summary>
        [Column("dots")]
        public int Dots { get; set; }

        /// <summary>
        /// 连音线标记，如：1-开始，0-表示结束
        /// </summary>
        [Column("slur")]
        public int? Slur { get; set; }

        /// <summary>
        /// 演奏方法，如：staccato-跳音，accent-重音
        /// </summary>
        [Column("articulation")]
        public string? Articulation { get; set; }

        /// <summary>
        /// 延长号标记，如：1-有，0-无。延长号的形状是一个半圆弧线，弧心中间加一小圆点。
        /// </summary>
        [Column("fermata")]
        public int Fermata { get; set; }

        /// <summary>
        /// 歌词
        /// </summary>
        [Column("lyrics")]
        public string? Lyrics { get; set; }
    }
}
