using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SEH.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Text;

namespace SEH.Commons
{
    /// <summary>
    /// 绘制简谱助手类
    /// </summary>
    public class DrawScoreHelper
    {
        /// <summary>
        /// 绘制简谱
        /// </summary>
        /// <param name="_score"></param>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        /// <param name="_renderElements"></param>
        public double DrawScore(Score _score, double _width, double _height, ObservableCollection<ScoreRenderElement> _renderElements)
        {
            _renderElements.Clear();

            double startX = _score.LeftMargin;
            double startY = _score.TopMargin;
            double canvasWidth = _width - _score.LeftMargin - _score.RightMargin;//设置画布宽度
            double lineHeight = 120;//设置每行高度（默认为120）
            double measureLeftPadding = 10;//小节左边距
            double measureRightPadding = 10;//小节右边距
            double noteBaseYOffset = 40;//音符在每行中的相对Y位置

            Dictionary<string, Note> dictNotes = [];//音符总字典
            Dictionary<string, int> dictNoteOrder = [];//音符总序号字典

            double currentY = startY;

            #region 1.重置页面尺寸
            if (_score.Direction == 1)//纵向
            {
                _height = 1123;
            }
            else//横向
            {
                _height = 794;
            }
            #endregion

            #region 2.绘制元信息

            #region 1.绘制标题
            if (!string.IsNullOrWhiteSpace(_score.Title))
            {
                double fontSize = 24;
                double titleWidth = CalcTextWidth(_score.Title, fontSize, FontWeights.Bold).Width; //动态计算标题宽度

                _renderElements.Add(new ScoreRenderTextElement
                {
                    X = _score.LeftMargin + (canvasWidth - titleWidth) / 2, //居中
                    Y = currentY,
                    Text = _score.Title,
                    FontSize = fontSize,
                    IsBold = true
                });
                currentY += 50;
            }
            #endregion

            #region 2.绘制调号、拍号和作曲
            {
                double metaY1 = currentY;
                double leftX1 = startX;

                //左侧：调号
                if (!string.IsNullOrWhiteSpace(_score.KeySignature))
                {
                    _renderElements.Add(new ScoreRenderTextElement
                    {
                        X = leftX1,
                        Y = metaY1,
                        Text = $"1={_score.KeySignature}",
                        FontSize = 18
                    });
                    leftX1 += 40; //调号后留出间距
                }

                //左侧：拍号（按分数上下显示）
                {
                    //绘制分子（往上偏移）
                    _renderElements.Add(new ScoreRenderTextElement
                    {
                        X = leftX1,
                        Y = metaY1 - 10,
                        Text = _score.MeasureBeatCount.ToString(),
                        FontSize = 18
                    });
                    //绘制中间的横线
                    _renderElements.Add(new ScoreRenderLineElement
                    {
                        X = leftX1,
                        Y = metaY1 + 14,
                        Width = 14,
                        Height = 1,
                        IsVertical = false
                    });
                    //绘制分母 （往下偏移）
                    _renderElements.Add(new ScoreRenderTextElement
                    {
                        X = leftX1,
                        Y = metaY1 + 8,
                        Text = _score.BeatDuration.ToString(),
                        FontSize = 18
                    });
                    leftX1 += 30; // 拍号占位
                }

                //右侧：作曲
                if (!string.IsNullOrWhiteSpace(_score.Composer))
                {
                    string composer = $"作曲: {_score.Composer}";
                    string lyricist = $"作词: {_score.Lyricist}";

                    double fontSize = 18;
                    double composerWidth = CalcTextWidth(composer, fontSize, FontWeights.Normal).Width;//动态计算作曲宽度
                    double lyricistWidth = CalcTextWidth(lyricist, fontSize, FontWeights.Normal).Width;//动态计算作词宽度
                    double maxWidth = Math.Max(composerWidth, lyricistWidth); //计算最大宽度

                    double rightX1 = _width - _score.RightMargin - maxWidth;

                    _renderElements.Add(new ScoreRenderTextElement
                    {
                        X = rightX1,
                        Y = metaY1,
                        Text = composer,
                        FontSize = fontSize
                    });
                }
            }
            #endregion

            #region 3.绘制速度和作词
            {
                double metaY2 = currentY + 35; //下移 35 像素作为第二行
                double leftX2 = startX;

                //左侧：速度
                if (!string.IsNullOrWhiteSpace(_score.Tempo.ToString()))
                {
                    _renderElements.Add(new ScoreRenderTextElement
                    {
                        X = leftX2,
                        Y = metaY2,
                        Text = $"♩={_score.Tempo}",
                        FontSize = 18
                    });
                }

                //右侧：作词
                if (!string.IsNullOrWhiteSpace(_score.Lyricist))
                {
                    string composer = $"作曲: {_score.Composer}";
                    string lyricist = $"作词: {_score.Lyricist}";

                    double fontSize = 18;
                    double composerWidth = CalcTextWidth(composer, fontSize, FontWeights.Normal).Width;//动态计算作曲宽度
                    double lyricistWidth = CalcTextWidth(lyricist, fontSize, FontWeights.Normal).Width;//动态计算作词宽度
                    double maxWidth = Math.Max(composerWidth, lyricistWidth);//计算最大宽度

                    double rightX2 = _width - _score.RightMargin - maxWidth;

                    _renderElements.Add(new ScoreRenderTextElement
                    {
                        X = rightX2,
                        Y = metaY2,
                        Text = lyricist,
                        FontSize = fontSize
                    });
                }
            }
            #endregion

            #region 4.绘制副标题
            if (!string.IsNullOrWhiteSpace(_score.Subtitle))
            {
                double fontSize = 18;
                Size subTitleSize = CalcTextWidth(_score.Subtitle, fontSize, FontWeights.Normal);//动态计算标题宽度

                _renderElements.Add(new ScoreRenderTextElement
                {
                    X = _score.LeftMargin + (canvasWidth - subTitleSize.Width) / 2, //水平居中
                    Y = currentY + (75 - subTitleSize.Height) / 2,//垂直居中
                    Text = _score.Subtitle,
                    FontSize = fontSize
                });
            }
            #endregion

            currentY += 75;

            #endregion

            #region 3.绘制简谱
            if (_score.Lines != null && _score.Lines.Count > 0)
            {
                int measureIndex = 1;//小节总序号
                int nodeIndex = 1;//音符总序号

                foreach (var line in _score.Lines)
                {
                    #region 1.计算行高
                    {
                        lineHeight = 120;//重置行高

                        if (line.Measures != null && line.Measures.Count > 0)
                        {
                            foreach (var measure in line.Measures)
                            {
                                if (measure.Notes != null && measure.Notes.Count > 0)
                                {
                                    foreach (var note in measure.Notes)
                                    {
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics2))
                                        {
                                            if (lineHeight < 140)
                                            {
                                                lineHeight = 140;
                                            }
                                        }
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics3))
                                        {
                                            if (lineHeight < 160)
                                            {
                                                lineHeight = 160;
                                            }
                                        }
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics4))
                                        {
                                            if (lineHeight < 180)
                                            {
                                                lineHeight = 180;
                                            }
                                        }
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics5))
                                        {
                                            if (lineHeight < 200)
                                            {
                                                lineHeight = 200;
                                            }
                                        }
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics6))
                                        {
                                            if (lineHeight < 220)
                                            {
                                                lineHeight = 220;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        line.Height = lineHeight;

                        if (currentY + line.Height + _score.BottomMargin > _height)
                        {
                            currentY = (_height + startY);

                            if (_score.Direction == 1)//纵向
                            {
                                _height += 1123;
                            }
                            else//横向
                            {
                                _height += 794;
                            }
                        }
                    }
                    #endregion

                    #region 2.计算当前行音符占位宽度
                    double currentLineBeats = 0;//当前行累计拍数
                    double currentLineNotes = 0;//当前行累计音符数
                    if (line.Measures != null && line.Measures.Count > 0)
                    {
                        foreach (var measure in line.Measures)
                        {
                            if (measure.Notes != null && measure.Notes.Count > 0)
                            {
                                //一个占位音符为一拍
                                foreach (var note in measure.Notes)
                                {
                                    if (string.IsNullOrWhiteSpace(note.BeamId))
                                    {
                                        if (note.Duration == 1)//四分音符
                                        {
                                            currentLineBeats += 1;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentLineBeats += (1 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentLineBeats += (1 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentLineBeats += (1 * 7.0 / 8.0);
                                            }
                                        }
                                        else if (note.Duration == 0.5)//八分音符
                                        {
                                            currentLineBeats += 0.5;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentLineBeats += (0.5 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentLineBeats += (0.5 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentLineBeats += (0.5 * 7.0 / 8.0);
                                            }
                                        }
                                        else if (note.Duration == 0.25)//十六分音符
                                        {
                                            currentLineBeats += 0.25;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentLineBeats += (0.25 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentLineBeats += (0.25 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentLineBeats += (0.25 * 7.0 / 8.0);
                                            }
                                        }
                                        else if (note.Duration == 0.125)//三十二分音符
                                        {
                                            currentLineBeats += 0.125;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentLineBeats += (0.125 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentLineBeats += (0.125 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentLineBeats += (0.125 * 7.0 / 8.0);
                                            }
                                        }
                                    }
                                }
                                //计算组合拍数，一个组合为一拍
                                if (measure.Beams != null && measure.Beams.Count > 0)
                                {
                                    foreach (var beam in measure.Beams)
                                    {
                                        if (measure.Notes != null)
                                        {
                                            var notesInBeam = measure.Notes.Where(n => n.BeamId == beam.Id);
                                            if (notesInBeam.Any())
                                            {
                                                currentLineBeats++;
                                            }
                                        }
                                    }
                                }
                                currentLineNotes += (measure.Notes?.Count) ?? 0;
                            }
                        }
                    }
                    //如果当前累计拍数小于规定拍数，则添加音符占位
                    if (line.MeasureCount * _score.MeasureBeatCount > currentLineBeats)
                    {
                        currentLineNotes += (line.MeasureCount * _score.MeasureBeatCount - currentLineBeats);
                    }
                    //计算当前行每个音符占位宽度
                    line.NoteWidth = (canvasWidth - line.MeasureCount * (measureLeftPadding + measureRightPadding)) / currentLineNotes;
                    #endregion

                    #region 3.绘制行起点竖线
                    //每行的高度为：rowHeight，上边距为：20，下边距为：20，中间绘制区高度为：80
                    //-------------------------
                    //   20
                    //-------------------------
                    //           20
                    //        -----------------
                    //
                    //   80      40 音符绘制区
                    //
                    //        -----------------
                    //           20
                    //-------------------------
                    //   20
                    //-------------------------
                    _renderElements.Add(new ScoreRenderLineElement
                    {
                        X = startX,
                        Y = currentY + 40,
                        Width = 1,
                        Height = 40,
                        IsVertical = true
                    });
                    #endregion

                    #region 4.绘制小节
                    if (line.Measures != null && line.Measures.Count > 0)
                    {
                        double currentX = startX;

                        foreach (var measure in line.Measures)
                        {
                            #region 1.绘制小节序号
                            _renderElements.Add(new ScoreRenderTextElement
                            {
                                FontSize = 8,
                                X = currentX,
                                Y = currentY + 20,
                                Text = measureIndex.ToString(),
                            });
                            #endregion

                            #region 2.绘制小节左边线（0-无，1-小节线，2-反复起始线）
                            if (measure.LeftLine == 0)//无
                            {
                                //默认不绘制
                            }
                            else if (measure.LeftLine == 1)//小节线
                            {
                                //默认不绘制左小节线
                            }
                            else if (measure.LeftLine == 2)//反复起始线
                            {
                                #region 绘制反复起始线
                                //查找上一个小节是否为反复终止线
                                int i = 0;
                                while (i < line.Measures.Count)
                                {
                                    if (line.Measures[i].Number == measure.Number - 1 && line.Measures[i].RightLine == 4)
                                    {
                                        break;
                                    }
                                    i++;
                                }
                                if (i < line.Measures.Count)
                                {
                                    #region 合并反复起始线和反复终止线
                                    _renderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX,
                                        Y = currentY + 40,
                                        Width = 2,
                                        Height = 40,
                                        IsVertical = true
                                    });

                                    _renderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX + 3,
                                        Y = currentY + 40,
                                        Width = 1,
                                        Height = 40,
                                        IsVertical = true
                                    });

                                    _renderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + 5,
                                        Y = currentY + 40 + 10,
                                        Radius = 2
                                    });

                                    _renderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + 5,
                                        Y = currentY + 40 + 30,
                                        Radius = 2
                                    });
                                    #endregion
                                }
                                else
                                {
                                    #region 绘制独立反复起始线
                                    _renderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX,
                                        Y = currentY + 40,
                                        Width = 3,
                                        Height = 40,
                                        IsVertical = true
                                    });

                                    _renderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX + 4,
                                        Y = currentY + 40,
                                        Width = 1,
                                        Height = 40,
                                        IsVertical = true
                                    });

                                    _renderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + 6,
                                        Y = currentY + 40 + 10,
                                        Radius = 2
                                    });

                                    _renderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + 6,
                                        Y = currentY + 40 + 30,
                                        Radius = 2
                                    });
                                    #endregion
                                }
                                #endregion
                            }
                            currentX += measureLeftPadding;
                            #endregion

                            #region 3.绘制音符
                            if (measure.Notes != null && measure.Notes.Count > 0)
                            {
                                double currentMeasureBeats = 0;

                                foreach (var note in measure.Notes)
                                {
                                    #region 1.绘制音高
                                    switch (note.Pitch)
                                    {
                                        #region 绘制中音
                                        case "1":
                                        case "2":
                                        case "3":
                                        case "4":
                                        case "5":
                                        case "6":
                                        case "7":
                                            {
                                                var size = CalcTextWidth(note.Pitch, 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                _renderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = note.Pitch,
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制低音
                                        case "-1":
                                        case "-2":
                                        case "-3":
                                        case "-4":
                                        case "-5":
                                        case "-6":
                                        case "-7":
                                            {
                                                var size = CalcTextWidth(note.Pitch.Replace("-", ""), 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                _renderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = note.Pitch.Replace("-", ""),
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制倍低
                                        case "--1":
                                        case "--2":
                                        case "--3":
                                        case "--4":
                                        case "--5":
                                        case "--6":
                                        case "--7":
                                            {
                                                var size = CalcTextWidth(note.Pitch.Replace("--", ""), 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                _renderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = note.Pitch.Replace("--", ""),
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制超低
                                        case "---1":
                                        case "---2":
                                        case "---3":
                                        case "---4":
                                        case "---5":
                                        case "---6":
                                        case "---7":
                                            {
                                                var size = CalcTextWidth(note.Pitch.Replace("---", ""), 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                _renderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = note.Pitch.Replace("---", ""),
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制高音
                                        case "+1":
                                        case "+2":
                                        case "+3":
                                        case "+4":
                                        case "+5":
                                        case "+6":
                                        case "+7":
                                            {
                                                var size = CalcTextWidth(note.Pitch.Replace("+", ""), 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                _renderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = note.Pitch.Replace("+", ""),
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制倍高
                                        case "++1":
                                        case "++2":
                                        case "++3":
                                        case "++4":
                                        case "++5":
                                        case "++6":
                                        case "++7":
                                            {
                                                var size = CalcTextWidth(note.Pitch.Replace("++", ""), 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                _renderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = note.Pitch.Replace("++", ""),
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制极高
                                        case "+++1":
                                        case "+++2":
                                        case "+++3":
                                        case "+++4":
                                        case "+++5":
                                        case "+++6":
                                        case "+++7":
                                            {
                                                var size = CalcTextWidth(note.Pitch.Replace("+++", ""), 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                _renderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = note.Pitch.Replace("+++", ""),
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制休止符
                                        case "0":
                                            {
                                                var size = CalcTextWidth("0", 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                _renderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = "0",
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制噪音符
                                        case "X":
                                            {
                                                var size = CalcTextWidth("X", 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                _renderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = "X",
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制增时符
                                        case "-":
                                            {
                                                //增时符宽度固定为10
                                                var noteBaseXOffset = (double)(line.NoteWidth - 10 > 0 ? (line.NoteWidth - 10) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = 10;
                                                note.Height = 30;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                _renderElements.Add(new ScoreRenderLineElement
                                                {
                                                    X = (double)note.X,
                                                    Y = (double)note.Y + 18,
                                                    Width = 10,
                                                    Height = 1,
                                                    IsVertical = false,
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制空白符
                                        case "_":
                                            break;
                                        #endregion
                                    }
                                    #endregion

                                    #region 2.绘制上方符号
                                    {
                                        double topYOffset = currentY + noteBaseYOffset;

                                        #region 1.绘制高音点
                                        if (note.Pitch.StartsWith("+++") || note.Pitch.StartsWith("++") || note.Pitch.StartsWith("+"))
                                        {
                                            if (note.X != null && note.Width != null)
                                            {
                                                _renderElements.Add(new ScoreRenderDotElement
                                                {
                                                    X = (double)(note.X + note.Width / 2 - 2),
                                                    Y = (double)topYOffset,
                                                    Radius = 4
                                                });

                                                topYOffset -= 6;
                                            }
                                        }
                                        if (note.Pitch.StartsWith("+++") || note.Pitch.StartsWith("++"))
                                        {
                                            if (note.X != null && note.Width != null)
                                            {
                                                _renderElements.Add(new ScoreRenderDotElement
                                                {
                                                    X = (double)(note.X + note.Width / 2 - 2),
                                                    Y = (double)topYOffset,
                                                    Radius = 4
                                                });

                                                topYOffset -= 6;
                                            }
                                        }
                                        if (note.Pitch.StartsWith("+++"))
                                        {
                                            if (note.X != null && note.Width != null)
                                            {
                                                _renderElements.Add(new ScoreRenderDotElement
                                                {
                                                    X = (double)(note.X + note.Width / 2 - 2),
                                                    Y = (double)topYOffset,
                                                    Radius = 4
                                                });

                                                topYOffset -= 6;
                                            }
                                        }
                                        #endregion

                                        #region 2.绘制延长号
                                        if (note.Fermata == 1)
                                        {
                                            if (note.X != null && note.Width != null)
                                            {
                                                topYOffset -= 8;

                                                _renderElements.Add(new ScoreRenderFermataElement
                                                {
                                                    X = (double)(currentX + (line.NoteWidth - 10) / 2),
                                                    Y = (double)topYOffset
                                                });
                                            }
                                        }
                                        #endregion
                                    }
                                    #endregion

                                    #region 3.绘制下方符号和歌词
                                    {
                                        double bottomYOffset = 0;

                                        #region 1.绘制减时线
                                        if (note.Duration == 0.5 || note.Duration == 0.25 || note.Duration == 0.125)
                                        {
                                            //如果是八分音符、十六分音符和三十二分音符
                                            if (note.X != null && note.Y != null && note.Width != null && note.Height != null)
                                            {
                                                //如果没有加入减时组合，则绘制减时线
                                                if (string.IsNullOrWhiteSpace(note.BeamId))
                                                {
                                                    _renderElements.Add(new ScoreRenderLineElement
                                                    {
                                                        X = (double)note.X,
                                                        Y = (double)(note.Y + note.Height),
                                                        Width = (double)note.Width,
                                                        Height = 1,
                                                        IsVertical = false
                                                    });
                                                }
                                                bottomYOffset = (double)(note.Y + note.Height);
                                            }
                                        }
                                        if (note.Duration == 0.25 || note.Duration == 0.125)
                                        {
                                            //如果是十六分音符和三十二分音符，则绘制第二条减时线
                                            if (note.X != null && note.Y != null && note.Width != null && note.Height != null)
                                            {
                                                //如果没有加入减时组合，则绘制减时线
                                                if (string.IsNullOrWhiteSpace(note.BeamId))
                                                {
                                                    _renderElements.Add(new ScoreRenderLineElement
                                                    {
                                                        X = (double)note.X,
                                                        Y = (double)(note.Y + note.Height + 3),
                                                        Width = (double)note.Width,
                                                        Height = 1,
                                                        IsVertical = false
                                                    });
                                                }
                                                bottomYOffset = (double)(note.Y + note.Height + 3);
                                            }
                                        }
                                        if (note.Duration == 0.125)
                                        {
                                            //如果是三十二分音符，则绘制第三条减时线
                                            if (note.X != null && note.Y != null && note.Width != null && note.Height != null)
                                            {
                                                //如果没有加入减时组合，则绘制减时线
                                                if (string.IsNullOrWhiteSpace(note.BeamId))
                                                {
                                                    _renderElements.Add(new ScoreRenderLineElement
                                                    {
                                                        X = (double)note.X,
                                                        Y = (double)(note.Y + note.Height + 6),
                                                        Width = (double)note.Width,
                                                        Height = 1,
                                                        IsVertical = false
                                                    });
                                                }
                                                bottomYOffset = (double)(note.Y + note.Height + 6);
                                            }
                                        }
                                        #endregion

                                        #region 2.绘制低音点
                                        if (note.Pitch.StartsWith("---") || note.Pitch.StartsWith("--") || (note.Pitch.StartsWith('-') && note.Pitch != "-"))
                                        {
                                            if (note.X != null && note.Y != null && note.Width != null && note.Height != null)
                                            {
                                                if (bottomYOffset == 0)
                                                {
                                                    bottomYOffset = (double)(note.Y + note.Height);
                                                }
                                                else
                                                {
                                                    bottomYOffset += 4;
                                                }

                                                _renderElements.Add(new ScoreRenderDotElement
                                                {
                                                    X = (double)(note.X + note.Width / 2 - 2),
                                                    Y = (double)bottomYOffset,
                                                    Radius = 4
                                                });
                                            }
                                        }
                                        if (note.Pitch.StartsWith("---") || note.Pitch.StartsWith("--"))
                                        {
                                            if (note.X != null && note.Y != null && note.Width != null)
                                            {
                                                bottomYOffset += 4;

                                                _renderElements.Add(new ScoreRenderDotElement
                                                {
                                                    X = (double)(note.X + note.Width / 2 - 2),
                                                    Y = (double)bottomYOffset,
                                                    Radius = 4
                                                });
                                            }
                                        }
                                        if (note.Pitch.StartsWith("---"))
                                        {
                                            if (note.X != null && note.Y != null && note.Width != null)
                                            {
                                                bottomYOffset += 4;

                                                _renderElements.Add(new ScoreRenderDotElement
                                                {
                                                    X = (double)(note.X + note.Width / 2 - 2),
                                                    Y = (double)bottomYOffset,
                                                    Radius = 4
                                                });
                                            }
                                        }
                                        #endregion

                                        #region 3.绘制歌词
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics) && note.X != null)
                                        {
                                            _renderElements.Add(new ScoreRenderTextElement
                                            {
                                                FontSize = 14,
                                                X = (double)note.X,
                                                Y = currentY + 100,
                                                Text = note.Lyrics
                                            });
                                        }
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics2) && note.X != null)
                                        {
                                            _renderElements.Add(new ScoreRenderTextElement
                                            {
                                                FontSize = 14,
                                                X = (double)note.X,
                                                Y = currentY + 120,
                                                Text = note.Lyrics2
                                            });
                                        }
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics3) && note.X != null)
                                        {
                                            _renderElements.Add(new ScoreRenderTextElement
                                            {
                                                FontSize = 14,
                                                X = (double)note.X,
                                                Y = currentY + 140,
                                                Text = note.Lyrics3
                                            });
                                        }
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics4) && note.X != null)
                                        {
                                            _renderElements.Add(new ScoreRenderTextElement
                                            {
                                                FontSize = 14,
                                                X = (double)note.X,
                                                Y = currentY + 160,
                                                Text = note.Lyrics4
                                            });
                                        }
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics5) && note.X != null)
                                        {
                                            _renderElements.Add(new ScoreRenderTextElement
                                            {
                                                FontSize = 14,
                                                X = (double)note.X,
                                                Y = currentY + 180,
                                                Text = note.Lyrics5
                                            });
                                        }
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics6) && note.X != null)
                                        {
                                            _renderElements.Add(new ScoreRenderTextElement
                                            {
                                                FontSize = 14,
                                                X = (double)note.X,
                                                Y = currentY + 200,
                                                Text = note.Lyrics6
                                            });
                                        }
                                        #endregion
                                    }
                                    #endregion

                                    #region 4.绘制附点
                                    if (note.Dots > 0 && note.X != null && note.Y != null && note.Width != null && note.Height != null)
                                    {
                                        for (int i = 0; i < note.Dots; i++)
                                        {
                                            _renderElements.Add(new ScoreRenderDotElement
                                            {
                                                X = (double)(note.X + note.Width + 2 + i * 5),
                                                Y = (double)(note.Y + note.Height / 2),
                                                Radius = 4
                                            });
                                        }
                                    }
                                    #endregion

                                    #region 5.绘制圆括号
                                    if (note.Paren == 1)
                                    {
                                        _renderElements.Add(new ScoreRenderTextElement
                                        {
                                            FontSize = 22,
                                            X = currentX - 4,
                                            Y = currentY + noteBaseYOffset,
                                            Text = "(",
                                        });
                                    }
                                    else if (note.Paren == 0)
                                    {
                                        _renderElements.Add(new ScoreRenderTextElement
                                        {
                                            FontSize = 22,
                                            X = currentX + line.NoteWidth - 4,
                                            Y = currentY + noteBaseYOffset,
                                            Text = ")",
                                        });
                                    }
                                    #endregion

                                    #region 6.计算当前小节音符拍数
                                    if (string.IsNullOrWhiteSpace(note.BeamId))
                                    {
                                        if (note.Duration == 1)//四分音符
                                        {
                                            currentMeasureBeats += 1;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentMeasureBeats += (1 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentMeasureBeats += (1 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentMeasureBeats += (1 * 7.0 / 8.0);
                                            }
                                        }
                                        else if (note.Duration == 0.5)//八分音符
                                        {
                                            currentMeasureBeats += 0.5;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentMeasureBeats += (0.5 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentMeasureBeats += (0.5 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentMeasureBeats += (0.5 * 7.0 / 8.0);
                                            }
                                        }
                                        else if (note.Duration == 0.25)//十六分音符
                                        {
                                            currentMeasureBeats += 0.25;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentMeasureBeats += (0.25 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentMeasureBeats += (0.25 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentMeasureBeats += (0.25 * 7.0 / 8.0);
                                            }
                                        }
                                        else if (note.Duration == 0.125)//三十二分音符
                                        {
                                            currentMeasureBeats += 0.125;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentMeasureBeats += (0.125 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentMeasureBeats += (0.125 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentMeasureBeats += (0.125 * 7.0 / 8.0);
                                            }
                                        }
                                    }
                                    #endregion

                                    currentX += line.NoteWidth;

                                    dictNotes[note.Id] = note;//存储到音符总字典
                                    dictNoteOrder[note.Id] = nodeIndex++;//存储到音符总序号字典
                                }

                                #region 计算当前小节音符组合拍数，一个组合为一拍
                                if (measure.Beams != null && measure.Beams.Count > 0)
                                {
                                    foreach (var beam in measure.Beams)
                                    {
                                        if (measure.Notes != null)
                                        {
                                            var notesInBeam = measure.Notes.Where(n => n.BeamId == beam.Id);
                                            if (notesInBeam.Any())
                                            {
                                                currentMeasureBeats++;
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region 填充占位音符宽度，如果当前小节拍数小于拍号中的小节拍数，则需要绘制空白音符占位
                                if (currentMeasureBeats < _score.MeasureBeatCount)
                                {
                                    currentX += line.NoteWidth * (_score.MeasureBeatCount - currentMeasureBeats);
                                }
                                #endregion
                            }
                            else
                            {
                                currentX += line.NoteWidth * _score.MeasureBeatCount;
                            }
                            #endregion

                            #region 4.绘制减时组合线
                            if (measure.Beams != null && measure.Beams.Count > 0)
                            {
                                foreach (var beam in measure.Beams)
                                {
                                    if (measure.Notes != null)
                                    {
                                        var beamNotes = measure.Notes.Where(n => n.BeamId == beam.Id).OrderBy(n => n.Number);
                                        if (beamNotes != null && beamNotes.Any())
                                        {
                                            #region 1.默认绘制八分音符组合线
                                            {
                                                double? beamX = beamNotes.First<Note>().X;
                                                double? beamY = beamNotes.First<Note>().Y + beamNotes.First<Note>().Height;
                                                double? beamWidth = beamNotes.Last<Note>().X - beamNotes.First<Note>().X + beamNotes.Last<Note>().Width;

                                                if (beamX > 0 && beamY > 0 && beamWidth > 0)
                                                {
                                                    _renderElements.Add(new ScoreRenderLineElement
                                                    {
                                                        X = (double)beamX,
                                                        Y = (double)beamY,
                                                        Width = (double)beamWidth,
                                                        Height = 1,
                                                        IsVertical = false
                                                    });
                                                }
                                            }
                                            #endregion

                                            #region 2.绘制十六分音符组合线
                                            {
                                                var noteSequences = FindConsecutiveNoteSequences(beamNotes.ToList(), 0.25F, 1);
                                                if (noteSequences != null && noteSequences.Count > 0)
                                                {
                                                    foreach (var noteSequence in noteSequences)
                                                    {
                                                        double? beamX = noteSequence.First<Note>().X;
                                                        double? beamY = noteSequence.First<Note>().Y + noteSequence.First<Note>().Height;
                                                        double? beamWidth = noteSequence.Last<Note>().X - noteSequence.First<Note>().X + noteSequence.Last<Note>().Width;

                                                        if (beamX > 0 && beamY > 0 && beamWidth > 0)
                                                        {
                                                            _renderElements.Add(new ScoreRenderLineElement
                                                            {
                                                                X = (double)beamX,
                                                                Y = (double)beamY + 3,
                                                                Width = (double)beamWidth,
                                                                Height = 1,
                                                                IsVertical = false
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion

                                            #region 3.绘制三十二分音符组合线
                                            {
                                                var noteSequences = FindConsecutiveNoteSequences(beamNotes.ToList(), 0.125F, 1);
                                                if (noteSequences != null && noteSequences.Count > 0)
                                                {
                                                    foreach (var noteSequence in noteSequences)
                                                    {
                                                        double? beamX = noteSequence.First<Note>().X;
                                                        double? beamY = noteSequence.First<Note>().Y + noteSequence.First<Note>().Height;
                                                        double? beamWidth = noteSequence.Last<Note>().X - noteSequence.First<Note>().X + noteSequence.Last<Note>().Width;

                                                        if (beamX > 0 && beamY > 0 && beamWidth > 0)
                                                        {
                                                            _renderElements.Add(new ScoreRenderLineElement
                                                            {
                                                                X = (double)beamX,
                                                                Y = (double)beamY + 6,
                                                                Width = (double)beamWidth,
                                                                Height = 1,
                                                                IsVertical = false
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region 5.绘制小节右边线（0-无，1-小节线，2-虚小节线，3-段落线，4-反复终止线，5-终止线）
                            if (measure.RightLine == 0)//无
                            {
                                //不绘制边线
                            }
                            else if (measure.RightLine == 1)//小节线
                            {
                                #region 绘制小节线
                                _renderElements.Add(new ScoreRenderLineElement
                                {
                                    X = currentX + measureRightPadding - 1,
                                    Y = currentY + 40,
                                    Width = 1,
                                    Height = 40,
                                    IsVertical = true
                                });
                                #endregion
                            }
                            else if (measure.RightLine == 2)//虚小节线
                            {
                                #region 绘制虚小节线
                                _renderElements.Add(new ScoreRenderLineElement
                                {
                                    X = currentX + measureRightPadding - 1,
                                    Y = currentY + 40,
                                    Width = 1,
                                    Height = 40,
                                    IsVertical = true,
                                    IsDashed = true
                                });
                                #endregion
                            }
                            else if (measure.RightLine == 3)//段落线
                            {
                                #region 绘制段落线
                                _renderElements.Add(new ScoreRenderLineElement
                                {
                                    X = currentX + measureRightPadding - 4,
                                    Y = currentY + 40,
                                    Width = 1,
                                    Height = 40,
                                    IsVertical = true
                                });

                                _renderElements.Add(new ScoreRenderLineElement
                                {
                                    X = currentX + measureRightPadding - 1,
                                    Y = currentY + 40,
                                    Width = 1,
                                    Height = 40,
                                    IsVertical = true
                                });
                                #endregion
                            }
                            else if (measure.RightLine == 4)//反复终止线
                            {
                                #region 绘制反复终止线
                                //查找下一个小节是否为反复起始线
                                int i = 0;
                                while (i < line.Measures.Count)
                                {
                                    if (line.Measures[i].Number == measure.Number + 1 && line.Measures[i].LeftLine == 2)
                                    {
                                        break;
                                    }
                                    i++;
                                }
                                if (i < line.Measures.Count)
                                {
                                    #region 合并反复终止线和反复起始线
                                    _renderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + measureRightPadding - 10,
                                        Y = currentY + 40 + 10,
                                        Radius = 2
                                    });

                                    _renderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + measureRightPadding - 10,
                                        Y = currentY + 40 + 30,
                                        Radius = 2
                                    });

                                    _renderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX + measureRightPadding - 6,
                                        Y = currentY + 40,
                                        Width = 1,
                                        Height = 40,
                                        IsVertical = true
                                    });

                                    _renderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX + measureRightPadding - 2,
                                        Y = currentY + 40,
                                        Width = 2,
                                        Height = 40,
                                        IsVertical = true
                                    });
                                    #endregion
                                }
                                else
                                {
                                    #region 绘制独立反复终止线
                                    _renderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + measureRightPadding - 11,
                                        Y = currentY + 40 + 10,
                                        Radius = 2
                                    });

                                    _renderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + measureRightPadding - 11,
                                        Y = currentY + 40 + 30,
                                        Radius = 2
                                    });

                                    _renderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX + measureRightPadding - 7,
                                        Y = currentY + 40,
                                        Width = 1,
                                        Height = 40,
                                        IsVertical = true
                                    });

                                    _renderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX + measureRightPadding - 3,
                                        Y = currentY + 40,
                                        Width = 3,
                                        Height = 40,
                                        IsVertical = true
                                    });
                                    #endregion
                                }
                                #endregion
                            }
                            else if (measure.RightLine == 5)//终止线
                            {
                                #region 绘制乐谱终止线
                                _renderElements.Add(new ScoreRenderLineElement
                                {
                                    X = currentX + measureRightPadding - 7,
                                    Y = currentY + 40,
                                    Width = 1,
                                    Height = 40,
                                    IsVertical = true
                                });

                                _renderElements.Add(new ScoreRenderLineElement
                                {
                                    X = currentX + measureRightPadding - 3,
                                    Y = currentY + 40,
                                    Width = 3,
                                    Height = 40,
                                    IsVertical = true
                                });
                                #endregion
                            }
                            currentX += measureRightPadding;
                            #endregion

                            measureIndex++;
                        }
                    }
                    #endregion

                    currentY += line.Height;
                }
            }
            #endregion

            #region 4.绘制连音线
            if (_score.Slurs != null && _score.Slurs.Count > 0)
            {
                //1.计算每条连音线的层级 (Level)
                foreach (var slur in _score.Slurs)
                {
                    if (dictNoteOrder.TryGetValue(slur.StartNoteId, out int slurStartIndex) && dictNoteOrder.TryGetValue(slur.EndNoteId, out int slurEndIndex))
                    {
                        int level = 0;

                        //检查有多少条其他连音线完全包含了当前连音线
                        foreach (var _slur in _score.Slurs)
                        {
                            if (slur.Id == _slur.Id) continue;

                            if (dictNoteOrder.TryGetValue(_slur.StartNoteId, out int _slurStartIndex) && dictNoteOrder.TryGetValue(_slur.EndNoteId, out int _slurEndIndex))
                            {
                                //如果 slur 包含 _slur
                                if (slurStartIndex <= _slurStartIndex && _slurEndIndex <= slurEndIndex)
                                {
                                    level = _slur.Level + 1;
                                }
                            }
                        }
                        slur.Level = level;
                    }
                }

                //2.根据 Level 进行绘制
                foreach (var slur in _score.Slurs)
                {
                    if (dictNotes.TryGetValue(slur.StartNoteId, out var fromNote) && dictNotes.TryGetValue(slur.EndNoteId, out var toNote) &&
                        dictNoteOrder.TryGetValue(slur.StartNoteId, out _) && dictNoteOrder.TryGetValue(slur.EndNoteId, out _))
                    {
                        if (fromNote != null && fromNote.X != null && fromNote.Y != null && fromNote.Width != null &&
                            toNote != null && toNote.X != null && toNote.Y != null && toNote.Width != null)
                        {
                            //计算起点坐标
                            double fromX = (double)(fromNote.X + fromNote.Width / 2);
                            double fromY = (double)fromNote.Y;
                            if (fromNote.Pitch.StartsWith("+++") || fromNote.Pitch.StartsWith("++") || fromNote.Pitch.StartsWith("+"))
                            {
                                fromY -= 6;
                            }
                            if (fromNote.Pitch.StartsWith("+++") || fromNote.Pitch.StartsWith("++"))
                            {
                                fromY -= 6;
                            }
                            if (fromNote.Pitch.StartsWith("+++"))
                            {
                                fromY -= 6;
                            }
                            if (fromNote.Fermata == 1)
                            {
                                fromY -= 10;
                            }
                            fromY -= 8;

                            //计算终点坐标
                            double toX = (double)(toNote.X + toNote.Width / 2);
                            double toY = (double)toNote.Y;
                            if (toNote.Pitch.StartsWith("+++") || toNote.Pitch.StartsWith("++") || toNote.Pitch.StartsWith("+"))
                            {
                                toY -= 6;
                            }
                            if (toNote.Pitch.StartsWith("+++") || toNote.Pitch.StartsWith("++"))
                            {
                                toY -= 6;
                            }
                            if (toNote.Pitch.StartsWith("+++"))
                            {
                                toY -= 6;
                            }
                            if (toNote.Fermata == 1)
                            {
                                toY -= 10;
                            }
                            toY -= 8;

                            //根据层级向上偏移：Level 0 偏移基础距离，Level 1 再多偏移一层
                            //假设每一层连音线间隔 10 像素
                            double levelOffset = slur.Level * 10;
                            fromY -= levelOffset;
                            toY -= levelOffset;

                            //判断是否跨行：如果 fromNote 和 toNote 不在同一行
                            bool isCrossLine = fromNote.LineId != toNote.LineId;
                            bool isSameLine = !isCrossLine;

                            if (isSameLine)
                            {
                                _renderElements.Add(new ScoreRenderTieLineElement
                                {
                                    X = fromX,
                                    Y = Math.Min(fromY, toY),
                                    Width = toX - fromX,
                                    Height = 1,
                                    Shape = TieLineShape.Full
                                });
                            }
                            else
                            {
                                //跨行：第一行末尾画半截
                                _renderElements.Add(new ScoreRenderTieLineElement
                                {
                                    X = fromX,
                                    Y = fromY,
                                    Width = _width - _score.RightMargin - fromX,
                                    Height = 1,
                                    Shape = TieLineShape.EndStraight
                                });

                                //跨行：下一行开头画半截
                                _renderElements.Add(new ScoreRenderTieLineElement
                                {
                                    X = _score.LeftMargin,
                                    Y = toY,
                                    Width = toX - _score.LeftMargin,
                                    Height = 1,
                                    Shape = TieLineShape.StartStraight
                                });
                            }
                        }
                    }
                }
            }
            #endregion

            #region 5.绘制分页符和页码
            {
                int pageHeight;
                if (_score.Direction == 1)//纵向
                {
                    pageHeight = 1123;
                }
                else//横向
                {
                    pageHeight = 794;
                }

                int pageCount = (int)(_height / pageHeight);

                //绘制页码
                for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
                {
                    var pageNo = $"第{pageIndex}页 共{pageCount}页";
                    var pageNoSize = CalcTextWidth(pageNo, 14, FontWeights.Normal);

                    _renderElements.Add(new ScoreRenderTextElement
                    {
                        FontSize = 14,
                        X = startX + (canvasWidth - pageNoSize.Width) / 2,
                        Y = pageIndex * pageHeight - _score.BottomMargin + (_score.BottomMargin - pageNoSize.Height) / 2,
                        Text = pageNo
                    });
                }

                //绘制分页符
                for (int pageIndex = 1; pageIndex < pageCount; pageIndex++)
                {
                    _renderElements.Add(new ScoreRenderLineElement
                    {
                        X = 0,
                        Y = pageIndex * pageHeight,
                        Width = _width,
                        Height = 1,
                        IsVertical = false,
                        IsDashed = true,
                        LineBrush = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 118, 118, 118))
                    });
                }
            }
            #endregion

            return _height;
        }

        /// <summary>
        /// 动态测量文字实际宽度
        /// </summary>
        /// <param name="_text"></param>
        /// <param name="_fontSize"></param>
        /// <returns></returns>
        private Size CalcTextWidth(string _text, double _fontSize, FontWeight _fontWeight)
        {
            //创建一个临时的 TextBlock 来测量文本宽度
            TextBlock textBlock = new()
            {
                Text = _text,
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = _fontSize,
                FontWeight = _fontWeight
            };

            //默认是 Tabular (等宽)，改为 Proportional (比例宽度)
            //设置为比例数字，使每个数字宽度自适应内容
            Microsoft.UI.Xaml.Documents.Typography.SetNumeralAlignment(textBlock, Microsoft.UI.Xaml.FontNumeralAlignment.Proportional);

            //测量文本的实际宽度
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return textBlock.DesiredSize;
        }

        /// <summary>
        /// 找出集合中所有连续的音符序列
        /// </summary>
        /// <param name="_notes">音符集合</param>
        /// <param name="_duration">时值，默认为八分音符0.5</param>
        /// <param name="_minCount">最小连续数量，默认为2（单个音符不构成序列）</param>
        /// <returns></returns>
        private List<List<Note>> FindConsecutiveNoteSequences(List<Note> _notes, float _duration = 0.5F, int _minCount = 2)
        {
            var result = new List<List<Note>>();
            var currentSequence = new List<Note>();

            foreach (var note in _notes)
            {
                //判断是否为指定时值音符
                if (note.Duration == _duration)
                {
                    currentSequence.Add(note);
                }
                else
                {
                    //遇到了非十六分音符，说明连续序列断裂
                    //如果当前序列满足最小长度要求，则加入结果集
                    if (currentSequence.Count >= _minCount)
                    {
                        result.Add(currentSequence);
                    }

                    //清空当前序列，准备寻找下一个连续序列
                    currentSequence = [];
                }
            }

            //遍历结束后，检查最后一段序列（因为可能集合的最后一个元素也是指定时值音符）
            if (currentSequence.Count >= _minCount)
            {
                result.Add(currentSequence);
            }

            return result;
        }

    }
}
