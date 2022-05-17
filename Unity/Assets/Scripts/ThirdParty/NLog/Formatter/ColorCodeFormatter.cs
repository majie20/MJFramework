// ANSI COLOR escape codes for colors and other things. 
// You can change the color of foreground and background plus bold, italic, underline etc
// 
// For a complete list see http://en.wikipedia.org/wiki/ANSI_escape_code#Colors

using System.Collections.Generic;

namespace NLog {
    public class ColorCodeFormatter {
        const string Reset = "0m";
        const string ESC = "\x1B[";

        // Foreground colors
        const string FG_Black   = "30m";
        const string FG_Red     = "31m";
        const string FG_Green   = "32m";
        const string FG_Yellow  = "33m";
        const string FG_Blue    = "34m";
        const string FG_Magenta = "35m";
        const string FG_Cyan    = "36m";
        const string FG_White   = "37m";

        // Background colors
        const string BG_None    = "";
        const string BG_Black   = "40m";
        const string BG_Red     = "41m";
        const string BG_Green   = "42m";
        const string BG_Yellow  = "43m";
        const string BG_Blue    = "44m";
        const string BG_Magenta = "45m";
        const string BG_Cyan    = "46m";
        const string BG_White   = "47m";

        // 0: background_color, 1: forground_color, 2: message
        const string Format = ESC + "{0}" + ESC + "{1}" + "{2}" + ESC + Reset;

        static readonly Dictionary<LogLevel, string[]> colors = new Dictionary<LogLevel, string[]> {
            { LogLevel.Trace, new [] { FG_White,  BG_Cyan } },
            { LogLevel.Debug, new [] { FG_Blue,   BG_None } },
            { LogLevel.Info,  new [] { FG_Green,  BG_None } },
            { LogLevel.Warn,  new [] { FG_Yellow, BG_None } },
            { LogLevel.Error, new [] { FG_White,  BG_Red } },
            { LogLevel.Fatal, new [] { FG_White,  BG_Magenta } }
        };

        public string FormatMessage(LogLevel logLevel, string message) {
            return string.Format(Format, colors[logLevel][1], colors[logLevel][0], message);
        }
    }
}

