
using System.Text.RegularExpressions;

public class EditorConfig
{
    public static string PROJECT_PATH = Regex.Replace(System.IO.Path.GetFullPath("./../"), @"\\", "/");//项目根目录：HotFramework/
}
