using System.Collections.Generic;
using SQLite;
using System.Collections.Generic;

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
        /// 休止符：0
        /// 噪音符：X
        /// 增时符：-
        /// </summary>
        [Column("pitch")]
        public string Pitch { get; set; } = "";

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
        /// 延长号标记，如：1-有，0-无。延长号的形状是一个半圆弧线，弧心中间加一小圆点。
        /// </summary>
        [Column("fermata")]
        public int Fermata { get; set; }

        /// <summary>
        /// 演奏方法，如：staccato-跳音，accent-重音
        /// </summary>
        [Column("articulation")]
        public string? Articulation { get; set; }

        /// <summary>
        /// 圆括号标记，如：1-左括号，0-右括号
        /// </summary>
        [Column("paren")]
        public int? Paren { get; set; }

        /// <summary>
        /// 歌词
        /// </summary>
        [Column("lyrics")]
        public string? Lyrics { get; set; }

        /// <summary>
        /// 歌词2
        /// </summary>
        [Column("lyrics2")]
        public string? Lyrics2 { get; set; }

        /// <summary>
        /// 歌词3
        /// </summary>
        [Column("lyrics3")]
        public string? Lyrics3 { get; set; }

        /// <summary>
        /// 歌词4
        /// </summary>
        [Column("lyrics4")]
        public string? Lyrics4 { get; set; }

        /// <summary>
        /// 歌词5
        /// </summary>
        [Column("lyrics5")]
        public string? Lyrics5 { get; set; }

        /// <summary>
        /// 歌词6
        /// </summary>
        [Column("lyrics6")]
        public string? Lyrics6 { get; set; }

        /// <summary>
        /// 组合主键
        /// </summary>
        [Column("beam_id")]
        public string? BeamId { get; set; }

        /// <summary>
        /// 所属组合
        /// </summary>
        [Ignore]
        public Beam? Beam { get; set; }

        /// <summary>
        /// 开始的连音线
        /// </summary>
        [Ignore]
        public List<Slur>? StartSlurs { get; set; }

        /// <summary>
        /// 结束的连音线
        /// </summary>
        [Ignore]
        public List<Slur>? EndSlurs { get; set; }

        /// <summary>
        /// 绘制X坐标
        /// </summary>
        [Ignore]
        public double? X { get; set; }

        /// <summary>
        /// 绘制Y坐标
        /// </summary>
        [Ignore]
        public double? Y { get; set; }

        /// <summary>
        /// 绘制宽度
        /// </summary>
        [Ignore]
        public double? Width { get; set; }

        /// <summary>
        /// 绘制高度
        /// </summary>
        [Ignore]
        public double? Height { get; set; }
    }
}
