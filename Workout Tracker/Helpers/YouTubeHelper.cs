using System.Text.RegularExpressions;

namespace Workout_Tracker.Helpers;

public static partial class YouTubeHelper
{
    // Matches:
    // https://www.youtube.com/watch?v=VIDEO_ID
    // https://youtu.be/VIDEO_ID
    // https://www.youtube.com/embed/VIDEO_ID
    [GeneratedRegex(@"(?:youtube\.com/watch\?v=|youtu\.be/|youtube\.com/embed/)([a-zA-Z0-9_-]{11})", RegexOptions.Compiled)]
    private static partial Regex YouTubeRegex();

    public static string? ExtractVideoId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        var match = YouTubeRegex().Match(url);
        return match.Success ? match.Groups[1].Value : null;
    }

    public static string? GetEmbedUrl(string? url)
    {
        var videoId = ExtractVideoId(url);
        return videoId != null
            ? $"https://dedicated-youtube-player.netlify.app/?v={videoId}"
            : null;
    }

    public static bool IsValidYouTubeUrl(string? url)
        => ExtractVideoId(url) != null;
}
