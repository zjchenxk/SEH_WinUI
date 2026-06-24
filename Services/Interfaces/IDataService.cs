using SEH.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SEH.Services.Interfaces
{
    public interface IDataService
    {
        #region 类别管理
        /// <summary>
        /// 获取所有类别列表
        /// </summary>
        /// <returns></returns>
        List<Category>? GetCategories();

        /// <summary>
        /// 获取指定类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Category? GetCategory(string id);

        /// <summary>
        /// 新增类别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool AddCategory(Category data);

        /// <summary>
        /// 修改类别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool UpdateCategory(Category data);

        /// <summary>
        /// 删除类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteCategory(string id);

        /// <summary>
        /// 判断类别名称是否存在
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        bool IsCategoryNameExists(string categoryName, string excludeCategoryId = "");
        #endregion

        #region 简谱管理
        /// <summary>
        /// 获取指定类别的简谱列表
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        List<Score>? GetScoresByCategoryId(string categoryId);

        /// <summary>
        /// 获取指定简谱记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Score? GetScore(string id);

        /// <summary>
        /// 新增简谱记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool AddScore(Score data);

        /// <summary>
        /// 修改简谱记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool UpdateScore(Score data);

        /// <summary>
        /// 删除简谱记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteScore(string id);
        #endregion

        #region 行管理
        /// <summary>
        /// 删除指定简谱的行记录
        /// </summary>
        /// <param name="scoreId"></param>
        /// <returns></returns>
        bool DeleteLinesByScoreId(string scoreId);
        #endregion

        #region 小节管理
        /// <summary>
        /// 删除指定简谱的小节记录
        /// </summary>
        /// <param name="scoreId"></param>
        /// <returns></returns>
        bool DeleteMeasuresByScoreId(string scoreId);
        #endregion

        #region 音符管理
        /// <summary>
        /// 删除指定简谱的音符记录
        /// </summary>
        /// <param name="scoreId"></param>
        /// <returns></returns>
        bool DeleteNotesByScoreId(string scoreId);
        #endregion

        #region 音符组合管理
        /// <summary>
        /// 删除指定简谱的连尾组合记录
        /// </summary>
        /// <param name="scoreId"></param>
        /// <returns></returns>
        bool DeleteBeamsByScoreId(string scoreId);

        /// <summary>
        /// 删除指定简谱的连尾组合音符记录
        /// </summary>
        /// <param name="scoreId"></param>
        /// <returns></returns>
        bool DeleteBeamNotesByScoreId(string scoreId);
        #endregion

    }
}
