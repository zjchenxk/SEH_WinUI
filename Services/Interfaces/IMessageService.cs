using System.Threading.Tasks;

namespace SEH.Services.Interfaces
{
    public interface IMessageService
    {
        Task ShowErrorAsync(string title, string message);
    }
}
