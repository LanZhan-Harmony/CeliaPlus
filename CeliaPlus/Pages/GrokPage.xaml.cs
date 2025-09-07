using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

namespace CeliaPlus.Pages;

public sealed partial class GrokPage : Page
{
    public GrokPage()
    {
        InitializeComponent();
    }

    private void WebView2_NavigationStarting(
        WebView2 sender,
        CoreWebView2NavigationStartingEventArgs args
    )
    {
        Data.MainWindow.IsProgressRingActive = true;
    }

    private void WebView2_NavigationCompleted(
        WebView2 sender,
        CoreWebView2NavigationCompletedEventArgs args
    )
    {
        Data.MainWindow.IsProgressRingActive = false;
    }
}
