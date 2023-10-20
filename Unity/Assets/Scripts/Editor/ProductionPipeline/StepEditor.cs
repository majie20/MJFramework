using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace M.ProductionPipeline
{
    [InitializeOnLoad]
    public class StepEditor
    {
        //[UnityEditor.Callbacks.DidReloadScripts()]
        //private static void OnScriptReload()
        //{
        //    EditorApplication.delayCall = Run;
        //}

        static StepEditor()
        {
            EditorApplication.delayCall = Run;
        }

        public static void RunStepGroup(List<IStep> list)
        {
            EditorHelper.ClearConsole();

            var stepSave = new StepSave();
            stepSave.Index = 0;
            stepSave.Steps = new List<List<FieldData>>(list.Count);
            stepSave.FullNames = new List<string>(list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                var type = list[i].GetType();
                var fullName = type.FullName;
                var dataList = new List<FieldData>();
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                for (int j = 0; j < fields.Length; j++)
                {
                    var t = fields[j].GetType();
                    var data = fields[j].GetValue(list[i]);

                    dataList.Add(new FieldData
                    {
                        Data = t == typeof(string) || t.IsPrimitive ? data.ToString() : CatJson.JsonParser.Default.ToJson(data),
                        Name = fields[j].Name,
                        TypeFullName = t.FullName,
                        AssemblyName = t.Assembly.FullName.Split(',')[0]
                    });
                }

                stepSave.Steps.Add(dataList);
                stepSave.FullNames.Add(fullName);
            }

            var settings = StepSaveSettings.Settings;
            settings.IsRunning = false;
            settings.StepSave = stepSave;
            EditorUtility.SetDirty(settings);
            ScriptCompileReloadTools.DisenableManualReloadDomain();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorApplication.delayCall = Run;
            UnityEngine.Debug.LogError("-----------------管线执行开始！-----------------");
        }

        private static void Run()
        {
            var settings = StepSaveSettings.Settings;

            if (settings.IsRunning)
            {
                return;
            }

            var stepSave = settings.StepSave;

            if (stepSave == null)
            {
                return;
            }

            settings.IsRunning = true;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var index = stepSave.Index;
            var count = stepSave.Steps.Count;

            if (index < count)
            {
                var assembly = typeof(StepEditor).Assembly;
                var fullName = stepSave.FullNames[index];
                var dataList = stepSave.Steps[index];
                var type = assembly.GetType(fullName);
                var o = Activator.CreateInstance(type) as IStep;

                for (int i = 0; i < dataList.Count; i++)
                {
                    var data = dataList[i];
                    var value = CatJson.JsonParser.Default.ParseJson(data.Data, System.Reflection.Assembly.Load(data.AssemblyName).GetType(data.TypeFullName));
                    var field = type.GetField(data.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    field?.SetValue(o, value);
                }

                if (!o.IsTriggerCompile())
                {
                    EditorApplication.delayCall = Run;
                }

                settings.StepSave.Index = ++index;
                UnityEngine.Debug.LogError(o.EnterText());
                o.Run();
                UnityEngine.Debug.LogError(o.ExitText());
            }
            else
            {
                settings.StepSave = null;
                UnityEngine.Debug.LogError("-----------------管线执行结束！-----------------");
            }

            settings.IsRunning = false;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}