using System.Threading.Tasks;

namespace SEH.Services.Interfaces
{
    public interface IAudioService
    {
        Task PlayNoteAsync(string pitch, double durationMs);
    }
}
