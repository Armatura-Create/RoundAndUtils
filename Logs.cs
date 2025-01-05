using static RoundEndUtils.RoundEndUtils;

namespace RoundEndUtils;
 
public static class Logs {
    public static void PrintConsole(string? message)
    {
        if (Instance.Config.Debug)
        {
            Console.WriteLine($"{Instance.ModuleName} {message}");
        }
    }
}