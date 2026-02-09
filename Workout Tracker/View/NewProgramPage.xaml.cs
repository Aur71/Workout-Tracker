using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class NewProgramPage : ContentPage, IQueryAttributable
{
    private readonly NewProgramViewModel _vm;

    public NewProgramPage(NewProgramViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        BuildColorPicker();
        _vm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(NewProgramViewModel.SelectedColor))
                UpdateColorSelection();
        };
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Content.Opacity = 0;
        if (query.TryGetValue("edit", out var editValue) && editValue?.ToString() == "true")
        {
            PageTitle.Text = "Edit Program";

            if (query.TryGetValue("id", out var idValue) && int.TryParse(idValue?.ToString(), out int id))
            {
                await _vm.LoadProgramAsync(id);
                UpdateColorSelection();
            }
        }
        await Content.FadeTo(1, 250, Easing.CubicOut);
    }

    private void BuildColorPicker()
    {
        ColorPicker.Children.Clear();
        foreach (var color in _vm.ColorOptions)
        {
            var isSelected = color == _vm.SelectedColor;
            var border = new Border
            {
                StrokeThickness = isSelected ? 3 : 0,
                Stroke = Color.FromArgb(color),
                WidthRequest = 40,
                HeightRequest = 40,
                BackgroundColor = Color.FromArgb(color),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 },
                Content = isSelected
                    ? new Label
                    {
                        Text = "\u2713",
                        FontSize = 16,
                        TextColor = Colors.Black,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                    : null
            };

            var tapGesture = new TapGestureRecognizer();
            var colorValue = color;
            tapGesture.Tapped += (s, e) => _vm.SelectColorCommand.Execute(colorValue);
            border.GestureRecognizers.Add(tapGesture);

            ColorPicker.Children.Add(border);
        }
    }

    private void UpdateColorSelection()
    {
        for (int i = 0; i < _vm.ColorOptions.Count && i < ColorPicker.Children.Count; i++)
        {
            var border = (Border)ColorPicker.Children[i];
            var isSelected = _vm.ColorOptions[i] == _vm.SelectedColor;

            border.StrokeThickness = isSelected ? 3 : 0;
            border.Stroke = Color.FromArgb(_vm.ColorOptions[i]);
            border.Content = isSelected
                ? new Label
                {
                    Text = "\u2713",
                    FontSize = 16,
                    TextColor = Colors.Black,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
                : null;
        }
    }
}
