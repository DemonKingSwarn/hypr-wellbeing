using System.IO;
using System.Runtime.InteropServices;
using hyprwatch.Logger;

class Program
{
    static void Main(string[] args)
  {
    if (args.Length == 0 || args[0] != "-d" && args[0] != "--show")
    {
      Console.WriteLine("Usage: -d || --show");
      return;
    }

    if(args[0] == "-d")
    {
        while(true)
        {
          WatchLog.LogCreation();
          System.Threading.Thread.Sleep(5000);
        }
    }

    if(args[0] == "--show")
    {
      Dictionary<string, string> data = new Dictionary<string, string>();

      var date = WatchLog.GetDate(); 
      string filePath = Environment.GetEnvironmentVariable("HOME") + $"/.cache/hyprwatch/daily_data/{date}.csv";

      var rawData = File.ReadAllLines(filePath);

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
      Console.WriteLine("{0,-30} {1,-30}", "App", "Time");
      Console.WriteLine(new string('-', 60));
      foreach (var entry in data)
      {
        Console.WriteLine("{0,-30} {1,-30}", entry.Key, entry.Value);
      }
    }
  }

}
