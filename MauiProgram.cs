using BergNoten.Model;
using BergNoten.View;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace BergNoten
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // AppManager
            builder.Services.AddSingleton<AppManager>();

            // Lade Page und ViewModel
            builder.Services.AddTransient<LadenViewModel>();
            builder.Services.AddTransient<Laden>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
