using System.Timers;
using Plugin.Maui.Audio;
using Microsoft.Maui.Storage;

namespace MetronomeMaui;

public partial class MainPage : ContentPage
{
    private readonly IAudioManager _audioManager;
    private IAudioPlayer? _audioPlayerStrong;
    private IAudioPlayer? _audioPlayerWeak;
    private readonly System.Timers.Timer _timer;
    private bool _isPlaying = false;
    private int _currentBpm = 120;
    private int _intervalMs = 500;
    private int _currentBeat = 1;
    private bool _isComplexMode = false;

    // BPM 历史列表（最近使用，最多8个）
    private List<int> _bpmHistory = new List<int>();
    private const string BpmHistoryKey = "bpm_history_list";
    private const int MaxHistoryCount = 8;

    public MainPage(IAudioManager audioManager)
    {
        InitializeComponent();
        _audioManager = audioManager;
        LoadAudio();

        _timer = new System.Timers.Timer(_intervalMs);
        _timer.Elapsed += OnTimerTick;
        _timer.AutoReset = true;

        BpmEntry.Text = _currentBpm.ToString();

        // 加载历史记录
        LoadBpmHistory();
        RenderHistoryButtons();

        // 恢复上次的模式选择
        _isComplexMode = Preferences.Get("metronome_mode", false);
        ModeSwitch.IsToggled = _isComplexMode;
    }

    private async void LoadAudio()
    {
        var strongStream = await FileSystem.OpenAppPackageFileAsync("tick1.mp3");
        _audioPlayerStrong = _audioManager.CreatePlayer(strongStream);
        _audioPlayerStrong.Loop = false;

        var weakStream = await FileSystem.OpenAppPackageFileAsync("tick2.mp3");
        _audioPlayerWeak = _audioManager.CreatePlayer(weakStream);
        _audioPlayerWeak.Loop = false;
    }

    // ========== BPM 历史记录逻辑 ==========

    private void LoadBpmHistory()
    {
        string json = Preferences.Get(BpmHistoryKey, string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                _bpmHistory = System.Text.Json.JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
            }
            catch { _bpmHistory = new List<int>(); }
        }

        // 如果没有历史，添加默认的常用值
        if (_bpmHistory.Count == 0)
        {
            _bpmHistory = new List<int> { 120, 100, 80, 60, 140, 160 };
            SaveBpmHistory();
        }
    }

    private void SaveBpmHistory()
    {
        string json = System.Text.Json.JsonSerializer.Serialize(_bpmHistory);
        Preferences.Set(BpmHistoryKey, json);
    }

    // 记录一个 BPM 到历史（最近使用排最前）
    private void RecordBpm(int bpm)
    {
        // 如果已存在，先移除
        if (_bpmHistory.Contains(bpm))
            _bpmHistory.Remove(bpm);

        // 插入到最前面
        _bpmHistory.Insert(0, bpm);

        // 限制数量
        if (_bpmHistory.Count > MaxHistoryCount)
            _bpmHistory.RemoveRange(MaxHistoryCount, _bpmHistory.Count - MaxHistoryCount);

        SaveBpmHistory();
        RenderHistoryButtons();
    }

    private void RenderHistoryButtons()
    {
        HistoryBpmContainer.Children.Clear();
        foreach (int bpm in _bpmHistory)
        {
            Button btn = new Button
            {
                Text = bpm.ToString(),
                WidthRequest = 70,
                HeightRequest = 40,
                FontSize = 14,
                CornerRadius = 10,
                BackgroundColor = Colors.LightGray,
                TextColor = Colors.Black
            };
            btn.Clicked += (s, e) => OnHistoryBpmClicked(bpm);
            btn.Margin = new Thickness(4); // 四个方向都是4的间距
            HistoryBpmContainer.Children.Add(btn);
        }
    }

    private void OnHistoryBpmClicked(int bpm)
    {
        // 设置 BPM
        SetBpmValue(bpm, recordHistory: false); // 避免重复记录
        // 自动开始播放
        if (!_isPlaying)
        {
            StartMetronome();
        }
        // 如果已经在播放，SetBpmValue 中已经更新了计时器间隔
    }

    // ========== BPM 设置核心方法 ==========

    private void SetBpmValue(int bpm, bool recordHistory = true)
    {
        if (bpm < 40) bpm = 40;
        if (bpm > 208) bpm = 208;

        _currentBpm = bpm;
        BpmSlider.Value = bpm;
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

        if (recordHistory)
        {
            RecordBpm(_currentBpm);
        }
    }

    private void UpdateBpmDisplay()
    {
        BpmLabel.Text = $"{_currentBpm} BPM";
        BpmEntry.Text = _currentBpm.ToString();
    }

    // 启动节拍器（内部方法）
    private void StartMetronome()
    {
        if (!_isPlaying)
        {
            _timer.Interval = _intervalMs;
            _timer.Start();
            StartStopButton.Text = "⏸️ 停止";
            _isPlaying = true;
        }
    }

    // ========== UI 事件处理 ==========

    private void OnModeToggled(object sender, ToggledEventArgs e)
    {
        _isComplexMode = e.Value;
        Preferences.Set("metronome_mode", _isComplexMode);

        if (_isPlaying)
        {
            _timer.Stop();
            _currentBeat = 1;
            ResetBeatColors();
            _timer.Start();
        }
        else
        {
            _currentBeat = 1;
            ResetBeatColors();
        }
    }

    private void OnBpmEntryCompleted(object sender, EventArgs e)
    {
        if (int.TryParse(BpmEntry.Text, out int newBpm))
        {
            newBpm = Math.Clamp(newBpm, 40, 208);
            SetBpmValue(newBpm, recordHistory: true);
        }
        else
        {
            BpmEntry.Text = _currentBpm.ToString();
        }
    }

    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        int newBpm = (int)e.NewValue;
        _currentBpm = newBpm;
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

    // 滑块拖动结束时记录历史（避免滑动过程中频繁记录）
    private void OnSliderDragCompleted(object sender, EventArgs e)
    {
        RecordBpm(_currentBpm);
    }

    private void OnTimerTick(object? sender, ElapsedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (_isComplexMode)
            {
                if (_currentBeat == 1)
                    _audioPlayerStrong?.Play();
                else
                    _audioPlayerWeak?.Play();
            }
            else
            {
                _audioPlayerWeak?.Play();
            }

            HighlightBeat(_currentBeat, _isComplexMode);

            _currentBeat++;
            if (_currentBeat > 4) _currentBeat = 1;
        });
    }

    private async void HighlightBeat(int beat, bool isComplex)
    {
        BoxView targetBeat = beat switch
        {
            1 => Beat1,
            2 => Beat2,
            3 => Beat3,
            4 => Beat4,
            _ => Beat1
        };

        Color highlightColor;
        if (isComplex)
            highlightColor = (beat == 1) ? Colors.Red : Colors.Green;
        else
            highlightColor = Colors.Green;

        targetBeat.Color = highlightColor;
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
            _currentBeat = 1;
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

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timer?.Stop();
        _timer?.Dispose();
        _audioPlayerStrong?.Dispose();
        _audioPlayerWeak?.Dispose();
    }
}