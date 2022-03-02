//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace APlus
{
    public class QuickOpener : Editor
    {
		[MenuItem("Tools/A+ Assets Explorer/Quick Folder Opener/Application.dataPath", false, 100)]
		private static void OpenDataPath()
		{
			Reveal(Application.dataPath);
		}

		[MenuItem("Tools/A+ Assets Explorer/Quick Folder Opener/Application.persistentDataPath", false, 100)]
        private static void OpenPersistentDataPath()
        {
			Reveal(Application.persistentDataPath);
        }

		[MenuItem("Tools/A+ Assets Explorer/Quick Folder Opener/Application.streamingAssetsPath", false, 100)]
		private static void OpenStreamingAssets()
		{
			Reveal(Application.streamingAssetsPath);
		}

		[MenuItem("Tools/A+ Assets Explorer/Quick Folder Opener/Application.temporaryCachePath", false, 100)]
		private static void OpenCachePath()
		{
			Reveal(Application.temporaryCachePath);
		}

		// http://docs.unity3d.com/ScriptReference/MenuItem-ctor.html
		//
		[MenuItem("Tools/A+ Assets Explorer/Quick Folder Opener/Asset Store Packages Folder", false, 111)]
		private static void OpenAssetStorePackagesFolder()
		{
			//http://answers.unity3d.com/questions/45050/where-unity-store-saves-the-packages.html
			//
            #if UNITY_EDITOR_OSX
            string path = GetAssetStorePackagesPathOnMac();
			#elif UNITY_EDITOR_WIN
			string path = GetAssetStorePackagesPathOnWindows();
			#endif

			Reveal(path);
		}
		
		[MenuItem("Tools/A+ Assets Explorer/Quick Folder Opener/Editor Application Path")]
		private static void OpenUnityEditorPath()
		{
			Reveal(new FileInfo(EditorApplication.applicationPath).Directory.FullName);
		}
		
		[MenuItem("Tools/A+ Assets Explorer/Quick Folder Opener/Editor Log Folder")]
		private static void OpenEditorLogFolderPath()
		{
			#if UNITY_EDITOR_OSX
			string rootFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			var libraryPath = Path.Combine(rootFolderPath, "Library");
			var logsFolder = Path.Combine(libraryPath, "Logs"); 
			var UnityFolder = Path.Combine(logsFolder, "Unity");
			Reveal(UnityFolder);
			#elif UNITY_EDITOR_WIN
			var rootFolderPath = System.Environment.ExpandEnvironmentVariables("%localappdata%");
			var unityFolder = Path.Combine(rootFolderPath, "Unity");
			Reveal(Path.Combine(unityFolder, "Editor"));
			#endif
		}

		[MenuItem("Tools/A+ Assets Explorer/Quick Folder Opener/Asset Backup Folder", false, 122)]
		public static  void OpenAEBackupFolder()
		{
			var folder = Path.Combine(Application.persistentDataPath, "AEBackup");
			Directory.CreateDirectory(folder);
			Reveal(folder);
		}

		private const string ASSET_STORE_FOLDER_NAME = "Asset Store-5.x";
		private static string GetAssetStorePackagesPathOnMac()
		{
			var rootFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			var libraryPath = Path.Combine(rootFolderPath, "Library");
			var unityFolder = Path.Combine(libraryPath, "Unity");
			return Path.Combine(unityFolder, ASSET_STORE_FOLDER_NAME);
		}

		private static string GetAssetStorePackagesPathOnWindows()
		{
			var rootFolderPath = System.Environment.ExpandEnvironmentVariables("%appdata%");
			var unityFolder = Path.Combine(rootFolderPath, "Unity");
			return Path.Combine(unityFolder, ASSET_STORE_FOLDER_NAME);
		}

		public static void Reveal(string folderPath)
		{
			if(!Directory.Exists(folderPath))
			{
				Debug.LogWarning(string.Format("Folder '{0}' is not Exists", folderPath));
				return;
			}

			EditorUtility.RevealInFinder(folderPath);
		}
    }
}

#endif
