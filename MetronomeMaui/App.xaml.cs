using MetronomeMaui;

namespace MetronomeMaui;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    // 1. 修改构造函数，接收 IServiceProvider 参数
    public App(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
    }

    // 2. 修改 CreateWindow，从容器中获取 MainPage 实例
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var mainPage = _services.GetService<MainPage>();
        return new Window(mainPage);
    }
}