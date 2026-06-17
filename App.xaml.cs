using Microsoft.UI.Xaml;
using Serilog;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SEH
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            //创建并配置Logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()                       //设置最低级别
                .WriteTo.Console()                          //输出到控制台
                .WriteTo.File(                              //输出到文件
                    path: "Logs\\log-.txt",
                    rollingInterval: RollingInterval.Day,   //按天滚动
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();                            //构建实例

            try
            {
                _window = new MainWindow();
                _window.Activate();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
