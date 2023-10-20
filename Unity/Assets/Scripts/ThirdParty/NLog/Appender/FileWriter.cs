using System.IO;

namespace NLog {
    public class FileWriter {
        readonly object _lock = new object();
        readonly string _filePath;

        public FileWriter(string filePath) {
            _filePath = filePath;
        }

        public void WriteLine(string message) {
            lock (_lock) {
                using (StreamWriter writer = new StreamWriter(_filePath, true)) {
                    writer.WriteLine(message);
                }
            }
        }

        public void ClearFile() {
            lock (_lock) {
                using (StreamWriter writer = new StreamWriter(_filePath, false)) {
                    writer.Write(string.Empty);
                }
            }
        }
    }
}

