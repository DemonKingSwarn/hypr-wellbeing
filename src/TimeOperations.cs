namespace hyprwatch.Time
{
    using System;

    public class TimeOperations
    {
        public static string TimeDifference(string a, string b)
        {
            int hr = int.Parse(b.Substring(0, 2)) - int.Parse(a.Substring(0, 2));
            int mn = int.Parse(b.Substring(3, 2)) - int.Parse(a.Substring(3, 2));
            int sec = int.Parse(b.Substring(6, 2)) - int.Parse(a.Substring(6, 2));

            if (mn < 0 && sec < 0)
            {
                hr -= 1;
                mn += 60 - 1;
                sec += 60;
                if (hr < 0) hr += 24;
            }
            else if (mn < 0 && sec >= 0)
            {
                hr -= 1;
                mn += 60;
                if (hr < 0) hr += 24;
            }
            else if (sec < 0 && mn > 0)
            {
                sec += 60;
                mn -= 1;
                if (hr < 0) hr += 24;
            }
            else if (sec < 0 && mn == 0)
            {
                hr -= 1;
                mn = 59;
                sec += 60;
            }

            return $"{hr:D2}:{mn:D2}:{sec:D2}";
        }

        public static string TimeAddition(string a, string b)
        {
            int hr = int.Parse(b.Substring(0, 2)) + int.Parse(a.Substring(0, 2));
            int mn = int.Parse(b.Substring(3, 2)) + int.Parse(a.Substring(3, 2));
            int sec = int.Parse(b.Substring(6, 2)) + int.Parse(a.Substring(6, 2));

            if (mn >= 60 && sec >= 60)
            {
                hr += 1;
                mn = mn - 60 + 1;
                sec -= 60;
            }
            else if (mn >= 60)
            {
                hr += 1;
                mn -= 60;
            }
            else if (sec >= 60)
            {
                mn += 1;
                sec -= 60;
            }

            return $"{hr:D2}:{mn:D2}:{sec:D2}";
        }

        public static string FormatTime(string t)
        {
            return $"{t.Substring(0, 2)}h {t.Substring(3, 2)}m {t.Substring(6)}s";
        }

        public static int ConvertIntoSec(string t)
        {
            int hr = int.Parse(t.Substring(0, 2));
            int mn = int.Parse(t.Substring(3, 2));
            int sec = int.Parse(t.Substring(6));
            return hr * 3600 + mn * 60 + sec;
        }

        public static string Convert(int sec)
        {
            sec %= 24 * 3600;
            int hr = sec / 3600;
            sec %= 3600;
            int mn = sec / 60;
            sec %= 60;

            return $"{hr:D2}:{mn:D2}:{sec:D2}";
        }
    }
}

