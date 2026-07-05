using Microsoft.UI.Xaml.Controls;
using System;

namespace SEH.Services.Interfaces
{
    /// <summary>
    /// 导航服务接口，定义了导航功能的基本方法
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// 用于在应用启动时注入实际的 Frame 控件
        /// </summary>
        /// <param name="frame"></param>
        void Initialize(Frame frame);

        /// <summary>
        /// 导航到指定页面，并可选地传递参数
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="parameter"></param>
        void NavigateTo(Type pageType, object? parameter = null);
    }
}
