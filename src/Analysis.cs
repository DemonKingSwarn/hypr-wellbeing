namespace hyprwatch.Report
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Globalization;
  using System.Collections.Generic;
  using hyprwatch.Time;

  public class Analysis
  {
    public static Dictionary<string, string> FinalReport(string date)
    {
      string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      string path = homeDir + "/.cache/hyprwatch/daily_data/";
      string filename = Path.Combine(path, date + ".csv");

      var report = new Dictionary<string, string>();

      if (File.Exists(filename))
      {
        var rawData = File.ReadAllLines(filename);
        foreach (var line in rawData)
        {
          var splitData = line.Split('\t');
          if (splitData.Length >= 2)
          {
            report[splitData[1].Trim()] = splitData[0];
          }
        }
      }

      report.Remove("Unknown");
      return report;
    }

    public static Dictionary<string, string> SortData(Dictionary<string, string> report)
    {
      var sortedValues = report.Select(kvp => TimeOperations.ConvertIntoSec(kvp.Value)).OrderByDescending(sec => sec).ToList();
      var sortedReport = new Dictionary<string, string>();

      foreach (var seconds in sortedValues)
      {
        foreach (var kvp in report)
        {
          if (TimeOperations.ConvertIntoSec(kvp.Value) == seconds && !sortedReport.ContainsKey(kvp.Key))
          {
            sortedReport[kvp.Key] = kvp.Value;
            break;
          }
        }
      }

      return sortedReport;
    }

    public static DateTime GetSundayOfWeek(string week)
    {
      int year = int.Parse(week.Substring(4));
      int weekNumber = int.Parse(week.Substring(1, 2));
      var firstDayOfYear = new DateTime(year, 1, 1);
      var baseDay = firstDayOfYear.DayOfWeek == DayOfWeek.Monday ? firstDayOfYear : firstDayOfYear.AddDays(8 - (int)firstDayOfYear.DayOfWeek);
      return baseDay.AddDays((weekNumber - 1) * 7 + 6);
    }

    public static List<string> WeekDates(DateTime theDay)
    {
      int weekday = (int)theDay.DayOfWeek - 1;
      var start = theDay.AddDays(-weekday);
      return Enumerable.Range(0, weekday + 1).Select(offset => start.AddDays(offset).ToString("yyyy-MM-dd")).ToList();
    }

    public static string WeekdayFromDate(string date)
    {
      if (DateTime.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
      {
        return parsedDate.ToString("ddd");
      }

      return string.Empty;
    }

    public static void WeeklyLogs(string week)
    {
      string user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      string filename = Path.Combine(user, ".cache/Watcher/Analysis/", week + ".csv");

      using (var writer = new StreamWriter(filename))
      {
        List<string> dates;
        if (GetCurrentWeek() == week)
        {
          dates = WeekDates(DateTime.Today);
        }
        else
        {
          dates = WeekDates(GetSundayOfWeek(week));
        }

        foreach (var date in dates)
        {
          string totalScreenTime = "00:00:00";
          foreach (var kvp in FinalReport(date))
          {
            totalScreenTime = TimeOperations.TimeAddition(totalScreenTime, kvp.Value);
          }
          writer.WriteLine($"{WeekdayFromDate(date)}\t{totalScreenTime}");
        }

        var allData = new Dictionary<string, string>();
        foreach (var date in dates)
        {
          foreach (var kvp in FinalReport(date))
          {
            if (!allData.ContainsKey(kvp.Key))
            {
              allData[kvp.Key] = "00:00:00";
            }
            allData[kvp.Key] = TimeOperations.TimeAddition(allData[kvp.Key], kvp.Value);
          }
        }

        allData = SortData(allData);
        foreach (var kvp in allData)
        {
          writer.WriteLine($"{kvp.Value}\t{kvp.Key}");
        }
      }
    }

    public static string GetCurrentWeek()
    {
      var currentWeek = DateTime.Now.ToString("\'W\'\'\'W" + CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
      return currentWeek;
    }
  }
}
