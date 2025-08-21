using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using hyprwatch.Time;

namespace hyprwatch.Report.Weekly
{
    public class WeeklyReport
    {
        private static string FormatTime(string time)
        {
            var parts = time.Split(':');
            if (parts.Length == 3)
            {
                int hr = int.Parse(parts[0]);
                int mn = int.Parse(parts[1]);
                int sec = int.Parse(parts[2]);
                return $"{hr:D2}:{mn:D2}:{sec:D2}";
            }
            return "00:00:00"; // Should not happen with valid data
        }

        private static int ConvertToSeconds(string time)
        {
            var parts = time.Split(':');
            if (parts.Length == 3)
            {
                int hr = int.Parse(parts[0]);
                int mn = int.Parse(parts[1]);
                int sec = int.Parse(parts[2]);
                return hr * 3600 + mn * 60 + sec;
            }
            return 0;
        }

        private static string ConvertFromSeconds(int totalSeconds)
        {
            int hr = totalSeconds / 3600;
            totalSeconds %= 3600;
            int mn = totalSeconds / 60;
            int sec = totalSeconds % 60;
            return $"{hr:D2}:{mn:D2}:{sec:D2}";
        }

        public static Dictionary<string, int> GetWeeklyReport()
        {
            var allData = new Dictionary<string, int>();
            string path = Environment.GetEnvironmentVariable("HOME") + "/.cache/hyprwatch/daily_data/";
            var files = Directory.GetFiles(path, "*.csv");

            DateTime today = DateTime.Today;
            int daysUntilMonday = (int)today.DayOfWeek - (int)DayOfWeek.Monday;
            if (daysUntilMonday < 0) daysUntilMonday += 7;
            DateTime startOfWeek = today.AddDays(-daysUntilMonday);

            foreach (var file in files)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                if (DateTime.TryParse(fileNameWithoutExtension, out DateTime fileDate))
                {
                    if (fileDate >= startOfWeek && fileDate < startOfWeek.AddDays(7))
                    {
                        var report = new Dictionary<string, string>();
                        var rawData = File.ReadAllLines(file);
                        foreach (var line in rawData)
                        {
                            var splitData = line.Split('\t');
                            if (splitData.Length >= 2)
                            {
                                report[splitData[1].Trim()] = splitData[0];
                            }
                        }

                        foreach (var kvp in report)
                        {
                            if (!allData.ContainsKey(kvp.Key))
                            {
                                allData[kvp.Key] = 0;
                            }
                            allData[kvp.Key] += ConvertToSeconds(FormatTime(kvp.Value));
                        }
                    }
                }
            }

            return allData;
        }

        public static void DisplayWeeklyReport()
        {
            string green = "\x1b[0;32m";
            string yellow = "\x1b[1;33m";
            string red = "\x1b[0;31m";
            string blue = "\x1b[0;34m";
            string reset = "\x1b[0m";

            var weeklyData = GetWeeklyReport();

            var sortedData = from entry in weeklyData orderby entry.Value descending select entry;

            int totalSeconds = weeklyData.Values.Sum();
            string totalTime = ConvertFromSeconds(totalSeconds);

            Console.WriteLine(" ");
            Console.WriteLine($"This Week's Screen Usage\t{totalTime}");
            Console.WriteLine($"{red}{new string('-', 60)}{reset}");
            Console.WriteLine($"{yellow}{"App".PadRight(30)}{"Time".PadLeft(30)}{reset}");
            Console.WriteLine($"{red}{new string('-', 60)}{reset}");

            foreach (var entry in sortedData)
            {
                string key = entry.Key.Length > 30 ? entry.Key.Substring(0, 27) + "..." : entry.Key;
                string time = ConvertFromSeconds(entry.Value);
                Console.WriteLine($"{blue}{key.PadRight(30)}{reset}{green}{time.PadLeft(30)}{reset}");
            }
        }
    }
}
