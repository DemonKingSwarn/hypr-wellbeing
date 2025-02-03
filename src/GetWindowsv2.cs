namespace hyprwatch.Window
{
  using System;
  using System.Net.Sockets;
  using System.Text;
  using System.IO;
  using System.Text.RegularExpressions;

  public partial class GetWindowsv2
  {
    public static string ActiveWindow()
    {
      string xdgRuntimeDir = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR"); 
      string hyprlandInstanceSig = Environment.GetEnvironmentVariable("HYPRLAND_INSTANCE_SIGNATURE");

      string socketPath = Path.Combine(xdgRuntimeDir, "hypr", hyprlandInstanceSig, ".socket2.sock");

      string? activeWindow = null;

      var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);

      try 
      {
        socket.Connect(new UnixDomainSocketEndPoint(socketPath));

        using (var stream = new NetworkStream(socket))
        using (var reader = new StreamReader(stream))
        {
            string line = reader.ReadLine();

            if (line != null)
            {

              var classMatch = ClassRegex().Match(line);
              if(classMatch.Success)
              {
                activeWindow = classMatch.Groups[1].Value.Trim();
              }
            }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      finally 
      {
        socket.Close();
      }

      return activeWindow ?? "Home-Screen";
    }

    [GeneratedRegex(@"activewindow>>([^,]+)")]
    public static partial Regex ClassRegex();
  }
}
