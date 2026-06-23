using System.Threading.Tasks;

namespace SEH.Services.Interfaces
{
    public interface IMessageService
    {
        /// <summary>
        /// 显示错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ShowErrorAsync(string message);
    }
}
