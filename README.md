# MetronomeMaui - 极简节拍器

[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/platform-Android-green)](https://developer.android.com/)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)

一款使用 .NET MAUI 开发的 Android 节拍器应用，支持强弱拍、BPM 历史记录和可视化节拍反馈。

## 📱 功能特性

- **节拍模式切换**
  - 简单模式：所有拍子使用相同音色
  - 强弱拍模式：第1拍强拍，后3拍弱拍
- **BPM 控制**
  - 滑块调节：40 ~ 208 BPM
  - 手动输入：支持键盘直接输入数值
- **智能历史记录**
  - 自动保存最近使用的 BPM 值
  - 点击历史按钮立即切换并自动启动节拍器
- **可视化节拍点**
  - 4个方形指示灯，随节拍依次高亮
  - 强弱拍模式下强拍显示红色，弱拍显示绿色
- **数据持久化**
  - 历史记录和模式偏好自动保存

## 🎵 音频文件

请在 `Resources/Raw/` 目录下准备以下音频文件（生成操作设为 `MauiAsset`）：

| 文件名 | 用途 |
|--------|------|
| `tick1.mp3` | 强拍音效 |
| `tick2.mp3` | 弱拍音效（简单模式也使用此文件） |

## 🚀 如何运行

### 环境要求
- Visual Studio 2022（17.5+）
- .NET 9.0 SDK（或 .NET 10.0）
- Android SDK（API 33+）

### 步骤
1. 克隆仓库
   ```bash
git clone https://github.com/zhengxiuyuanustc-cmd/MetronomeMaui.git

2. 用 Visual Studio 打开解决方案

3. 选择 Android 模拟器或真机

4. 运行调试

## 🛠️ 技术栈

- .NET MAUI – 跨平台 UI 框架
- Plugin.Maui.Audio – 低延迟音频播放
- System.Timers.Timer – 节拍计时
- Preferences – 轻量级数据存储

## 📄 许可证

MIT License

## 🤝 贡献

欢迎提交 Issue 和 Pull Request。

## 📧 联系方式

如有问题，请通过 GitHub Issues 联系。