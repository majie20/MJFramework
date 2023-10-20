using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace M.ProductionPipeline
{
    public class ExportAllAssetCollectorStep : IStep
    {
        public void Run()
        {
            List<string> pathList = new List<string>();
            EditorHelper.GetAssetPath(pathList, EditorConst.ASSET_COLLECTOR);
            EditorHelper.GetAssetPath(pathList, EditorConst.UNIT_PREFAB_PATH);
            EditorHelper.GetAssetPath(pathList, EditorConst.ENVIRONMENT_PATH);

            for (int i = pathList.Count - 1; i >= 0; i--)
            {
                if (Path.GetFileNameWithoutExtension(pathList[i]).EndsWith("_AC"))
                {
                    AssetCollectorEditor.ExportAssetCollector(AssetDatabase.LoadAssetAtPath<Object>(pathList[i]));
                }
            }
        }

        public string EnterText()
        {
            return $"导出所有资源收集器 开始！";
        }

        public string ExitText()
        {
            return $"导出所有资源收集器 结束！";
        }

        public bool IsTriggerCompile()
        {
            return false;
        }
    }
}