using System.IO;

namespace SNESFromScratch.SNESSystem
{
    public class Analyzer
    {
        private static int Counter;
        private static StreamWriter Writer = new StreamWriter(Counter + ".txt");

        static Analyzer()
        {
            Writer.AutoFlush = false;
        }

        public static void IncreaseFrame()
        {
            Counter += 1;
            Writer.Flush();
            Writer.Close();
            Writer.Dispose();
            Writer = new StreamWriter(Counter + ".txt");
            Writer.AutoFlush = false;
        }

        public static void WriteLine(string line)
        {
            Writer.WriteLine(line);
        }
    }
}