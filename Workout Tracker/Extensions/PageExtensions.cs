using Workout_Tracker.Services;

namespace Workout_Tracker.Extensions;

public static class PageExtensions
{
    public static void AddLoadingOverlay(this ContentPage page)
    {
        var original = page.Content;
        if (original == null) return;

        var loadingService = Application.Current!.Handler!.MauiContext!.Services.GetRequiredService<LoadingService>();

        var overlay = new Grid
        {
            IsVisible = false,
            InputTransparent = false,
            BackgroundColor = Colors.Black.WithAlpha(0.4f),
            Children =
            {
                new VerticalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 12,
                    Children =
                    {
                        new ActivityIndicator
                        {
                            IsRunning = true,
                            Color = Color.FromArgb("#00D9A5"),
                            WidthRequest = 48,
                            HeightRequest = 48
                        },
                        new Label
                        {
                            TextColor = Colors.White,
                            FontSize = 15,
                            HorizontalOptions = LayoutOptions.Center
                        }
                    }
                }
            }
        };

        // Bind overlay visibility
        overlay.SetBinding(VisualElement.IsVisibleProperty, new Binding(
            nameof(LoadingService.IsLoading),
            source: loadingService));

        // Bind message label
        var stack = (VerticalStackLayout)overlay.Children[0];
        var label = (Label)stack.Children[1];
        label.SetBinding(Label.TextProperty, new Binding(
            nameof(LoadingService.LoadingMessage),
            source: loadingService));

        var root = new Grid();
        root.Children.Add(original);
        root.Children.Add(overlay);

        page.Content = root;
    }
}
