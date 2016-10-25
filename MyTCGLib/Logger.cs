using System;

namespace MyTCGLib
{
   public static class Logger
   {
      public static ILogWriter LogWriter { get; set; }

      public static void Log(string text)
      {
         if (LogWriter != null)
         {
            LogWriter.Log(text);
         }
      }
   }
}
