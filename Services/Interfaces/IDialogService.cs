using SEH.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEH.Services.Interfaces
{
    public interface IDialogService
    {
        /// <summary>
        /// 显示行编辑对话框
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        Task<Line?> ShowEditLineDialogAsync(Line? line = null);

        /// <summary>
        /// 显示小节编辑对话框
        /// </summary>
        /// <param name="measure"></param>
        /// <returns></returns>
        Task<Measure?> ShowEditMeasureDialogAsync(Measure? measure = null);

        /// <summary>
        /// 显示音符编辑对话框
        /// </summary>
        /// <param name="beams"></param>
        /// <param name="slurs"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        Task<Note?> ShowEditNoteDialogAsync(List<Beam>? beams = null, List<Slur>? slurs = null, Note? note = null);

        /// <summary>
        /// 显示连音线编辑对话框
        /// </summary>
        /// <param name="slur"></param>
        /// <returns></returns>
        Task<Slur?> ShowEditSlurDialogAsync(Slur? slur = null);
    }
}
