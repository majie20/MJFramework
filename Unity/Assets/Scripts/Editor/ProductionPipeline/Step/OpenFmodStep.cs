//using Model;
//using System.Text;
//using System.Text.RegularExpressions;
//using UnityEditor;

//namespace M.ProductionPipeline
//{
//    public class OpenFmodStep : IStep
//    {
//        public void Run()
//        {
//            var audioManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/AudioManager.asset")[0];
//            var serializedManager = new SerializedObject(audioManager);
//            var prop = serializedManager.FindProperty("m_DisableAudio");
//            prop.boolValue = true;
//            serializedManager.ApplyModifiedProperties();

//            var str = Encoding.UTF8.GetString(FileHelper.LoadFileByStream(EditorConst.LINK_PATH));

//            if (!Regex.IsMatch(str, "<assembly fullname=.FMODUnity. preserve=.all./>"))
//            {
//                str = str.Replace("<!--<assembly fullname=\"FMODUnity\" preserve=\"all\"/>-->", "<assembly fullname=\"FMODUnity\" preserve=\"all\"/>");
//            }

//            if (!Regex.IsMatch(str, "<assembly fullname=.FMODUnityResonance. preserve=.all./>"))
//            {
//                str = str.Replace("<!--<assembly fullname=\"FMODUnityResonance\" preserve=\"all\"/>-->", "<assembly fullname=\"FMODUnityResonance\" preserve=\"all\"/>");
//            }

//            if (!Regex.IsMatch(str, "<!--<assembly fullname=.UnityEngine.AudioModule. preserve=.all./>-->"))
//            {
//                str = str.Replace("<assembly fullname=\"UnityEngine.AudioModule\" preserve=\"all\"/>", "<!--<assembly fullname=\"UnityEngine.AudioModule\" preserve=\"all\"/>-->");
//            }

//            FileHelper.SaveFileByStream(EditorConst.LINK_PATH, Encoding.UTF8.GetBytes(str));

//            if (System.IO.Directory.Exists(EditorConst.FMOD_PATH_))
//            {
//                UnityEditor.FileUtil.ReplaceDirectory(EditorConst.FMOD_PATH_, EditorConst.FMOD_PATH);
//                UnityEditor.FileUtil.DeleteFileOrDirectory(EditorConst.FMOD_PATH_);
//            }
//        }

//        public string EnterText()
//        {
//            return $"开启Fmod 开始！";
//        }

//        public string ExitText()
//        {
//            return $"开启Fmod 结束！";
//        }

//        public bool IsTriggerCompile()
//        {
//            return System.IO.Directory.Exists(EditorConst.FMOD_PATH_);
//        }
//    }
//}