using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using CeliaPlus.Helpers;
using CeliaPlus.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.UI.ViewManagement;

namespace CeliaPlus;

public sealed partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly UISettings settings;

    private static readonly Dictionary<string, Type> PageMap = new()
    {
        ["Celia"] = typeof(CeliaPage),
        ["DeepSeek"] = typeof(DeepSeekPage),
        ["Copilot"] = typeof(CopilotPage),
        ["Gemini"] = typeof(GeminiPage),
        ["GoogleAIStudio"] = typeof(GoogleAIStudioPage),
        ["ChatGPT"] = typeof(ChatGPTPage),
        ["Grok"] = typeof(GrokPage),
        ["DouBao"] = typeof(DouBaoPage),
        ["Kimi"] = typeof(KimiPage),
        ["Qwen"] = typeof(QwenPage),
        ["Claude"] = typeof(ClaudePage),
        ["Settings"] = typeof(SettingsPage),
    };

    private string? CurrentPage
    {
        get;
        set
        {
            field = value;
            SaveToSettingAsync();
        }
    }

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
            OnPropertyChanged(nameof(IsHiddenItemsVisible));
        }
    } = Visibility.Collapsed;

    public MainWindow()
    {
        LoadFromSettingAsync();
        InitializeComponent();
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        ExtendsContentIntoTitleBar = true;
        settings = new UISettings();
        settings.ColorValuesChanged += Settings_ColorValuesChanged;
        ((FrameworkElement)Content).ActualThemeChanged += Window_ThemeChanged;
        Data.MainWindow = this;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void NavigationView_Loaded(object sender, RoutedEventArgs e)
    {
        var pageToNavigate = GetPageTypeByTag(CurrentPage);
        NavView.SelectedItem = FindMenuItemByTag(CurrentPage);
        NavigateToPage(pageToNavigate);
    }

    private void NavigationView_ItemInvoked(
        NavigationView sender,
        NavigationViewItemInvokedEventArgs args
    )
    {
        if (args.InvokedItemContainer is NavigationViewItem invokedItem)
        {
            NavigateToPage(GetPageTypeByTag($"{invokedItem.Tag}"));
        }
    }

    private void NavigateToPage(Type pageType)
    {
        if (NavFrame.CurrentSourcePageType != pageType)
        {
            NavFrame.Navigate(pageType);
            CurrentPage = pageType.Name.Replace("Page", string.Empty);
        }
    }

    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        DispatcherQueue.TryEnqueue(TitleBarHelper.ApplySystemThemeToCaptionButtons);
    }

    private void Window_ThemeChanged(FrameworkElement sender, object args)
    {
        TitleBarHelper.UpdateTitleBar(sender.ActualTheme);
    }

    private static Type GetPageTypeByTag(string? tag)
    {
        if (!string.IsNullOrEmpty(tag) && PageMap.TryGetValue(tag, out var type))
        {
            return type;
        }
        return typeof(CeliaPage);
    }

    private object? FindMenuItemByTag(string? tag)
    {
        if (string.IsNullOrEmpty(tag))
        {
            return NavView.MenuItems[0];
        }
        foreach (var item in NavView.MenuItems)
        {
            if (item is NavigationViewItem nvi && $"{nvi.Tag}" == tag)
            {
                return nvi;
            }
        }
        foreach (var item in NavView.FooterMenuItems)
        {
            if (item is NavigationViewItem nvi && $"{nvi.Tag}" == tag)
            {
                return nvi;
            }
        }
        return NavView.MenuItems[0];
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
        if (
            localSettings.Values.TryGetValue("CurrentPage", out var pageValue)
            && pageValue is string pageString
        )
        {
            CurrentPage = pageString;
        }
    }

    private void SaveToSettingAsync()
    {
        if (!string.IsNullOrEmpty(CurrentPage))
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["CurrentPage"] = CurrentPage;
        }
    }
}
