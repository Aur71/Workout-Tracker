using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class NewLogPage : ContentPage, IQueryAttributable
{
    private readonly NewLogViewModel _vm;

    public NewLogPage(NewLogViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;

        _vm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(NewLogViewModel.SorenessLevel))
                UpdateSorenessSelection();
            else if (e.PropertyName == nameof(NewLogViewModel.StressLevel))
                UpdateStressSelection();
            else if (e.PropertyName == nameof(NewLogViewModel.SelectedActivityLevel))
                UpdateActivitySelection();
            else if (e.PropertyName == nameof(NewLogViewModel.IsRecovery) && _vm.IsRecovery)
                BuildRecoveryChips();
            else if (e.PropertyName == nameof(NewLogViewModel.IsCalorie) && _vm.IsCalorie)
                BuildActivityChips();
        };
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Content.Opacity = 0;
        if (query.TryGetValue("type", out var typeValue))
        {
            var type = typeValue?.ToString() ?? "";

            if (query.TryGetValue("id", out var idValue) && int.TryParse(idValue?.ToString(), out int id))
            {
                PageTitle.Text = "Edit Log";
                await _vm.LoadLogAsync(type, id);
                RebuildChips();
            }
            else
            {
                _vm.SetType(type);
                RebuildChips();
            }
        }
        await Content.FadeTo(1, 250, Easing.CubicOut);
    }

    private void RebuildChips()
    {
        if (_vm.IsRecovery)
            BuildRecoveryChips();
        if (_vm.IsCalorie)
            BuildActivityChips();
    }

    private void BuildRecoveryChips()
    {
        BuildLevelChips(SorenessContainer, _vm.LevelOptions, _vm.SorenessLevel,
            level => _vm.SelectSorenessCommand.Execute(level));
        BuildLevelChips(StressContainer, _vm.LevelOptions, _vm.StressLevel,
            level => _vm.SelectStressCommand.Execute(level));
    }

    private void BuildLevelChips(Layout container, List<int> options, int? selectedLevel, Action<int> onTap)
    {
        container.Children.Clear();
        foreach (var level in options)
        {
            var isSelected = level == selectedLevel;
            var border = new Border
            {
                StrokeThickness = 0,
                WidthRequest = 48,
                HeightRequest = 40,
                BackgroundColor = isSelected
                    ? Color.FromArgb("#00D9A5")
                    : Application.Current!.RequestedTheme == AppTheme.Dark
                        ? Color.FromArgb("#252525")
                        : Color.FromArgb("#F5F5F5"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                Content = new Label
                {
                    Text = level.ToString(),
                    FontSize = 15,
                    FontAttributes = isSelected ? FontAttributes.Bold : FontAttributes.None,
                    TextColor = isSelected
                        ? Colors.Black
                        : Application.Current.RequestedTheme == AppTheme.Dark
                            ? Colors.White
                            : Color.FromArgb("#666666"),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };

            var tapGesture = new TapGestureRecognizer();
            var levelValue = level;
            tapGesture.Tapped += (s, e) => onTap(levelValue);
            border.GestureRecognizers.Add(tapGesture);

            container.Children.Add(border);
        }
    }

    private void BuildActivityChips()
    {
        ActivityContainer.Children.Clear();
        foreach (var activity in _vm.ActivityLevelOptions)
        {
            var isSelected = activity == _vm.SelectedActivityLevel;
            var border = new Border
            {
                StrokeThickness = 0,
                Padding = new Thickness(16, 8),
                Margin = new Thickness(0, 0, 8, 8),
                BackgroundColor = isSelected
                    ? Color.FromArgb("#F59E0B")
                    : Application.Current!.RequestedTheme == AppTheme.Dark
                        ? Color.FromArgb("#252525")
                        : Color.FromArgb("#F5F5F5"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                Content = new Label
                {
                    Text = activity,
                    FontSize = 14,
                    FontAttributes = isSelected ? FontAttributes.Bold : FontAttributes.None,
                    TextColor = isSelected
                        ? Colors.Black
                        : Application.Current.RequestedTheme == AppTheme.Dark
                            ? Colors.White
                            : Color.FromArgb("#666666"),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };

            var tapGesture = new TapGestureRecognizer();
            var activityValue = activity;
            tapGesture.Tapped += (s, e) => _vm.SelectActivityLevelCommand.Execute(activityValue);
            border.GestureRecognizers.Add(tapGesture);

            ActivityContainer.Children.Add(border);
        }
    }

    private void UpdateSorenessSelection()
    {
        UpdateLevelSelection(SorenessContainer, _vm.LevelOptions, _vm.SorenessLevel, "#00D9A5");
    }

    private void UpdateStressSelection()
    {
        UpdateLevelSelection(StressContainer, _vm.LevelOptions, _vm.StressLevel, "#00D9A5");
    }

    private void UpdateLevelSelection(Layout container, List<int> options, int? selectedLevel, string selectedColor)
    {
        for (int i = 0; i < options.Count && i < container.Children.Count; i++)
        {
            var border = (Border)container.Children[i];
            var isSelected = options[i] == selectedLevel;

            border.BackgroundColor = isSelected
                ? Color.FromArgb(selectedColor)
                : Application.Current!.RequestedTheme == AppTheme.Dark
                    ? Color.FromArgb("#252525")
                    : Color.FromArgb("#F5F5F5");

            if (border.Content is Label label)
            {
                label.FontAttributes = isSelected ? FontAttributes.Bold : FontAttributes.None;
                label.TextColor = isSelected
                    ? Colors.Black
                    : Application.Current.RequestedTheme == AppTheme.Dark
                        ? Colors.White
                        : Color.FromArgb("#666666");
            }
        }
    }

    private void UpdateActivitySelection()
    {
        for (int i = 0; i < _vm.ActivityLevelOptions.Count && i < ActivityContainer.Children.Count; i++)
        {
            var border = (Border)ActivityContainer.Children[i];
            var isSelected = _vm.ActivityLevelOptions[i] == _vm.SelectedActivityLevel;

            border.BackgroundColor = isSelected
                ? Color.FromArgb("#F59E0B")
                : Application.Current!.RequestedTheme == AppTheme.Dark
                    ? Color.FromArgb("#252525")
                    : Color.FromArgb("#F5F5F5");

            if (border.Content is Label label)
            {
                label.FontAttributes = isSelected ? FontAttributes.Bold : FontAttributes.None;
                label.TextColor = isSelected
                    ? Colors.Black
                    : Application.Current.RequestedTheme == AppTheme.Dark
                        ? Colors.White
                        : Color.FromArgb("#666666");
            }
        }
    }
}
