using SEH.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SEH.Services
{
    public class AudioService : IAudioService
    {
        public Task PlayNoteAsync(string pitch, double durationMs)
        {
            throw new NotImplementedException();
        }
    }
}
