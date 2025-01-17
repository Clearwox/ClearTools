using System.IO;
using System.Threading.Tasks;

namespace Clear.Tools
{
    public static partial class Manager
    {
        public static void Write(string filename, string text)
        {
            using var sw = new StreamWriter(filename, append: false);
            sw.WriteLine(text);
        }

        public static string Read(string filename)
        {
            using var sr = new StreamReader(filename);
            return sr.ReadToEnd();
        }

        public static async Task WriteAsync(string filename, string text)
        {
            using var sw = new StreamWriter(filename, append: false);
            await sw.WriteLineAsync(text).ConfigureAwait(false);
        }

        public static async Task<string> ReadAsync(string filename)
        {
            using var sr = new StreamReader(filename);
            return await sr.ReadToEndAsync().ConfigureAwait(false);
        }
    }
}