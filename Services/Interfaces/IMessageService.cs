using System.Threading.Tasks;

namespace SEH.Services.Interfaces
{
    public interface IMessageService
    {
        /// <summary>
        /// 显示错误对话框
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ShowErrorAsync(string message);

        /// <summary>
        /// 显示提示对话框
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ShowInfoAsync(string message);

        /// <summary>
        /// 显示警告对话框
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ShowWarningAsync(string message);

        /// <summary>
        /// 显示确认对话框
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<bool> ShowConfirmAsync(string message);
    }
}
