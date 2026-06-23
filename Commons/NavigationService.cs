using Microsoft.UI.Xaml.Controls;
using System;

namespace SEH.Commons
{
    /// <summary>
    /// 导航服务实现类，负责处理页面导航逻辑
    /// </summary>
    public class NavigationService : INavigationService
    {
        private Frame _frame;

        /// <summary>
        /// 用于在应用启动时注入实际的 Frame 控件
        /// </summary>
        /// <param name="frame"></param>
        public void Initialize(Frame frame)
        {
            _frame = frame;
        }

        /// <summary>
        /// 导航到指定页面，并可选地传递参数
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="parameter"></param>
        public void NavigateTo(Type pageType, object parameter = null)
        {
            if (_frame != null)
            {
                _frame.Navigate(pageType, parameter);
            }
        }
    }
}
