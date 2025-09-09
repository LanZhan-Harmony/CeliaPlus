using System;
using System.ComponentModel;
using System.IO;
using CeliaPlus.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;

namespace CeliaPlus;

public sealed partial class MainWindow : Window, INotifyPropertyChanged
{
    // 连击检测字段
    private readonly TimeSpan _celiaClickWindow = TimeSpan.FromSeconds(3);
    private int _celiaClickCount = 0;
    private DateTime _celiaFirstClickTime = DateTime.MinValue;

    public bool IsProgressRingActive
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(IsProgressRingActive));
        }
    } = false;

    public Visibility IsHiddenItemsVisible
    {
        get;
        set
        {
            field = value;
            SaveToSettingAsync();
            OnPropertyChanged(nameof(IsHiddenItemsVisible));
        }
    } = Visibility.Collapsed;

    public MainWindow()
    {
        LoadFromSettingAsync();
        InitializeComponent();
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        ExtendsContentIntoTitleBar = true;
        Data.MainWindow = this;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void NavigationView_Loaded(object sender, RoutedEventArgs e)
    {
        (sender as NavigationView)!.SelectedItem = (sender as NavigationView)!.MenuItems[0];
        NavigateToPage(typeof(CeliaPage));
    }

    private void NavigationView_ItemInvoked(
        NavigationView sender,
        NavigationViewItemInvokedEventArgs args
    )
    {
        if (args.InvokedItemContainer is NavigationViewItem invokedItem)
        {
            var tag = $"{invokedItem.Tag}";

            if (tag == "Celia")
            {
                var now = DateTime.UtcNow;
                if (
                    _celiaFirstClickTime == DateTime.MinValue
                    || (now - _celiaFirstClickTime) > _celiaClickWindow
                )
                {
                    _celiaFirstClickTime = now;
                    _celiaClickCount = 1;
                }
                else
                {
                    _celiaClickCount++;
                }

                if (_celiaClickCount >= 5)
                {
                    IsHiddenItemsVisible =
                        IsHiddenItemsVisible == Visibility.Visible
                            ? Visibility.Collapsed
                            : Visibility.Visible;
                    // 重置计数
                    _celiaClickCount = 0;
                    _celiaFirstClickTime = DateTime.MinValue;
                }
            }

            var pageToNavigate = tag switch
            {
                "Celia" => typeof(CeliaPage),
                "DeepSeek" => typeof(DeepSeekPage),
                "Copilot" => typeof(CopilotPage),
                "Gemini" => typeof(GeminiPage),
                "GoogleAIStudio" => typeof(GoogleAIStudioPage),
                "ChatGPT" => typeof(ChatGPTPage),
                "Grok" => typeof(GrokPage),
                "DouBao" => typeof(DouBaoPage),
                "Kimi" => typeof(KimiPage),
                "Qwen" => typeof(QwenPage),
                "Claude" => typeof(ClaudePage),
                "Settings" => typeof(SettingsPage),
                _ => typeof(CeliaPage),
            };
            NavigateToPage(pageToNavigate);
        }
    }

    private void NavigateToPage(Type pageType)
    {
        if (NavFrame.CurrentSourcePageType != pageType)
        {
            NavFrame.Navigate(pageType);
        }
    }

    private void SaveToSettingAsync()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        localSettings.Values["IsHiddenItemsVisible"] = IsHiddenItemsVisible == Visibility.Visible;
    }

    private void LoadFromSettingAsync()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        if (
            localSettings.Values.TryGetValue("IsHiddenItemsVisible", out var value)
            && value is bool isVisible
        )
        {
            IsHiddenItemsVisible = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
