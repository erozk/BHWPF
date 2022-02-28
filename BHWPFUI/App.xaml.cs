using AutoMapper;
using BHWPF.Data.Repository;
using BHWPF.UI.Abstract;
using BHWPF.UI.Models;
using BHWPF.UI.ViewModels;
using BHWPF.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Windows;

namespace BHWPF.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();

        }

        private void ConfigureServices(IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Data.Models.CalculationTypes, CalculationTypesModel>();
            }
            );

            var mapper = config.CreateMapper();

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddFilter("BHWPF.UI", LogLevel.Information);
            });

            ILogger logger = loggerFactory.CreateLogger(string.Format("UI_Logs_{0}_{1}", DateTime.Now.ToShortDateString(),DateTime.Now.ToShortTimeString()));

            services.AddSingleton(mapper);
            services.AddSingleton(logger);
            services.AddSingleton<IIOService, IOService>();
            services.AddSingleton<CalculationView>();
            services.AddSingleton<CalculationViewModel>();
            services.AddSingleton<ICalculationRepository, CalculationRepository>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            
            await _host.StartAsync();

            var initialView = _host.Services.GetRequiredService<CalculationView>();
            initialView.DataContext= _host.Services.GetRequiredService<CalculationViewModel>();
            initialView.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync();
            }
            
            base.OnExit(e);
        }
    }
}
