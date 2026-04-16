# MetronomeMaui - Smart Metronome

[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/platform-Android-green)](https://developer.android.com/)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)

A metronome app for Android built with .NET MAUI, featuring accent beats, BPM history, and visual beat feedback.

## 📱 Features

- **Beat Mode Toggle**
  - Simple Mode: same sound for all beats
  - Accent Mode: 1st beat strong (accent), 2nd-4th beats weak
- **BPM Control**
  - Slider: 40 ~ 208 BPM
  - Manual entry: direct numeric input
- **Smart History**
  - Automatically saves recently used BPM values
  - Tap a history button to instantly switch BPM and start playing
- **Visual Beat Indicators**
  - 4 square lights, highlight sequentially with each beat
  - Accent mode: strong beat = red, weak beats = green
- **Data Persistence**
  - Auto-saves history and mode preference

## 🎵 Audio Files

Place the following audio files in `Resources/Raw/` (Build Action = `MauiAsset`):

| Filename | Usage |
|----------|-------|
| `tick1.mp3` | Strong beat (accent) |
| `tick2.mp3` | Weak beats (also used in Simple mode) |

## 🚀 How to Run

### Requirements
- Visual Studio 2022 (17.5+)
- .NET 9.0 SDK (or .NET 10.0)
- Android SDK (API 33+)

### Steps
1. Clone the repository
   ```bash
   git clone https://github.com/zhengxiuyuanustc-cmd/MetronomeMaui.git

2. Open the solution in Visual Studio

3. Select an Android emulator or a physical device

4. run

## 🛠️ Tech Stack

- .NET MAUI – cross-platform UI framework
- Plugin.Maui.Audio – low-latency audio playback
- System.Timers.Timer – beat timing
- Preferences – lightweight data storage

## 📄 License

MIT License

## 🤝 Contributing

Issues and pull requests are welcome.

## 📧 Contact

Please use GitHub Issues for any questions.