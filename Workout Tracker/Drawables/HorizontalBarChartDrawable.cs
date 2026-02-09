namespace Workout_Tracker.Drawables;

public record BarEntry(string Label, double Value);

public class HorizontalBarChartDrawable : IDrawable
{
    public List<BarEntry> Entries { get; }
    public Color BarColor { get; }
    public Color TextColor { get; }
    public Color GridColor { get; }

    public HorizontalBarChartDrawable(List<BarEntry> entries, Color barColor, Color textColor, Color gridColor)
    {
        Entries = entries;
        BarColor = barColor;
        TextColor = textColor;
        GridColor = gridColor;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Entries.Count == 0) return;

        var width = dirtyRect.Width;
        var height = dirtyRect.Height;

        float labelWidth = width * 0.26f;
        float valueTextWidth = 34f;
        float barAreaWidth = width - labelWidth - valueTextWidth - 8;
        float topPadding = 4f;
        float bottomPadding = 4f;
        float availableHeight = height - topPadding - bottomPadding;
        float barSpacing = 6f;
        float barHeight = (availableHeight - barSpacing * (Entries.Count - 1)) / Entries.Count;
        barHeight = Math.Min(barHeight, 26f);

        var maxValue = Entries.Max(e => e.Value);
        if (maxValue <= 0) maxValue = 1;

        canvas.FontSize = 11;
        canvas.StrokeSize = 0.5f;
        canvas.StrokeColor = GridColor;

        // Draw light grid lines
        int gridLines = 4;
        for (int g = 0; g <= gridLines; g++)
        {
            float x = labelWidth + (barAreaWidth / gridLines) * g;
            canvas.DrawLine(x, topPadding, x, topPadding + Entries.Count * (barHeight + barSpacing) - barSpacing);
        }

        for (int i = 0; i < Entries.Count; i++)
        {
            var entry = Entries[i];
            float y = topPadding + i * (barHeight + barSpacing);
            float barW = (float)(entry.Value / maxValue) * barAreaWidth;
            barW = Math.Max(barW, 4f);

            // Label
            canvas.FontColor = TextColor;
            canvas.FontSize = 11;
            canvas.DrawString(entry.Label, 0, y, labelWidth - 8, barHeight,
                HorizontalAlignment.Right, VerticalAlignment.Center);

            // Bar
            canvas.FillColor = BarColor;
            canvas.FillRoundedRectangle(labelWidth, y + 1, barW, barHeight - 2, 4);

            // Value
            canvas.FontColor = TextColor;
            canvas.FontSize = 10;
            canvas.DrawString(entry.Value.ToString("0.#"), labelWidth + barW + 6, y, valueTextWidth, barHeight,
                HorizontalAlignment.Left, VerticalAlignment.Center);
        }
    }
}
