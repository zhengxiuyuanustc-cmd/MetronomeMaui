using System.Timers;
using Plugin.Maui.Audio;

namespace MetronomeMaui;

public partial class MainPage : ContentPage
{
    private readonly IAudioManager _audioManager;
    private IAudioPlayer? _audioPlayerStrong; // 强拍播放器
    private IAudioPlayer? _audioPlayerWeak;   // 弱拍播放器
    private readonly System.Timers.Timer _timer;
    private bool _isPlaying = false;
    private int _currentBpm = 120;
    private int _intervalMs = 500;
    private int _currentBeat = 1; // 当前节拍位置 1~4

    public MainPage(IAudioManager audioManager)
    {
        InitializeComponent();
        _audioManager = audioManager;
        LoadAudio();

        _timer = new System.Timers.Timer(_intervalMs);
        _timer.Elapsed += OnTimerTick;
        _timer.AutoReset = true;

        // 绑定 Entry 初始值
        BpmEntry.Text = _currentBpm.ToString();
    }

    private async void LoadAudio()
    {
        // 加载强拍音频
        var strongStream = await FileSystem.OpenAppPackageFileAsync("tick1.mp3");
        _audioPlayerStrong = _audioManager.CreatePlayer(strongStream);
        _audioPlayerStrong.Loop = false;

        // 加载弱拍音频
        var weakStream = await FileSystem.OpenAppPackageFileAsync("tick2.mp3");
        _audioPlayerWeak = _audioManager.CreatePlayer(weakStream);
        _audioPlayerWeak.Loop = false;
    }

    // 手动输入 BPM 的回调（按回车后触发）
    private void OnBpmEntryCompleted(object sender, EventArgs e)
    {
        if (int.TryParse(BpmEntry.Text, out int newBpm))
        {
            // 限制范围 40~208
            newBpm = Math.Clamp(newBpm, 40, 208);
            _currentBpm = newBpm;
            BpmSlider.Value = newBpm;
            UpdateBpmDisplay();
        }
        else
        {
            // 输入无效，恢复显示当前 BPM
            BpmEntry.Text = _currentBpm.ToString();
        }
    }

    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        _currentBpm = (int)e.NewValue;
        UpdateBpmDisplay();
        _intervalMs = (int)(60000.0 / _currentBpm);

        if (_isPlaying)
        {
            _timer.Stop();
            _timer.Interval = _intervalMs;
            _timer.Start();
        }
        else
        {
            _timer.Interval = _intervalMs;
        }
    }

    private void UpdateBpmDisplay()
    {
        BpmLabel.Text = $"{_currentBpm} BPM";
        BpmEntry.Text = _currentBpm.ToString();
    }

    private void OnTimerTick(object? sender, ElapsedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // 播放对应的音频
            if (_currentBeat == 1)
                _audioPlayerStrong?.Play();
            else
                _audioPlayerWeak?.Play();

            // 点亮对应的节拍点
            HighlightBeat(_currentBeat);

            // 更新节拍计数器（1->2->3->4->1）
            _currentBeat++;
            if (_currentBeat > 4) _currentBeat = 1;
        });
    }

    // 高亮当前拍点，然后恢复
    private async void HighlightBeat(int beat)
    {
        BoxView targetBeat = beat switch
        {
            1 => Beat1,
            2 => Beat2,
            3 => Beat3,
            4 => Beat4,
            _ => Beat1
        };

        // 改变颜色为绿色（或其他高亮色）
        targetBeat.Color = Colors.Green;
        await Task.Delay(50);
        targetBeat.Color = Colors.LightGray;
    }

    private void OnStartStopClicked(object sender, EventArgs e)
    {
        if (_isPlaying)
        {
            _timer.Stop();
            StartStopButton.Text = "▶️ 启动";
            _isPlaying = false;
            // 停止时重置拍子计数器
            _currentBeat = 1;
            // 重置所有节拍点颜色
            ResetBeatColors();
        }
        else
        {
            _timer.Interval = _intervalMs;
            _timer.Start();
            StartStopButton.Text = "⏸️ 停止";
            _isPlaying = true;
        }
    }

    private void ResetBeatColors()
    {
        Beat1.Color = Colors.LightGray;
        Beat2.Color = Colors.LightGray;
        Beat3.Color = Colors.LightGray;
        Beat4.Color = Colors.LightGray;
    }

    // 页面销毁时释放资源
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timer?.Stop();
        _timer?.Dispose();
        _audioPlayerStrong?.Dispose();
        _audioPlayerWeak?.Dispose();
    }
}