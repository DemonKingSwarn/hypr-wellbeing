namespace hyprwatch.Window
{
  using System;
  using System.Text;
  using System.Runtime.InteropServices;
  using System.Diagnostics;
  using System.Text.RegularExpressions;
  using Newtonsoft.Json;

  public partial class GetWindows
  {
    public static string ActiveWindow()
    {
      string desktopEnv = Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP");
      string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      string? activeWindow = null;
      string? os = null;

      string configFile = Path.Combine(homeDir, ".config", "hypr-wellbeing", "config.json");

      if(File.Exists(configFile))
      {
        string content = File.ReadAllText(configFile);
        var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
    
        os = config["os"];
      }
      else
      {
        os = "Linux";
      }

      if(os == "Linux")
      {
        try {
          Process process = new Process();
          process.StartInfo = new ProcessStartInfo();
          
          process.StartInfo.RedirectStandardOutput = true;
          process.StartInfo.UseShellExecute = false;
          process.StartInfo.CreateNoWindow = true;


          if(desktopEnv == "Hyprland")
          {
            process.StartInfo.FileName = "hyprctl";
            process.StartInfo.Arguments = "activewindow";
          } 
          else if(desktopEnv == "niri")
          {
            process.StartInfo.FileName = "niri";
            process.StartInfo.Arguments = "msg focused-window";
          }

          process.Start();

          string output = process.StandardOutput.ReadToEnd();
          process.WaitForExit();

          var classMatch = ClassRegex().Match(output);

          if(desktopEnv == "niri")
            {
              var match = Regex.Match(output, "App ID:\\s+\"([^\"]+)\"");
              if(match.Success)
              {
                activeWindow = match.Groups[1].Value.Trim();
              }
          }


          if(desktopEnv == "Hyprland")
          {
            if(classMatch.Success)
            {
              activeWindow = classMatch.Groups[1].Value.Trim();
            }
          }


        }
        catch(Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      if(activeWindow == null)
      {
        activeWindow = "Home-Screen";
      }

      return activeWindow ?? string.Empty;
    }

        [GeneratedRegex(@"class:(.+)")]
        private static partial Regex ClassRegex();
    }
}
