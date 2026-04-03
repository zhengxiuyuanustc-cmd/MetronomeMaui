// MauiProgram.cs
using MetronomeMaui;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio; // 引入音频库的命名空间

namespace MetronomeMaui; // 请替换为你的项目命名空间

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddTransient<MainPage>();
        // 注册音频服务，注意这里是 AddAudio()
        builder.AddAudio();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}