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
      string green = "\x1b[0;32m";
      string yellow = "\x1b[1;33m";
      string red = "\x1b[0;31m";
      string blue = "\x1b[0;34m";
      string reset = "\x1b[0m";

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
      Console.WriteLine($"{yellow}{"App",-30}{"Time",30}{reset}");
      Console.WriteLine($"{red}{new string('-', 60)}{reset}");
      foreach (var entry in data)
      {
        Console.WriteLine($"{blue}{entry.Key,-30}{reset}{green}{entry.Value,30}{reset}");
      }
    }
  }

}
