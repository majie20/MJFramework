using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class UtilsEditor : EditorWindow
{
    [MenuItem("Tools/Excel导出(用于导出帧数表)")]
    public static void ExcelExportJson()
    {
        if (Directory.Exists("./Excel"))
        {
            foreach (var filePath in Directory.GetFiles("./Excel"))
            {
                ExportJson(filePath);
            }
        }
        else
        {
            Debug.Log("无");
        }
    }

    [MenuItem("Tools/ModelEditorPanel")]
    public static void OpenModelUtilsPanel()
    {
        CreateInstance<ModelEditor>().Show();
    }

    [MenuItem("Tools/PrefabAssociatePanel")]
    public static void OpenPrefabAssociatePanel()
    {
        CreateInstance<PrefabAssociateEditor>().Show();
    }

    private static List<string> firstRowCells;

    private static void ExportJson(string filePath)
    {
        if (Path.GetFileName(filePath).StartsWith("~"))
        {
            return;
        }

        var extension = Path.GetExtension(filePath);

        if (extension != ".xlsx" && extension != ".xls")
        {
            return;
        }

        var fileName = Path.GetFileNameWithoutExtension(filePath);
        IWorkbook wk = null;
        try
        {
            if (extension == ".xlsx")
            {
                wk = new XSSFWorkbook(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            }
            else if (extension == ".xls")
            {
                wk = new HSSFWorkbook(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            }

            var rootPath = "./Assets/StreamingAssets/TXT";
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
            else DelectDir(rootPath);

            for (int i = 0; i < wk.NumberOfSheets; i++)
            {
                ISheet sheet = wk.GetSheetAt(i);
                IRow firstRow = sheet.GetRow(0);
                if (firstRow == null)
                {
                    //第一行必须要写内容
                    Debug.Log($"{fileName}---{sheet.SheetName}的第一行什么都没有....");
                    continue;
                }
                //读取第一行每个单元的内容格式：(name&type)
                firstRowCells = new List<string>();
                foreach (var cell in firstRow.Cells)
                {
                    firstRowCells.Add(cell.ToString());
                }

                var type = "monster";

                if (sheet.SheetName[0] == '5')
                {
                    type = "boss";
                }

                var exportPath = Path.Combine(rootPath, $"{type}_{sheet.SheetName}.json");

                using (FileStream fs = File.Create(exportPath))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("[");
                        bool isResult;

                        for (int j = 1; j < sheet.LastRowNum + 1; j++)
                        {
                            IRow row = sheet.GetRow(j);
                            isResult = false;

                            if (row != null)
                            {
                                StringBuilder sb1 = new StringBuilder();
                                sb1.Append("{");

                                for (int k = 0; k < firstRowCells.Count; k++)
                                {
                                    sb1.Append($"\"{firstRowCells[k]}\":");
                                    var cell = row.GetCell(k);
                                    if (cell == null)
                                    {
                                        isResult = true;
                                        break;
                                    }
                                    sb1.Append($"\"{cell}\"");

                                    if (k != firstRowCells.Count - 1)
                                        sb1.Append(",");
                                }

                                if (isResult)
                                {
                                    if (j == sheet.LastRowNum)
                                        sb.Remove(sb.Length - 1, 1);
                                }
                                else
                                {
                                    if (j == sheet.LastRowNum)
                                        sb1.Append("}");
                                    else
                                        sb1.Append("},");

                                    sb.Append(sb1);
                                }
                            }
                        }
                        sb.Append("]");
                        sw.Write(sb);
                    }
                }
            }

            Debug.Log($"《{fileName}》导出完成....");
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log($"《{fileName}》导出失败....");
        }
    }

    private static void DelectDir(string srcPath)
    {
        DirectoryInfo dir = new DirectoryInfo(srcPath);
        FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
        foreach (FileSystemInfo i in fileinfo)
        {
            if (i is DirectoryInfo)            //判断是否文件夹
            {
                DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                subdir.Delete(true);          //删除子目录和文件
            }
            else
            {
                File.Delete(i.FullName);      //删除指定文件
            }
        }
    }
}