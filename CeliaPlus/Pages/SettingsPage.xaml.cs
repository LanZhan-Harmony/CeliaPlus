using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.Storage;

namespace CeliaPlus.Pages;

public sealed partial class SettingsPage : Page
{
    public bool IsHiddenItemsVisible
    {
        get;
        set
        {
            field = value;
            Data.MainWindow.IsHiddenItemsVisible = value
                ? Visibility.Visible
                : Visibility.Collapsed;
            SaveToSettingAsync();
        }
    } = false;

    public SettingsPage()
    {
        LoadFromSettingAsync();
        InitializeComponent();
    }

    private string GetVersionDescription()
    {
        var packageVersion = Package.Current.Id.Version;
        var version = new Version(
            packageVersion.Major,
            packageVersion.Minor,
            packageVersion.Build,
            packageVersion.Revision
        );
        return $"版本 {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    private void LoadFromSettingAsync()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        if (
            localSettings.Values.TryGetValue("IsHiddenItemsVisible", out var value)
            && value is bool isVisible
        )
        {
            IsHiddenItemsVisible = isVisible;
        }
    }

    private void SaveToSettingAsync()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        localSettings.Values["IsHiddenItemsVisible"] = IsHiddenItemsVisible;
    }
}
