using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipeHat_fountain
{
  public class SyncLog
  {
    private static string logFileName;
    private static StreamWriter logFile;
    static SyncLog()
    {
      LogFileName = "C:\\temp\\answer.txt";
    }

    public static string LogFileName
    {
      get => logFileName;
      set
      {
        logFileName = value;
        logFile = new StreamWriter(logFileName);
        logFile.AutoFlush = true;
      }
    }


    public static void Write(string msg)
    {
      lock(logFile)
      {
        var ds = DateTime.Now.ToString("HH:mm:ss.ffff");
        msg = msg.Replace("\x0b", "<VT>").Replace("\r", "<CR>").Replace("\n", "<LF>").Replace("\x1c", "<FS>");
        logFile.WriteLine($"{ds} {msg}");
      }
    }
    
  }
}
