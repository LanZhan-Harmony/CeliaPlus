using System;
using System.ComponentModel;
using CeliaPlus.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CeliaPlus;

public sealed partial class MainWindow : Window, INotifyPropertyChanged
{
    public bool IsProgressRingActive
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(IsProgressRingActive));
        }
    } = false;

    public MainWindow()
    {
        InitializeComponent();
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
}
