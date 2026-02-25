namespace Workout_Tracker.Drawables;

public record ChartDataPoint(DateTime Date, double Value);

public record ChartLine(string Name, Color Color, List<ChartDataPoint> Points, int YAxisIndex = 0);

public class LineChartDrawable : IDrawable
{
    public List<ChartLine> Series { get; }
    public Color TextColor { get; }
    public Color GridColor { get; }
    public string? YAxis1Label { get; }
    public string? YAxis2Label { get; }

    private const float DotRadius = 4f;
    private const float LineWidth = 2f;

    public LineChartDrawable(List<ChartLine> series, Color textColor, Color gridColor,
        string? yAxis1Label = null, string? yAxis2Label = null)
    {
        Series = series;
        TextColor = textColor;
        GridColor = gridColor;
        YAxis1Label = yAxis1Label;
        YAxis2Label = yAxis2Label;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var allPoints = Series.SelectMany(s => s.Points).ToList();
        if (allPoints.Count == 0) return;

        float width = dirtyRect.Width;
        float height = dirtyRect.Height;

        bool hasDualAxis = Series.Any(s => s.YAxisIndex == 1);
        float leftMargin = 48f;
        float rightMargin = hasDualAxis ? 48f : 16f;
        float topMargin = 12f;
        int legendRowCount = GetLegendRowCount(width);
        float bottomMargin = 50f + legendRowCount * 16f; // space for X labels + legend rows

        float chartLeft = leftMargin;
        float chartRight = width - rightMargin;
        float chartTop = topMargin;
        float chartBottom = height - bottomMargin;
        float chartWidth = chartRight - chartLeft;
        float chartHeight = chartBottom - chartTop;

        if (chartWidth <= 0 || chartHeight <= 0) return;

        // Compute date range
        var minDate = allPoints.Min(p => p.Date);
        var maxDate = allPoints.Max(p => p.Date);
        double dateRange = (maxDate - minDate).TotalDays;
        if (dateRange < 1) dateRange = 1;

        // Compute Y ranges per axis
        var (yMin0, yMax0) = GetYRange(Series.Where(s => s.YAxisIndex == 0));
        var (yMin1, yMax1) = hasDualAxis
            ? GetYRange(Series.Where(s => s.YAxisIndex == 1))
            : (0, 1);

        // Draw grid lines
        canvas.StrokeSize = 0.5f;
        canvas.StrokeColor = GridColor;
        canvas.FontSize = 10;

        int gridCount = 5;
        for (int i = 0; i <= gridCount; i++)
        {
            float y = chartTop + chartHeight * i / gridCount;
            canvas.DrawLine(chartLeft, y, chartRight, y);

            // Left Y axis labels
            double val0 = yMax0 - (yMax0 - yMin0) * i / gridCount;
            canvas.FontColor = Series.FirstOrDefault(s => s.YAxisIndex == 0)?.Color ?? TextColor;
            canvas.DrawString(FormatValue(val0), 0, y - 8, leftMargin - 6, 16,
                HorizontalAlignment.Right, VerticalAlignment.Center);

            // Right Y axis labels
            if (hasDualAxis)
            {
                double val1 = yMax1 - (yMax1 - yMin1) * i / gridCount;
                canvas.FontColor = Series.FirstOrDefault(s => s.YAxisIndex == 1)?.Color ?? TextColor;
                canvas.DrawString(FormatValue(val1), chartRight + 6, y - 8, rightMargin - 8, 16,
                    HorizontalAlignment.Left, VerticalAlignment.Center);
            }
        }

        // Draw X axis labels
        canvas.FontColor = TextColor;
        canvas.FontSize = 10;
        int xLabelCount = Math.Min(allPoints.Count, 6);
        for (int i = 0; i < xLabelCount; i++)
        {
            double ratio = xLabelCount <= 1 ? 0.5 : (double)i / (xLabelCount - 1);
            var date = minDate.AddDays(dateRange * ratio);
            float x = chartLeft + (float)(ratio * chartWidth);
            canvas.DrawString(date.ToString("MMM d"), x - 25, chartBottom + 8, 50, 16,
                HorizontalAlignment.Center, VerticalAlignment.Top);
        }

        // Draw series
        foreach (var series in Series)
        {
            if (series.Points.Count == 0) continue;

            var (yMin, yMax) = series.YAxisIndex == 0 ? (yMin0, yMax0) : (yMin1, yMax1);
            double yRange = yMax - yMin;
            if (yRange <= 0) yRange = 1;

            canvas.StrokeColor = series.Color;
            canvas.StrokeSize = LineWidth;
            canvas.StrokeLineCap = LineCap.Round;
            canvas.StrokeLineJoin = LineJoin.Round;

            var sortedPoints = series.Points.OrderBy(p => p.Date).ToList();

            // Build path
            var path = new PathF();
            for (int i = 0; i < sortedPoints.Count; i++)
            {
                float x = chartLeft + (float)((sortedPoints[i].Date - minDate).TotalDays / dateRange) * chartWidth;
                float y = chartTop + (float)((yMax - sortedPoints[i].Value) / yRange) * chartHeight;

                // Clamp
                x = Math.Clamp(x, chartLeft, chartRight);
                y = Math.Clamp(y, chartTop, chartBottom);

                if (i == 0)
                    path.MoveTo(x, y);
                else
                    path.LineTo(x, y);
            }

            canvas.DrawPath(path);

            // Draw dots
            canvas.FillColor = series.Color;
            for (int i = 0; i < sortedPoints.Count; i++)
            {
                float x = chartLeft + (float)((sortedPoints[i].Date - minDate).TotalDays / dateRange) * chartWidth;
                float y = chartTop + (float)((yMax - sortedPoints[i].Value) / yRange) * chartHeight;
                x = Math.Clamp(x, chartLeft, chartRight);
                y = Math.Clamp(y, chartTop, chartBottom);
                canvas.FillCircle(x, y, DotRadius);
            }
        }

        // Draw legend (multi-row, each row centered)
        canvas.FontSize = 10;
        var legendRows = BuildLegendRows(width);
        float legendBaseY = height - (legendRows.Count * 16f) + 8f;
        foreach (var row in legendRows)
        {
            float rowWidth = 0;
            foreach (var s in row)
                rowWidth += 14 + s.Name.Length * 5.5f + 10;
            rowWidth -= 10;
            float legendX = (width - rowWidth) / 2;
            foreach (var series in row)
            {
                canvas.FillColor = series.Color;
                canvas.FillCircle(legendX + 5, legendBaseY, 4);
                canvas.FontColor = TextColor;
                canvas.DrawString(series.Name, legendX + 14, legendBaseY - 7, 100, 14,
                    HorizontalAlignment.Left, VerticalAlignment.Center);
                legendX += 14 + series.Name.Length * 5.5f + 10;
            }
            legendBaseY += 16f;
        }
    }

