// MainPage.xaml.cs
using System.Timers;
using Plugin.Maui.Audio; // 引入音频库的命名空间

namespace MetronomeMaui;

public partial class MainPage : ContentPage
{
    // 1. 将 audioManager 声明为类的私有字段，便于所有方法访问
    private readonly IAudioManager _audioManager;
    private IAudioPlayer? _audioPlayer; // 音频播放器实例
    private readonly System.Timers.Timer _timer;
    private bool _isPlaying = false;
    private int _currentBpm = 120;
    private int _intervalMs = 500;

    // 2. 通过构造函数注入 IAudioManager 服务
    public MainPage(IAudioManager audioManager)
    {
        InitializeComponent();
        _audioManager = audioManager;

        // 加载音频文件 (确保 tick.mp3 位于 Resources\Raw 目录，且生成操作为 MauiAsset)
        LoadAudio();

        // 初始化计时器
        _timer = new System.Timers.Timer(_intervalMs);
        _timer.Elapsed += OnTimerTick;
        _timer.AutoReset = true;
    }

    // 异步加载音频文件，避免阻塞UI线程
    private async void LoadAudio()
    {
        var audioStream = await FileSystem.OpenAppPackageFileAsync("tick.mp3");
        using var memoryStream = new MemoryStream();
        await audioStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        _audioPlayer = _audioManager.CreatePlayer(memoryStream);
        _audioPlayer.Loop = false; // 确保不循环播放，由计时器控制节拍
    }

    // 滑块值改变时触发
    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        _currentBpm = (int)e.NewValue;
        BpmLabel.Text = $"{_currentBpm} BPM";
        _intervalMs = (int)(60000.0 / _currentBpm);

        if (_isPlaying)
        {
            // 如果正在播放，重启计时器以应用新间隔
            _timer.Stop();
            _timer.Interval = _intervalMs;
            _timer.Start();
        }
        else
        {
            _timer.Interval = _intervalMs;
        }
    }

    // 计时器的滴答事件
    private void OnTimerTick(object? sender, ElapsedEventArgs e)
    {
        // 确保UI操作在主线程上执行
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _audioPlayer?.Play();
            FlashBpmLabel();
        });
    }

    // 启动/停止按钮逻辑
    private void OnStartStopClicked(object sender, EventArgs e)
    {
        if (_isPlaying)
        {
            _timer.Stop();
            StartStopButton.Text = "▶️ 启动";
            _isPlaying = false;
        }
        else
        {
            _timer.Interval = _intervalMs;
            _timer.Start();
            StartStopButton.Text = "⏸️ 停止";
            _isPlaying = true;
        }
    }

    // 视觉反馈：让BPM标签闪烁一下
    private async void FlashBpmLabel()
    {
        if (!Dispatcher.IsDispatchRequired) return;
        await Dispatcher.DispatchAsync(async () =>
        {
            BpmLabel.TextColor = Colors.Red;
            await Task.Delay(50);
            BpmLabel.TextColor = Colors.Black;
        });
    }
}