namespace Workout_Tracker.Drawables;

public record ColumnGroup(string Label, double Value1, double Value2);

public class GroupedColumnChartDrawable : IDrawable
{
    public List<ColumnGroup> Groups { get; }
    public Color Color1 { get; }
    public Color Color2 { get; }
    public string Name1 { get; }
    public string Name2 { get; }
    public Color TextColor { get; }
    public Color GridColor { get; }

    public GroupedColumnChartDrawable(List<ColumnGroup> groups,
        Color color1, Color color2, string name1, string name2,
        Color textColor, Color gridColor)
    {
        Groups = groups;
        Color1 = color1;
        Color2 = color2;
        Name1 = name1;
        Name2 = name2;
        TextColor = textColor;
        GridColor = gridColor;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Groups.Count == 0) return;

        float width = dirtyRect.Width;
        float height = dirtyRect.Height;

        float leftMargin = 36f;
        float rightMargin = 12f;
        float topMargin = 12f;
        float bottomMargin = 66f;

        float chartLeft = leftMargin;
        float chartRight = width - rightMargin;
        float chartTop = topMargin;
        float chartBottom = height - bottomMargin;
        float chartWidth = chartRight - chartLeft;
        float chartHeight = chartBottom - chartTop;

        if (chartWidth <= 0 || chartHeight <= 0) return;

        double maxValue = Groups.Max(g => Math.Max(g.Value1, g.Value2));
        if (maxValue <= 0) maxValue = 1;

        // Draw grid lines
        canvas.StrokeSize = 0.5f;
        canvas.StrokeColor = GridColor;
        canvas.FontSize = 10;
        canvas.FontColor = TextColor;

        int gridCount = 4;
        for (int i = 0; i <= gridCount; i++)
        {
            float y = chartTop + chartHeight * i / gridCount;
            canvas.DrawLine(chartLeft, y, chartRight, y);

            double val = maxValue - maxValue * i / gridCount;
            canvas.DrawString(val.ToString("0"), 0, y - 8, leftMargin - 6, 16,
                HorizontalAlignment.Right, VerticalAlignment.Center);
        }

        // Draw baseline
        canvas.StrokeSize = 1f;
        canvas.StrokeColor = GridColor;
        canvas.DrawLine(chartLeft, chartBottom, chartRight, chartBottom);

        // Draw columns
        float groupWidth = chartWidth / Groups.Count;
        float barWidth = Math.Min(groupWidth * 0.32f, 18f);
        float gap = 3f;

        bool rotateLabels = Groups.Count > 8;

        for (int i = 0; i < Groups.Count; i++)
        {
            var group = Groups[i];
            float groupCenter = chartLeft + groupWidth * i + groupWidth / 2;

            // Bar 1 (planned/left)
            float bar1Height = (float)(group.Value1 / maxValue) * chartHeight;
            float bar1X = groupCenter - barWidth - gap / 2;
            canvas.FillColor = Color1;
            canvas.FillRoundedRectangle(bar1X, chartBottom - bar1Height, barWidth, bar1Height, 3);

            // Bar 2 (completed/right)
            float bar2Height = (float)(group.Value2 / maxValue) * chartHeight;
            float bar2X = groupCenter + gap / 2;
            canvas.FillColor = Color2;
            canvas.FillRoundedRectangle(bar2X, chartBottom - bar2Height, barWidth, bar2Height, 3);

            // X label
            canvas.FontColor = TextColor;
            canvas.FontSize = 9;

            if (rotateLabels)
            {
                // Show every other label when rotated
                if (i % 2 == 0)
                {
                    canvas.SaveState();
                    canvas.Translate(groupCenter, chartBottom + 4);
                    canvas.Rotate(45);
                    canvas.DrawString(group.Label, 0, 0, 50, 14,
                        HorizontalAlignment.Left, VerticalAlignment.Top);
                    canvas.RestoreState();
                }
            }
            else
            {
                canvas.DrawString(group.Label, groupCenter - 25, chartBottom + 4, 50, 14,
                    HorizontalAlignment.Center, VerticalAlignment.Top);
            }
        }

        // Draw legend
        float legendY = height - 8;
        float legendX = chartLeft;
        canvas.FontSize = 10;

        canvas.FillColor = Color1;
        canvas.FillRoundedRectangle(legendX, legendY - 5, 10, 10, 2);
        canvas.FontColor = TextColor;
        canvas.DrawString(Name1, legendX + 14, legendY - 7, 80, 14,
            HorizontalAlignment.Left, VerticalAlignment.Center);

        legendX += 14 + Name1.Length * 5.5f + 10;

        canvas.FillColor = Color2;
        canvas.FillRoundedRectangle(legendX, legendY - 5, 10, 10, 2);
        canvas.FontColor = TextColor;
        canvas.DrawString(Name2, legendX + 14, legendY - 7, 80, 14,
            HorizontalAlignment.Left, VerticalAlignment.Center);
    }
}
