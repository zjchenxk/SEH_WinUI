using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using SEH.Services;
using SEH.Services.Interfaces;
using SEH.ViewModels;
using SEH.Views;
using Serilog;
using System;
using System.IO;

namespace SEH
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 全局服务提供者
        /// </summary>
        public static IServiceProvider? Services { get; private set; }

        /// <summary>
        /// 主窗体
        /// </summary>
        public static Window? MainWindow { get; private set; }


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            //配置依赖注入容器
            ConfigureServices();
        }

        /// <summary>
        /// 配置依赖注入容器
        /// </summary>
        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            //1.注册服务
            services.AddSingleton<IMessenger, WeakReferenceMessenger>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddTransient<IDataService, DataService>();
            services.AddTransient<IAudioService, AudioService>();

            //2.注册ViewModel
            services.AddSingleton<MainViewModel>();
            services.AddTransient<EditCategoryViewModel>();
            services.AddTransient<EditScoreViewModel>();
            services.AddTransient<ViewScoreViewModel>();
            services.AddTransient<MoveScoreViewModel>();
            services.AddTransient<EditNoteViewModel>();
            services.AddTransient<EditMeasureViewModel>();
            services.AddTransient<EditSlurViewModel>();

            //3.注册窗口和页面
            services.AddSingleton<MainWindow>();
            services.AddTransient<HomePage>();
            services.AddTransient<EditCategoryPage>();
            services.AddTransient<EditScorePage>();
            services.AddTransient<ViewScorePage>();
            services.AddTransient<MoveScorePage>();
            services.AddTransient<EditNoteDialog>();
            services.AddTransient<EditMeasureDialog>();
            services.AddTransient<EditSlurDialog>();

            //4.注册MessageService，并在获取实例时把 MainWindow 传给它
            services.AddSingleton<IMessageService>(sp =>
            {
                var window = sp.GetRequiredService<MainWindow>();
                return new MessageService(window);
            });

            //构建服务提供者
            Services = services.BuildServiceProvider();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            //获取应用程序所在的绝对路径 (通常是 bin\Debug\net8.0-windows10.0.19041.0\...)
            string baseDir = AppContext.BaseDirectory;
            string logDir = System.IO.Path.Combine(baseDir, "Logs");

            //确保日志目录存在
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            string logFilePath = System.IO.Path.Combine(logDir, "log-.txt");

            //创建并配置Logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()                       //设置最低级别
                .WriteTo.Console()                          //输出到控制台
                .WriteTo.File(                              //输出到文件
                    path: logFilePath,
                    rollingInterval: RollingInterval.Day,   //按天滚动
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();                            //构建实例

            try
            {
                //从容器中获取主窗口实例（此时会自动注入它所需的依赖）
                MainWindow = Services?.GetRequiredService<MainWindow>();
                MainWindow?.Activate();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "发生致命错误");
            }
        }
    }
}
