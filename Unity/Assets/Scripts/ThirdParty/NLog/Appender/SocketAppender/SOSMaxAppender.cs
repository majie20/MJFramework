using System;
using System.Text;
using System.Linq;

namespace NLog {
    public class SOSMaxAppender : SocketAppenderBase {
        protected override byte[] serializeMessage(LogLevel logLevel, string message) {
            var formattedMessage = formatLogMessage(logLevel.ToString(), message);
            var bytes = Encoding.UTF8.GetBytes(formattedMessage).ToList();
            bytes.Add(0);
            return bytes.ToArray();
        }

        string formatLogMessage(string logLevel, string message) {
            string[] lines = message.Split('\n');
            string commandType = lines.Length == 1 ? "showMessage" : "showFoldMessage";
            bool isMultiLine = lines.Length > 1;

            return string.Format("!SOS<{0} key=\"{1}\">{2}</{0}>",
                commandType,
                logLevel,
                isMultiLine ? multilineMessage(lines[0], message) : replaceXmlSymbols(message));
        }

        string multilineMessage(string title, string message) {
            return string.Format("<title>{0}</title><message>{1}</message>",
                replaceXmlSymbols(title),
                replaceXmlSymbols(message.Substring(message.IndexOf('\n') + 1)));
        }

        string replaceXmlSymbols(string str) {
            return str.Replace("<", "&lt;")
                        .Replace(">", "&gt;")
                        .Replace("&lt;", "<![CDATA[<]]>")
                        .Replace("&gt;", "<![CDATA[>]]>")
                        .Replace("&", "<![CDATA[&]]>");
        }
    }
}