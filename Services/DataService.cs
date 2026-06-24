using SEH.Models;
using SEH.Services.Interfaces;
using Serilog;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SEH.Services
{
    public class DataService : IDataService
    {
        private SQLiteConnection? db = null;

        public DataService()
        {
            try
            {
                string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "data.db");
                db = new SQLiteConnection(databasePath);
                if (db != null)
                {
                    db.CreateTable<Category>();
                    db.CreateTable<Score>();
                    db.CreateTable<Line>();
                    db.CreateTable<Measure>();
                    db.CreateTable<Note>();
                    db.CreateTable<Beam>();
                    db.CreateTable<BeamNote>();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
            }
        }

        ~DataService()
        {
            if (db != null)
            {
                try
                {
                    db.Close();
                    db = null;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "发生致命错误");
                }
            }
        }

        #region 类别管理
        /// <summary>
        /// 获取所有类别列表
        /// </summary>
        /// <returns></returns>
        public List<Category>? GetCategories()
        {
            try
            {
                if (db != null)
                {
                    return [.. db.Table<Category>()];
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
                return null;
            }
        }

        /// <summary>
        /// 新增类别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool AddCategory(Category data)
        {
            try
            {
                if (db != null)
                {
                    if (db.Insert(data) > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
                return false;
            }
        }

        /// <summary>
        /// 修改类别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool UpdateCategory(Category data)
        {
            try
            {
                if (db != null)
                {
                    if (db.Update(data) > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
                return false;
            }
        }

        /// <summary>
        /// 删除类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteCategory(string id)
        {
            try
            {
                if (db != null)
                {
                    if (db.Delete<Category>(id) > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
                return false;
            }
        }

        /// <summary>
        /// 判断类别名称是否存在
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsCategoryNameExists(string categoryName)
        {
            try
            {
                if (db != null)
                {
                    var category = db.Table<Category>().FirstOrDefault(u => u.Name == categoryName);
                    return category != null;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
                return false;
            }
        }
        #endregion

        #region 简谱管理
        /// <summary>
        /// 获取指定类别的简谱列表
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public List<Score>? GetScoresByCategoryId(string categoryId)
        {
            try
            {
                if (db != null)
                {
                    return [.. db.Table<Score>().Where(u => u.CategoryId == categoryId)];
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
                return null;
            }
        }

        /// <summary>
        /// 获取指定简谱记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Score? GetScore(string id)
        {
            try
            {
                if (db != null)
                {
                    var score = db.Find<Score>(id);
                    if (score != null)
                    {
                        //读取行记录
                        var lines = db.Table<Line>().Where(u => u.ScoreId == id).ToList();
                        if (lines != null && lines.Count > 0)
                        {
                            foreach (var line in lines)
                            {
                                //读取每行的小节记录
                                var measures = db.Table<Measure>().Where(u => u.ScoreId == id && u.LineId == line.Id).ToList();
                                if (measures != null && measures.Count > 0)
                                {
                                    foreach (var measure in measures)
                                    {
                                        //读取每小节的音符记录
                                        var notes = db.Table<Note>().Where(u => u.ScoreId == id && u.LineId == line.Id && u.MeasureId == measure.Id).ToList();
                                        measure.Notes = notes;

                                        //读取每小节的连尾组合记录
                                        var beams = db.Table<Beam>().Where(u => u.ScoreId == id && u.LineId == line.Id && u.MeasureId == measure.Id).ToList();
                                        if (beams != null && beams.Count > 0)
                                        {
                                            foreach (var beam in beams)
                                            {
                                                //读取每个连尾组合的音符记录
                                                var beamNotes = db.Table<BeamNote>().Where(u => u.BeamId == beam.Id).ToList();
                                                beam.Notes = beamNotes;
                                            }
                                        }
                                        measure.Beams = beams;
                                    }
                                }
                                line.Measures = measures;
                            }
                        }
                        score.Lines = lines;
                    }
                    return score;
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
                return null;
            }
        }

        /// <summary>
        /// 新增简谱记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool AddScore(Score data)
        {
            try
            {
                if (db != null)
                {
                    db.BeginTransaction();

                    //新增简谱记录
                    if (db.Insert(data) <= 0)
                    {
                        db.Rollback();
                        Log.Error($"新增简谱记录失败！{JsonSerializer.Serialize(data)}");
                        return false;
                    }

                    //新增行记录
                    if (data.Lines != null && data.Lines.Count > 0)
                    {
                        foreach (var line in data.Lines)
                        {
                            if (db.Insert(line) <= 0)
                            {
                                db.Rollback();
                                Log.Error($"新增简谱行记录失败！{JsonSerializer.Serialize(line)}");
                                return false;
                            }

                            //新增小节记录
                            if (line.Measures != null && line.Measures.Count > 0)
                            {
                                foreach (var measure in line.Measures)
                                {
                                    if (db.Insert(measure) <= 0)
                                    {
                                        db.Rollback();
                                        Log.Error($"新增简谱小节记录失败！{JsonSerializer.Serialize(measure)}");
                                        return false;
                                    }

                                    //新增音符记录
                                    if (measure.Notes != null && measure.Notes.Count > 0)
                                    {
                                        foreach (var note in measure.Notes)
                                        {
                                            if (db.Insert(note) <= 0)
                                            {
                                                db.Rollback();
                                                Log.Error($"新增简谱小节音符记录失败！{JsonSerializer.Serialize(note)}");
                                                return false;
                                            }
                                        }
                                    }

                                    //新增连尾组合记录
                                    if (measure.Beams != null && measure.Beams.Count > 0)
                                    {
                                        foreach (var beam in measure.Beams)
                                        {
                                            if (db.Insert(beam) <= 0)
                                            {
                                                db.Rollback();
                                                Log.Error($"新增简谱小节连尾组合记录失败！{JsonSerializer.Serialize(beam)}");
                                                return false;
                                            }

                                            //新增连尾组合音符记录
                                            if (beam.Notes != null && beam.Notes.Count > 0)
                                            {
                                                foreach (var beamNote in beam.Notes)
                                                {
                                                    if (db.Insert(beamNote) <= 0)
                                                    {
                                                        db.Rollback();
                                                        Log.Error($"新增简谱小节连尾组合音符记录失败！{JsonSerializer.Serialize(beamNote)}");
                                                        return false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    db.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                if (db != null)
                {
                    db.Rollback();
                }
                Log.Error(ex, "发生致命错误");
                return false;
            }
        }

        /// <summary>
        /// 修改简谱记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool UpdateScore(Score data)
        {
            try
            {
                if (db != null)
                {
                    db.BeginTransaction();

                    //修改简谱记录
                    if (db.Update(data) <= 0)
                    {
                        db.Rollback();
                        Log.Error($"修改简谱记录失败！{JsonSerializer.Serialize(data)}");
                        return false;
                    }

                    //删除行记录
                    if (!DeleteLinesByScoreId(data.Id))
                    {
                        db.Rollback();
                        Log.Error($"删除简谱行记录失败！Id={data.Id}");
                        return false;
                    }

                    //删除小节记录
                    if (!DeleteMeasuresByScoreId(data.Id))
                    {
                        db.Rollback();
                        Log.Error($"删除简谱小节记录失败！Id={data.Id}");
                        return false;
                    }

                    //删除音符记录
                    if (!DeleteNotesByScoreId(data.Id))
                    {
                        db.Rollback();
                        Log.Error($"删除简谱音符记录失败！Id={data.Id}");
                        return false;
                    }

                    //删除连尾组合音符记录
                    if (!DeleteBeamNotesByScoreId(data.Id))
                    {
                        db.Rollback();
                        Log.Error($"删除简谱连尾组合音符记录失败！Id={data.Id}");
                        return false;
                    }

                    //删除连尾组合记录
                    if (!DeleteBeamsByScoreId(data.Id))
                    {
                        db.Rollback();
                        Log.Error($"删除简谱连尾组合记录失败！Id={data.Id}");
                        return false;
                    }

                    //新增行记录
                    if (data.Lines != null && data.Lines.Count > 0)
                    {
                        foreach (var line in data.Lines)
                        {
                            if (db.Insert(line) <= 0)
                            {
                                db.Rollback();
                                Log.Error($"新增简谱行记录失败！{JsonSerializer.Serialize(line)}");
                                return false;
                            }

                            //新增小节记录
                            if (line.Measures != null && line.Measures.Count > 0)
                            {
                                foreach (var measure in line.Measures)
                                {
                                    if (db.Insert(measure) <= 0)
                                    {
                                        db.Rollback();
                                        Log.Error($"新增简谱小节记录失败！{JsonSerializer.Serialize(measure)}");
                                        return false;
                                    }

                                    //新增音符记录
                                    if (measure.Notes != null && measure.Notes.Count > 0)
                                    {
                                        foreach (var note in measure.Notes)
                                        {
                                            if (db.Insert(note) <= 0)
                                            {
                                                db.Rollback();
                                                Log.Error($"新增简谱小节音符记录失败！{JsonSerializer.Serialize(note)}");
                                                return false;
                                            }
                                        }
                                    }

                                    //新增连尾组合记录
                                    if (measure.Beams != null && measure.Beams.Count > 0)
                                    {
                                        foreach (var beam in measure.Beams)
                                        {
                                            if (db.Insert(beam) <= 0)
                                            {
                                                db.Rollback();
                                                Log.Error($"新增简谱小节连尾组合记录失败！{JsonSerializer.Serialize(beam)}");
                                                return false;
                                            }

                                            //新增连尾组合音符记录
                                            if (beam.Notes != null && beam.Notes.Count > 0)
                                            {
                                                foreach (var beamNote in beam.Notes)
                                                {
                                                    if (db.Insert(beamNote) <= 0)
                                                    {
                                                        db.Rollback();
                                                        Log.Error($"新增简谱小节连尾组合音符记录失败！{JsonSerializer.Serialize(beamNote)}");
                                                        return false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    db.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                if (db != null)
                {
                    db.Rollback();
                }
                Log.Error(ex, "发生致命错误");
                return false;
            }
        }

        /// <summary>
        /// 删除简谱记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteScore(string id)
        {
            try
            {
                if (db != null)
                {
                    db.BeginTransaction();

                    //删除简谱记录
                    if (db.Delete<Score>(id) <= 0)
                    {
                        db.Rollback();
                        Log.Error($"删除简谱记录失败！Id={id}");
                        return false;
                    }

                    //删除行记录
                    if (!DeleteLinesByScoreId(id))
                    {
                        db.Rollback();
                        Log.Error($"删除简谱行记录失败！Id={id}");
                        return false;
                    }

                    //删除小节记录
                    if (!DeleteMeasuresByScoreId(id))
                    {
                        db.Rollback();
                        Log.Error($"删除简谱小节记录失败！Id={id}");
                        return false;
                    }

                    //删除音符记录
                    if (!DeleteNotesByScoreId(id))
                    {
                        db.Rollback();
                        Log.Error($"删除简谱音符记录失败！Id={id}");
                        return false;
                    }

                    //删除连尾组合音符记录
                    if (!DeleteBeamNotesByScoreId(id))
                    {
                        db.Rollback();
                        Log.Error($"删除简谱连尾组合音符记录失败！Id={id}");
                        return false;
                    }

                    //删除连尾组合记录
                    if (!DeleteBeamsByScoreId(id))
                    {
                        db.Rollback();
                        Log.Error($"删除简谱连尾组合记录失败！Id={id}");
                        return false;
                    }

                    db.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                if (db != null)
                {
                    db.Rollback();
                }
                Log.Error(ex, "发生致命错误");
                return false;
            }
        }
        #endregion

        #region 行管理
        /// <summary>
        /// 删除指定简谱的行记录
        /// </summary>
        /// <param name="scoreId"></param>
        /// <returns></returns>
        public bool DeleteLinesByScoreId(string scoreId)
        {
            try
            {
                if (db != null)
                {
                    var lines = db.Table<Line>().Where(u => u.ScoreId == scoreId).ToList();
                    if (lines.Count > 0)
                    {
                        foreach (var line in lines)
                        {
                            if (db.Delete<Line>(line.Id) <= 0)
                            {
                                Log.Error($"删除行记录失败！Id={line.Id}");
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
                return false;
            }
        }
        #endregion

        #region 小节管理
        /// <summary>
        /// 删除指定简谱的小节记录
        /// </summary>
        /// <param name="scoreId"></param>
        /// <returns></returns>
        public bool DeleteMeasuresByScoreId(string scoreId)
        {
            try
            {
                if (db != null)
                {
                    var measures = db.Table<Measure>().Where(u => u.ScoreId == scoreId).ToList();
                    if (measures.Count > 0)
                    {
                        foreach (var measure in measures)
                        {
                            if (db.Delete<Measure>(measure.Id) <= 0)
                            {
                                Log.Error($"删除小节记录失败！Id={measure.Id}");
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
                return false;
            }
        }
        #endregion

        #region 音符管理
        /// <summary>
        /// 删除指定简谱的音符记录
        /// </summary>
        /// <param name="scoreId"></param>
        /// <returns></returns>
        public bool DeleteNotesByScoreId(string scoreId)
        {
            try
            {
                if (db != null)
                {
                    var notes = db.Table<Note>().Where(u => u.ScoreId == scoreId).ToList();
                    if (notes.Count > 0)
                    {
                        foreach (var note in notes)
                        {
                            if (db.Delete<Note>(note.Id) <= 0)
                            {
                                Log.Error($"删除音符记录失败！Id={note.Id}");
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
                return false;
            }
        }
        #endregion

        #region 音符组合管理
        /// <summary>
        /// 删除指定简谱的连尾组合记录
        /// </summary>
        /// <param name="scoreId"></param>
        /// <returns></returns>
        public bool DeleteBeamsByScoreId(string scoreId)
        {
            try
            {
                if (db != null)
                {
                    var beams = db.Table<Beam>().Where(u => u.ScoreId == scoreId).ToList();
                    if (beams.Count > 0)
                    {
                        foreach (var beam in beams)
                        {
                            if (db.Delete<Beam>(beam.Id) <= 0)
                            {
                                Log.Error($"删除连尾组合记录失败！Id={beam.Id}");
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
                return false;
            }
        }

        /// <summary>
        /// 删除指定简谱的连尾组合音符记录
        /// </summary>
        /// <param name="scoreId"></param>
        /// <returns></returns>
        public bool DeleteBeamNotesByScoreId(string scoreId)
        {
            try
            {
                if (db != null)
                {
                    var beams = db.Table<Beam>().Where(u => u.ScoreId == scoreId).ToList();
                    if (beams.Count > 0)
                    {
                        foreach (var beam in beams)
                        {
                            var beamNotes = db.Table<BeamNote>().Where(u => u.BeamId == beam.Id).ToList();
                            if (beamNotes.Count > 0)
                            {
                                foreach (var beamNote in beamNotes)
                                {
                                    if (db.Delete<BeamNote>(beamNote.Id) <= 0)
                                    {
                                        Log.Error($"删除连尾组合音符记录失败！Id={beamNote.Id}");
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
                return false;
            }
        }
        #endregion

    }
}
