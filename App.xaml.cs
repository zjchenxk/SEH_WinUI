using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using SEH.Services;
using SEH.Services.Interfaces;
using SEH.ViewModels;
using SEH.Views;
using Serilog;
using System;

namespace SEH
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        //全局服务提供者
        public static IServiceProvider Services { get; private set; }

        private Window? _window;

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
            services.AddTransient<IDataService, DataService>();

            //2.注册ViewModel
            services.AddSingleton<MainViewModel>();
            services.AddTransient<EditCategoryViewModel>();
            services.AddTransient<EditScoreViewModel>();

            //3.注册窗口和页面
            services.AddSingleton<MainWindow>();
            services.AddTransient<EditCategoryPage>();
            services.AddTransient<EditScorePage>();

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
                //从容器中获取主窗口实例（此时会自动注入它所需的依赖）
                _window = Services.GetRequiredService<MainWindow>();
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