    private static (double min, double max) GetYRange(IEnumerable<ChartLine> series)
    {
        var allValues = series.SelectMany(s => s.Points.Select(p => p.Value)).ToList();
        if (allValues.Count == 0) return (0, 1);

        double min = allValues.Min();
        double max = allValues.Max();
        double range = max - min;

        if (range < 0.001)
        {
            min -= 1;
            max += 1;
        }
        else
        {
            double padding = range * 0.1;
            min -= padding;
            max += padding;
        }

        return (min, max);
    }

    private static string FormatValue(double val)
    {
        if (Math.Abs(val) >= 1000)
            return $"{val / 1000:0.#}k";
        if (Math.Abs(val) >= 100)
            return val.ToString("0");
        if (Math.Abs(val) >= 10)
            return val.ToString("0.#");
        return val.ToString("0.##");
    }

    private List<List<ChartLine>> BuildLegendRows(float availableWidth)
    {
        var rows = new List<List<ChartLine>>();
        var currentRow = new List<ChartLine>();
        float currentWidth = 0;

        foreach (var series in Series)
        {
            float itemWidth = 14 + series.Name.Length * 5.5f + 10;
            if (currentRow.Count > 0 && currentWidth + itemWidth - 10 > availableWidth)
            {
                rows.Add(currentRow);
                currentRow = new List<ChartLine>();
                currentWidth = 0;
            }
            currentRow.Add(series);
            currentWidth += itemWidth;
        }

        if (currentRow.Count > 0)
            rows.Add(currentRow);

        return rows;
    }

    private int GetLegendRowCount(float availableWidth)
    {
        return BuildLegendRows(availableWidth).Count;
    }
}
