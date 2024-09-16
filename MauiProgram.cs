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

            // Username View
            builder.Services.AddTransient<Username>();

            // Laden ViewModel und View
            builder.Services.AddTransient<LadenViewModel>();
            builder.Services.AddTransient<Laden>();

            //Prüfungen ViewModel und View
            builder.Services.AddTransient<PruefungenViewModel>();
            builder.Services.AddTransient<Pruefungen>();

            //Teilnehmer ViewModel und View
            builder.Services.AddTransient<TeilnehmerViewModel>();
            builder.Services.AddTransient<Teilnehmer>();

            //Noten ViewModel und View
            builder.Services.AddTransient<NotenViewModel>();
            builder.Services.AddTransient<Noten>();

            //Exportieren View
            builder.Services.AddTransient<Exportieren>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
