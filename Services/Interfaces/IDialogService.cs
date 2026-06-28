using SEH.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEH.Services.Interfaces
{
    public interface IDialogService
    {
        Task<Note>? ShowEditNoteDialogAsync(List<Beam>? beams = null, Note? note = null);
    }
}
