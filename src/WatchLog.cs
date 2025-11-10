namespace hyprwatch.Logger
{
  using System;
  using System.IO;
  using System.Threading;
  using System.Diagnostics;
  using System.Collections.Generic;
  using Newtonsoft.Json;
  using hyprwatch.Window;
  using hyprwatch.Time;

  public class WatchLog
  {
    public static string GetTime()
    {
      string? t = null;
      string? os = null;

      string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
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
        try
        {
          Process process = new Process
          {
            StartInfo = new ProcessStartInfo
            {
              FileName = "date",
              Arguments = "+%T",
              RedirectStandardOutput = true,
              UseShellExecute = false,
              CreateNoWindow = true,
            }
          };

          process.Start();

          string output = process.StandardOutput.ReadToEnd();
          process.WaitForExit();

          t = output.Substring(0, output.Length - 1);
        }
        catch(Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      else if(os == "Windows")
      {
        t = DateTime.Now.ToString("HH:mm:ss");
      }

      return t ?? string.Empty;
    }

    public static string GetDate()
    {
      string? d = null;
      string? os = null;

      string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
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
        try
        {
          Process process = new Process
          {
            StartInfo = new ProcessStartInfo
            {
              FileName = "date",
              Arguments = "+%d-%m-%Y",
              RedirectStandardOutput = true,
              UseShellExecute = false,
              CreateNoWindow = true,
            }
          };

          process.Start();

          string output = process.StandardOutput.ReadToEnd();
          process.WaitForExit();

          d = output.Substring(0, output.Length - 1);
        }
        catch(Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      else if(os == "Windows")
      {
        try
        {
          Process process = new Process
          {
            StartInfo = new ProcessStartInfo
            {
              FileName = "powershell",
              Arguments = "-Command \"Get-Date -Format dd-MM-yyyy\"",
              RedirectStandardOutput = true,
              UseShellExecute = false,
              CreateNoWindow = true,
            }
          };

          process.Start();

          string output = process.StandardOutput.ReadToEnd();
          process.WaitForExit();

          d = output.Substring(0, output.Length - 1);
        }
        catch(Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      return d ?? string.Empty;
    }

    static void UpdateCSV(string date, Dictionary<string, string> data)
    {
      string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      string filePath = Path.Combine(homeDir, ".cache", "hyprwatch", "daily_data", $"{date}.csv");


      string? dirPath = Path.GetDirectoryName(filePath);
      if(dirPath is not null)
      {
        Directory.CreateDirectory(dirPath);
      }
      else
      {
        throw new InvalidOperationException("Invalid file path.");
      }

      var overwriteData = new List<string[]>();
      foreach(var kvp in data)
      {
        overwriteData.Add(new[] { kvp.Value, kvp.Key });
      }

      using (var writer = new StreamWriter(filePath))
      {
        foreach (var row in overwriteData)
        {
          writer.WriteLine(string.Join("\t", row));
        }
      }
    }

    static Dictionary<string, string> ImportData(string file)
    {
      var data = new Dictionary<string, string>();

      var rawData = File.ReadAllLines(file);

      foreach(var line in rawData)
      {
        var parts = line.Split('\t');
        if(parts.Length >= 2)
        {
          string key = parts[1].TrimEnd();
          string value = parts[0];
          data[key] = value;
        }
      }

      return data;
    }

    public static void LogCreation()
    {
      string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      string currentDate = GetDate();
      string filename = Path.Combine($"{homeDir}", ".cache", "hyprwatch", "daily_data", $"{currentDate}.csv");
      if(!File.Exists(filename))
      {
        string directoryPath = Path.Combine($"{homeDir}", ".cache", "Watcher", "daily_data");

        Directory.CreateDirectory(directoryPath);

        using (var fp = File.Create(filename))
        {
          // The using block ensures the file is created and closed properly
        }
      }

        //bool isAfk = false;
        //int afkTimeout = 1;
        
        var data = ImportData(filename);
        data ??= new Dictionary<string, string>();
        
        while(true)
        {
          string newDate = GetDate();
          if(newDate != currentDate)
          {
            currentDate = newDate;
            filename = Path.Combine($"{homeDir}", ".cache", "hyprwatch", "daily_data", $"{currentDate}.csv");
            data.Clear();

            using (var fp = File.Create(filename))
            {
              // The using block ensures the file is created and closed properly
            }
          }

          //Console.WriteLine(data);

          string activeWindow = GetWindows.ActiveWindow();
          Console.WriteLine(activeWindow);
          string usage = data.TryGetValue(activeWindow, out string? value) ? value : "00:00:00";

          Thread.Sleep(1000);

          usage = TimeOperations.TimeAddition("00:00:01", usage);
          data[activeWindow] = usage;

          /*if(File.Exists(filename))
          {
            UpdateCSV(GetDate(), data);
          }
          else if(!File.Exists(filename))
          {
            string newFilename = Path.Combine($"{homeDir}", ".cache", "hyprwatch", "daily_data", $"{GetDate()}.csv");
            using (var fp = File.Create(newFilename))
            {
                // The using block ensures the file is created and closed properly
            }

            data.Clear();
          

          }*/
          
          UpdateCSV(currentDate, data);
      }
    }
  }
}
