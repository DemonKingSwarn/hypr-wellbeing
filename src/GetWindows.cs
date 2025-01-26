namespace hyprwatch.Window 
{
  using System;
  using System.Diagnostics;
  using System.Text.RegularExpressions;

  public class GetWindows
  {
    public static string ActiveWindow()
    {
      string? activeWindow = null;

      try {
        Process process = new Process
        {
          StartInfo = new ProcessStartInfo
          {
            FileName = "hyprctl",
            Arguments = "activewindow",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
          }
        };

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        var classMatch = Regex.Match(output, @"class:(.+)");

        if(classMatch.Success)
        {
          activeWindow = classMatch.Groups[1].Value.Trim();
        }
      }
      catch(Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

      if(activeWindow == null)
      {
        activeWindow = "Home-Screen";
      }

      return activeWindow ?? string.Empty;
    }
  }
}
