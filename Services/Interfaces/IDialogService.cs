using SEH.Models;
using System.Threading.Tasks;

namespace SEH.Services.Interfaces
{
    public interface IDialogService
    {
        Task<Note>? ShowEditNoteDialogAsync();
    }
}
