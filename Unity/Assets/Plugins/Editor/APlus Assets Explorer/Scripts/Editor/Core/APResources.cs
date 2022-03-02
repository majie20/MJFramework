//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

namespace APlus
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using System.IO;
    using System.Text.RegularExpressions;

#if UNITY_5_5_OR_NEWER
    using UnityEngine.Profiling;
#endif

#if UNITY_5_6_OR_NEWER
    using UnityEngine.Video;
    using MovieTextureType =  UnityEngine.Video.VideoClip;
#else
    using MovieTextureType =  UnityEngine.MovieTexture;
#endif

    public class APResources
    {
        public static string[] GetAssetGuidsByType(APAssetType type)
        {
            return AssetDatabase.FindAssets(string.Format("t:{0}", type.ToString()));
        }

        public static APAsset GetAPAssetByPath(APAssetType type, string assetid)
        {
            string guid = Utility.GetGuidFromAssetId(assetid);
            string fileId = Utility.GetFileIdFromAssetId(assetid);

            switch (type)
            {
                case APAssetType.Font:
                    return GetAPFontFromAssetGuid(guid);
                case APAssetType.MovieTexture:
                    return GetAPMovieTextureFromAssetGuid(guid);
                case APAssetType.Texture:
                    return GetAPTextureFromAssetGuid(guid);
                case APAssetType.Model:
                    return GetAPModelFromAssetGuid(guid);
                case APAssetType.AudioClip:
                    return GetAPAudioFromAssetGuid(guid);
                case APAssetType.Material:
                    return GetAPMaterialFromAssetGuid(guid);
                case APAssetType.Shader:
                    return GetAPShaderFromAssetGuid(guid);
                case APAssetType.AnimationClip:
                    return GetAPAnimationFromAssetPath(guid, fileId);
                case APAssetType.Prefab:
                    return GetAPPrefabFromAssetGuid(guid);
                case APAssetType.StreamingAssets:
                    return GetStreamingAssetFile(AssetDatabase.GUIDToAssetPath(assetid));
                case APAssetType.Script:
                    return GetCodeFile(AssetDatabase.GUIDToAssetPath(assetid));
                case APAssetType.Others:
                    return GetOtherFile(AssetDatabase.GUIDToAssetPath(assetid));
                default:
                    return null;
            }
        }

        public static List<T> GetResourcesListByType<T>(APAssetType type, Func<string, T> parseFunction) where T : class
        {
            var textGuids = GetAssetGuidsByType(type);
            List<T> list = new List<T>();
            for (int i = 0; i < textGuids.Length; i++)
            {
                T obj = parseFunction(textGuids[i]);
                if (obj != null)
                {
                    list.Add(obj);
                }
            }

            return list;
        }

        public static List<APFont> GetFonts()
        {
            return GetResourcesListByType<APFont>(APAssetType.Font, GetAPFontFromAssetGuid);
        }

        public static List<APTexture> GetTextures()
        {
            return GetResourcesListByType<APTexture>(APAssetType.Texture, GetAPTextureFromAssetGuid);
        }

        public static List<APModel> GetModels()
        {
            return GetResourcesListByType<APModel>(APAssetType.Model, GetAPModelFromAssetGuid);
        }

        public static List<APShader> GetShaders()
        {
            return GetResourcesListByType<APShader>(APAssetType.Shader, GetAPShaderFromAssetGuid);
        }

        public static List<APMaterial> GetMaterials()
        {
            List<APMaterial> materials = new List<APMaterial>();
            materials.AddRange(GetResourcesListByType<APMaterial>(APAssetType.Material, GetAPMaterialFromAssetGuid));
            materials.AddRange(GetResourcesListByType<APMaterial>(APAssetType.PhysicMaterial, GetAPMaterialFromAssetGuid));
            materials.AddRange(GetResourcesListByType<APMaterial>(APAssetType.PhysicsMaterial2D, GetAPMaterialFromAssetGuid));
            return materials;
        }

        public static List<APAudio> GetAudios()
        {
            return GetResourcesListByType<APAudio>(APAssetType.AudioClip, GetAPAudioFromAssetGuid);
        }

        public static List<APMovieTexture> GetMovies()
        {
            var list = GetResourcesListByType<APMovieTexture>(APAssetType.MovieTexture, GetAPMovieTextureFromAssetGuid);
#if UNITY_5_6_OR_NEWER
            list.AddRange(GetResourcesListByType<APMovieTexture>(APAssetType.VideoClip, GetAPMovieTextureFromAssetGuid));
#endif
            return list;
        }

        public static List<APPrefab> GetPrefabs()
        {
            return GetResourcesListByType<APPrefab>(APAssetType.Prefab, GetAPPrefabFromAssetGuid);
        }

        public static List<APStreamingAssetsFile> GetStreamingAssets()
        {
            var files = Utility.GetFilesInRelativePath(Application.streamingAssetsPath);
            List<APStreamingAssetsFile> apfiles = new List<APStreamingAssetsFile>();

            if (files == null)
            {
                return apfiles;
            }

            foreach (var item in files)
            {
                string folder = System.IO.Path.Combine("Assets", "StreamingAssets");
                string itemPath = folder + item;
                apfiles.Add(GetStreamingAssetFile(itemPath));
            }

            return apfiles;
        }

        public static List<APCodeFile> GetCodeFiles()
        {
            string[] filesExtensions = PreferencesItems.CodeFileType.ToLower().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<APCodeFile> codeFiles = new List<APCodeFile>();
            foreach (var extension in filesExtensions)
            {
                var files = Utility.GetFilesInRelativePath(Application.dataPath, extension);
                if (files == null)
                {
                    continue;
                }

                foreach (var file in files)
                {
                    string filePath = "Assets" + file;
                    codeFiles.Add(GetCodeFile(filePath));
                }
            }

            return codeFiles;
        }

        public static Dictionary<string, string> GetMonoScriptsMap()
        {
            var guids = GetAssetGuidsByType(APAssetType.Script);
            Dictionary<string, string> map = new Dictionary<string, string>();

            foreach (var item in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(item);
                MonoScript ms = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)) as MonoScript;
                if (ms != null && !map.ContainsKey(ms.name))
                {
                    var cls = ms.GetClass();
                    if (cls != null)
                    {
                        map.Add(ms.name, path);
                    }
                }
            }

            return map;
        }

        public static List<APOtherFile> GetOthers()
        {
            List<APOtherFile> files = new List<APOtherFile>();
            var allfiles = Utility.GetFilesInRelativePath(Application.dataPath);

            foreach (var file in allfiles)
            {
                string filePath = "Assets" + file;
                var guid = AssetDatabase.AssetPathToGUID(filePath);
                if (APCache.HasAsset(guid))
                {
                    continue;
                }

                files.Add(GetOtherFile(filePath));
            }

            return files;
        }

        public static APOtherFile GetOtherFile(string path)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);

            APOtherFile otherFile = new APOtherFile();
            otherFile.Path = path;
            otherFile.Hash = Utility.GetFileMd5(path);
            otherFile.Name = Utility.GetFileName(path);
            otherFile.FileSize = Utility.GetFileSize(path);
            otherFile.Icon = GetIconID(path);
            otherFile.FileType = Utility.GetFileExtension(path);
            otherFile.Id = guid;

            return otherFile;
        }

        public static APCodeFile GetCodeFile(string path)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);

            APCodeFile codeFile = new APCodeFile();
            codeFile.Path = path;
            codeFile.Hash = Utility.GetFileMd5(path);
            codeFile.Name = Utility.GetFileName(path);
            codeFile.FileSize = Utility.GetFileSize(path);
            codeFile.FileType = Utility.GetFileExtension(path);
            codeFile.Icon = GetIconID(path);
            codeFile.Id = guid;

            if (codeFile.FileType.Equals(".cs", StringComparison.OrdinalIgnoreCase))
            {
                var tags = GetTagsInCode(File.ReadAllText(codeFile.Path)).ToArray();
                codeFile.ContainTags = string.Join(",", tags);

                if (!string.IsNullOrEmpty(codeFile.ContainTags))
                {
                    Utility.DebugLog("Tags In CodeFile: " + codeFile.ContainTags);
                }
            }

            return codeFile;
        }

        public static APStreamingAssetsFile GetStreamingAssetFile(string path)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);

            APStreamingAssetsFile file = new APStreamingAssetsFile();
            file.Path = path;
            file.Hash = Utility.GetFileMd5(path);
            file.Name = Utility.GetFileName(path);
            file.FileSize = Utility.GetFileSize(path);
            file.Icon = GetIconID(path);
            file.APType = AssetType.STREAMING_ASSETS;
            file.Id = guid;

            return file;
        }

        public static APPrefab GetAPPrefabFromAssetGuid(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            APPrefab prefab = new APPrefab();
            prefab.Path = path;
            prefab.Hash = Utility.GetFileMd5(path);
            prefab.Icon = GetIconID(path);
            prefab.FileSize = Utility.GetFileSize(path);
            prefab.Name = Utility.GetFileName(path);
            prefab.Id = guid;

            HashSet<string> Tags = new HashSet<string>();
            HashSet<string> Layers = new HashSet<string>();

            var objects = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var obj in objects)
            {
                var go = obj as GameObject;
                if (go == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(go.tag)
                    && !go.tag.Equals("untagged", StringComparison.OrdinalIgnoreCase))
                {
                    Tags.Add(go.tag);
                }

                var layerName = LayerMask.LayerToName(go.layer);
                if (!string.IsNullOrEmpty(layerName) &&
                    !layerName.Equals("default", StringComparison.OrdinalIgnoreCase))
                {
                    Layers.Add(layerName);
                }
            }

            prefab.InLayers = string.Join(",", Layers.ToArray());
            prefab.ContainTags = string.Join(",", Tags.ToArray());

            objects = null;
            return prefab;
        }

        public static bool IsPreviewAnimation(AnimationClip clip)
        {
            return clip.name.IndexOf("__preview__") != -1;
        }

        public static List<string> GetAnimationClipAssetIdInModel(string modelAssetPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(modelAssetPath);

            List<string> ids = new List<string>();
            foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(modelAssetPath))
            {
                if (obj is AnimationClip)
                {
                    if (IsPreviewAnimation(obj as AnimationClip))
                    {
                        UnloadAsset(obj);
                        continue;
                    }

                    ids.Add(Utility.GetAssetId(guid, Utility.GetLocalIndentifierOfObject(obj).ToString()));
                }

                UnloadAsset(obj);
            }

            return ids;
        }

        public static List<APAnimation> GetAnimations()
        {
            string[] guids = GetAssetGuidsByType(APAssetType.AnimationClip);

            HashSet<string> uniqueGuids = new HashSet<string>();
            foreach (var guid in guids)
            {
                uniqueGuids.Add(guid);
            }

            List<APAnimation> list = new List<APAnimation>();
            foreach (var guid in uniqueGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Utility.IsUntyNewAnimation(path))
                {
                    AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                    APAnimation apClip = GetAPAnimationFromClip(clip);
                    apClip.Path = path;
                    apClip.Hash = Utility.GetFileMd5(path);
                    apClip.FileSize = Utility.GetFileSize(path);
                    apClip.InFile = Utility.GetFileName(path);
                    apClip.Id = guid;

                    UnloadAsset(clip);
                    list.Add(apClip);
                }
                else
                {
                    foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(path))
                    {
                        if (obj is AnimationClip)
                        {
                            if (IsPreviewAnimation(obj as AnimationClip))
                            {
                                UnloadAsset(obj);
                                continue;
                            }

                            APAnimation apClip = GetAPAnimationFromClip(obj as AnimationClip);
                            apClip.Path = path;
                            apClip.Hash = Utility.GetFileMd5(path);
                            apClip.InFile = Utility.GetFileName(path);
                            apClip.FileSize = 0;
                            apClip.Id = Utility.GetAssetId(guid, Utility.GetLocalIndentifierOfObject(obj).ToString());

                            UnloadAsset(obj);
                            list.Add(apClip);
                        }
                    }
                }
            }

            return list;
        }

        public static T GetAssetImporterFromAssetGuid<T>(string guid) where T : class
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return AssetImporter.GetAtPath(path) as T;
        }

        private static bool InUseOrSelection(UnityEngine.Object obj)
        {
            // In selection
            //
            foreach (var selected in Selection.objects)
            {
                if (selected == obj)
                {
                    return true;
                }
            }

            return false;
        }

        public static void UnloadAsset(UnityEngine.Object obj)
        {
            if (InUseOrSelection(obj) || obj is GameObject || obj is Component || obj is AssetBundle)
            {
                obj = null;
                return;
            }

            Resources.UnloadAsset(obj);
            obj = null;
        }

        public static APAnimation GetAPAnimationFromAssetPath(string guid, string fileId)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            if (Utility.IsUntyNewAnimation(path))
            {
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                APAnimation apClip = GetAPAnimationFromClip(clip);
                apClip.Path = path;
                apClip.Hash = Utility.GetFileMd5(path);
                apClip.InFile = Utility.GetFileName(path);
                apClip.Id = guid;

                UnloadAsset(clip);
                return apClip;
            }
            else
            {
                if (string.IsNullOrEmpty(fileId))
                {
                    return null;
                }


                var objects = AssetDatabase.LoadAllAssetsAtPath(path);

                foreach (var obj in objects)
                {
                    if ((obj is AnimationClip) && Utility.GetLocalIndentifierOfObject(obj).ToString() == fileId)
                    {
                        APAnimation apClip = GetAPAnimationFromClip(obj as AnimationClip);
                        apClip.Path = path;
                        apClip.Hash = Utility.GetFileMd5(path);
                        apClip.InFile = Utility.GetFileName(path);
                        apClip.Id = Utility.GetAssetId(guid, fileId);

                        foreach (var item in objects)
                        {
                            UnloadAsset(item);
                        }

                        return apClip;
                    }
                }


                foreach (var item in objects)
                {
                    UnloadAsset(item);
                }
            }

            return null;
        }

        public static APAnimation GetAPAnimationFromClip(AnimationClip clip)
        {
            APAnimation animation = new APAnimation();
            AnimationClipSettings setting = AnimationUtility.GetAnimationClipSettings(clip);
            animation.Name = clip.name;
            animation.CycleOffset = setting.cycleOffset;
            animation.LoopPose = setting.loopBlend;
            animation.LoopTime = setting.loopTime;
            animation.FPS = Mathf.RoundToInt(clip.frameRate);
            animation.Length = clip.length;
            animation.Icon = GetIconID("", true);

            return animation;
        }

        public static APMaterial GetAPMaterialFromAssetGuid(string guid)
        {
            APMaterial material = new APMaterial();
            string path = AssetDatabase.GUIDToAssetPath(guid);
            material.Icon = GetIconID(path);

            if (Utility.IsMaterial(path))
            {
                if (path.EndsWith(".mat"))
                {
                    material.Type = MaterialType.Material;
                }
                else
                {
                    if (path.EndsWith(".physicmaterial"))
                    {
                        material.Type = MaterialType.PhysicMaterial;
                    }
                    else
                    {
                        material.Type = MaterialType.PhysicsMaterial2D;
                    }
                }

                material.Path = path;
                material.Hash = Utility.GetFileMd5(path);
                material.FileSize = Utility.GetFileSize(path);
                material.Name = Utility.GetFileName(path);

                Material m = AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;
                if (m != null && m.shader != null)
                {
                    material.Shader = m.shader.name;
                }
                else
                {
                    material.Shader = string.Empty;
                }

                material.Id = guid;
                UnloadAsset(m);
                return material;
            }

            return null;
        }

        public static APFont GetAPFontFromAssetGuid(string guid)
        {
            TrueTypeFontImporter importer = GetAssetImporterFromAssetGuid<TrueTypeFontImporter>(guid);

            if (importer == null)
            {
                return null;
            }
            string path = AssetDatabase.GUIDToAssetPath(guid);

            APFont apFont = new APFont();
            apFont.Icon = GetIconID(path);
            apFont.Name = Utility.GetFileName(path);
            apFont.Character = importer.fontTextureCase;
            apFont.RenderingMode = importer.fontRenderingMode;
            apFont.Path = path;
            apFont.Hash = Utility.GetFileMd5(path);
            apFont.FileSize = Utility.GetFileSize(path);
            apFont.FontNames = importer.fontTTFName;
            apFont.Id = guid;
            return apFont;
        }

        public static APAudio GetAPAudioFromAssetGuid(string guid)
        {
            AudioImporter audioImporter = GetAssetImporterFromAssetGuid<AudioImporter>(guid);
            if (audioImporter == null)
            {
                return null;
            }

            APAudio audio = new APAudio();
            string path = AssetDatabase.GUIDToAssetPath(guid);
            audio.Icon = GetIconID(path);
            AudioClip audioClip = AssetDatabase.LoadAssetAtPath(path, typeof(AudioClip)) as AudioClip;

            if (audioClip == null)
            {
                return null;
            }

            audio.Name = Utility.GetFileName(path);
            audio.FileSize = Utility.GetFileSize(path);
            audio.Background = audioImporter.loadInBackground;

            string platform = EditorUserBuildSettings.activeBuildTarget.ToString();
            AudioImporterSampleSettings settings = audioImporter.defaultSampleSettings;

            if (audioImporter.ContainsSampleSettingsOverride(platform))
            {
                settings = audioImporter.GetOverrideSampleSettings(EditorUserBuildSettings.activeBuildTarget.ToString());
            }

            Type t = typeof(AudioImporter);
            var comSizeProperty = t.GetProperty("compSize", BindingFlags.NonPublic | BindingFlags.Instance);
            if (comSizeProperty != null)
            {
                audio.ImportedSize = long.Parse(comSizeProperty.GetValue(audioImporter, null).ToString());
            }

            audio.Path = path;
            audio.Hash = Utility.GetFileMd5(path);
            audio.Ratio = audio.ImportedSize * 100f / audio.FileSize;
            audio.Quality = settings.quality;
            audio.Compress = settings.compressionFormat;
            audio.Duration = Utility.GetTimeDurationString(TimeSpan.FromSeconds(audioClip.length));
            audio.Frequency = AudioUtil.GetFrequency(audioClip);
            audio.Id = guid;

            UnloadAsset(audioClip);
            return audio;
        }

        public static APShader GetAPShaderFromAssetGuid(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Shader shader = AssetDatabase.LoadAssetAtPath(path, typeof(Shader)) as Shader;
            if (shader == null)
            {
                return null;
            }

            APShader apShader = new APShader();
            apShader.Icon = GetIconID(path);
            apShader.Path = path;
            apShader.Hash = Utility.GetFileMd5(path);
            apShader.FileSize = Utility.GetFileSize(path);
            apShader.FileName = Utility.GetFileName(path);
            apShader.CastShadows = ShaderUtils.HasShadowCasterPass(shader);
            apShader.DisableBatching = ShaderUtils.GetDisableBatching(shader);
            apShader.SurfaceShader = ShaderUtils.HasSurfaceShaders(shader);
            apShader.IgnoreProjector = ShaderUtils.DoesIgnoreProjector(shader);
            apShader.LOD = ShaderUtils.GetLOD(shader);
            apShader.RenderQueue = ShaderUtils.GetRenderQueue(shader);
            apShader.RenderQueueText = Utility.GetShaderRenderQueueText(apShader.RenderQueue);
            apShader.VariantsIncluded = ShaderUtils.GetVariantsCount(shader, true);
            apShader.Name = shader.name;
            apShader.VariantsTotal = ShaderUtils.GetVariantsCount(shader, false);
            apShader.Id = guid;

            UnloadAsset(shader);
            return apShader;
        }

        public static APModel GetAPModelFromAssetGuid(string guid)
        {
            ModelImporter modelImpoter = GetAssetImporterFromAssetGuid<ModelImporter>(guid);
            if (modelImpoter == null)
            {
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guid);
            APModel model = new APModel();
            model.Icon = GetIconID(path);
            model.Name = Utility.GetFileName(path);
            model.FileSize = Utility.GetFileSize(path);
            model.MeshCompression = modelImpoter.meshCompression;
            model.OptimizeMesh = modelImpoter.optimizeMesh;
            model.ScaleFactor = modelImpoter.globalScale;
            model.ReadWrite = modelImpoter.isReadable;
            model.ImportBlendShapes = modelImpoter.importBlendShapes;
            model.GenerateColliders = modelImpoter.addCollider;
            model.SwapUVs = modelImpoter.swapUVChannels;
            model.LightmapToUV2 = modelImpoter.generateSecondaryUV;
            model.Path = path;
            model.Hash = Utility.GetFileMd5(path);
            model.Id = guid;

            var modelObject = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var obj in modelObject)
            {
                if (obj is MeshFilter)
                {
                    Mesh mesh = (obj as MeshFilter).sharedMesh;
                    model.Tris += mesh.triangles.Length / 3;
                    model.Vertexes += mesh.vertexCount;
                }

                if (obj is SkinnedMeshRenderer)
                {
                    Mesh mesh = (obj as SkinnedMeshRenderer).sharedMesh;
                    model.Tris += mesh.triangles.Length / 3;
                    model.Vertexes += mesh.vertexCount;
                }

                UnloadAsset(obj);
            }

            return model;
        }

        public static APMovieTexture GetAPMovieTextureFromAssetGuid(string guid)
        {

#if UNITY_5_6_OR_NEWER
            VideoClipImporter movieImporter = GetAssetImporterFromAssetGuid<VideoClipImporter>(guid);
#else
            MovieImporter movieImporter = GetAssetImporterFromAssetGuid<MovieImporter>(guid);
#endif
            Debug.Log(movieImporter);
            // if texture is render texture or others, tImporter will to set to null.
            //
            if (movieImporter == null)
            {
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

#if UNITY_5_6_OR_NEWER
            var texture = AssetDatabase.LoadAssetAtPath(path, typeof(VideoClip)) as VideoClip;
#else
            var texture = AssetDatabase.LoadAssetAtPath(path, typeof(MovieTexture)) as MovieTexture;
#endif
            Debug.Log(texture);
            if (texture == null)
            {
                return null;
            }

            APMovieTexture apMovieTexture = new APMovieTexture();
            apMovieTexture.Icon = GetIconID(path);
#if UNITY_5_6_OR_NEWER
            apMovieTexture.Size = (long)movieImporter.outputFileSize;
            double approx = 0;
#else
            apMovieTexture.Size = TextureUtillity.GetStorageMemorySize(texture);
            double approx = (GetVideoBitrateForQuality(movieImporter.quality) + GetAudioBitrateForQuality(movieImporter.quality)) * movieImporter.duration / 8;
#endif
            apMovieTexture.Approx = approx;

            apMovieTexture.Name = Utility.GetFileName(path);
            apMovieTexture.Path = path;
            apMovieTexture.Hash = Utility.GetFileMd5(path);
            apMovieTexture.Quality = movieImporter.quality;
            apMovieTexture.FileSize = Utility.GetFileSize(path);
#if UNITY_5_6_OR_NEWER
            TimeSpan duration = TimeSpan.FromSeconds(texture.length);
#else
            TimeSpan duration = TimeSpan.FromSeconds(texture.duration);
#endif
            apMovieTexture.Duration = Utility.GetTimeDurationString(duration);
            apMovieTexture.Id = guid;

            UnloadAsset(texture);
            return apMovieTexture;
        }

        private static double GetAudioBitrateForQuality(double f)
        {
            return 56000.0 + 200000.0 * f;
        }

        private static double GetVideoBitrateForQuality(double f)
        {
            return 100000.0 + 8000000.0 * f;
        }

        public static APTexture GetAPTextureFromAssetGuid(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var texture = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

#if UNITY_5_6_OR_NEWER
            if (texture == null || texture is VideoClip)
#else
            if (texture == null || texture is MovieTexture)
#endif
            {
                return null;
            }

            APTexture apTexture = new APTexture();
            apTexture.Icon = GetIconID(path);
            if (texture is RenderTexture)
            {
                var renderTexture = texture as RenderTexture;
                apTexture.StorageSize = TextureUtillity.GetStorageMemorySize(renderTexture);
#if UNITY_5 && !UNITY_5_6
                apTexture.RuntimeSize = Profiler.GetRuntimeMemorySize(renderTexture);
#else
                apTexture.RuntimeSize = Profiler.GetRuntimeMemorySizeLong(renderTexture);
#endif
                apTexture.Width = renderTexture.width;
                apTexture.Height = renderTexture.height;
                apTexture.TextureType = "Render";
                apTexture.Path = path;
                apTexture.Hash = Utility.GetFileMd5(path);
                apTexture.Name = Utility.GetFileName(path);
                apTexture.FileSize = Utility.GetFileSize(path);
                apTexture.Id = guid;
                apTexture.Compress = "None";
                UnloadAsset(texture);
                return apTexture;
            }

            TextureImporter tImporter = GetAssetImporterFromAssetGuid<TextureImporter>(guid);
            if (tImporter == null)
            {
                return null;
            }

            var tex = texture as Texture;

#if UNITY_5_5_OR_NEWER
            TextureImporterCompression importerCompression = tImporter.textureCompression;
#else
            TextureImporterFormat importerFormat = tImporter.textureFormat;
#endif

#if UNITY_5_5_OR_NEWER
            var platformSettings = tImporter.GetPlatformTextureSettings(Utility.BuildTargetToPlatform(EditorUserBuildSettings.activeBuildTarget));
            apTexture.CompressionQuality = platformSettings.compressionQuality;
            apTexture.CrunchedCompression = platformSettings.crunchedCompression;
            apTexture.MaxSize = platformSettings.maxTextureSize;
            apTexture.Compress = GetCommonNameOfCompression(platformSettings.textureCompression.ToString());
#else
            int maxTextureSize = tImporter.maxTextureSize;
            int compressQuality = tImporter.compressionQuality;
            tImporter.GetPlatformTextureSettings(Utility.BuildTargetToPlatform(EditorUserBuildSettings.activeBuildTarget), out maxTextureSize, out importerFormat, out compressQuality);
            apTexture.MaxSize = maxTextureSize;
            apTexture.CompressionQuality = compressQuality;
            apTexture.CrunchedCompression = false;
            apTexture.Compress = GetCommonNameOfCompression(importerFormat.ToString());
#endif

            // Get texture settings for different platform
            //
            apTexture.StorageSize = TextureUtillity.GetStorageMemorySize(tex);
            apTexture.RuntimeSize = TextureUtillity.GetRuntimeMemorySize(tex);
            apTexture.Name = Utility.GetFileName(path);
            apTexture.ReadWrite = tImporter.isReadable;

#if UNITY_5_5_OR_NEWER
            if ((int)platformSettings.format > 0)
            {
                apTexture.TextureFormat = platformSettings.format.ToString();
            }
            else
            {
                apTexture.TextureFormat = "Auto";
            }
#else
            apTexture.TextureFormat = importerFormat.ToString();
#endif
            apTexture.TextureType = tImporter.textureType.ToString();
            apTexture.Path = path;
            apTexture.Hash = Utility.GetFileMd5(path);
            apTexture.MipMap = tImporter.mipmapEnabled;
            apTexture.Width = tex.width;
            apTexture.Height = tex.height;
            apTexture.FileSize = Utility.GetFileSize(path);
            int widthInPixel = 0;
            int heightInPixel = 0;
            TextureUtillity.GetImageSize(texture as Texture2D, out widthInPixel, out heightInPixel);

            apTexture.WidthInPixel = widthInPixel;
            apTexture.HeightInPixel = heightInPixel;

            if (tImporter.textureType == TextureImporterType.Sprite)
            {
                apTexture.PackingTag = tImporter.spritePackingTag;
            }

            apTexture.Id = guid;

            UnloadAsset(texture);

            return apTexture;
        }

        public static string GetCommonNameOfCompression(string compress)
        {
            switch (compress.ToLower())
            {
                case "compressed":
                    return "Normal";
                case "compressedlq":
                    return "Low";
                case "compressedhq":
                    return "High";
                case "uncompressed":
                    return "None";
                default:
                    return compress;
            }
        }

        public static string GetIconID(string assetPath, bool isAnimation = false)
        {
            if (isAnimation)
            {
                var iconData1 = "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";
                var key1 = Utility.MD5(iconData1);
                APCache.AddIcon(key1, iconData1);
                return key1;
            }
            try
            {
                var texture = AssetDatabase.GetCachedIcon(assetPath);

                if (texture == null)
                {
                    return GetNullKey();
                }

                var format = (texture as Texture2D).format;
                
                if (!IsSupportedFormat(format))
                {
                    return GetNullKey();
                }

                var newTexture = new Texture2D(texture.width, texture.height, (texture as Texture2D).format, false);
                newTexture.LoadRawTextureData((texture as Texture2D).GetRawTextureData());

                // Thanks Axel Garcia for fixing the read icon texture exception.
                //
                var bytes = new byte[0];
                try
                {
                    bytes = newTexture.EncodeToPNG();
                }
                catch (Exception e)
                {
                    Debug.Log(string.Format("Get Icon of {0} failed\r\n Exception: \r\n {1}", assetPath, e));
                    return GetNullKey();
                }

                if (bytes != null && bytes.Length > 0)
                {
                    var iconData = System.Convert.ToBase64String(newTexture.EncodeToPNG());
                    var key = Utility.MD5(iconData);
                    APCache.AddIcon(key, iconData);
                    return key;
                }
                else
                {
                    return GetNullKey();
                }
            }
            catch
            {
                return GetNullKey();
            }
        }

        private static bool IsSupportedFormat(TextureFormat format) 
        {
            // Supported float for PNG export: ARGB32, RGB32, RGB24, Aplha8 or one of float formats
            // 
            return format == TextureFormat.ARGB32 
                   || format == TextureFormat.RGBA32 
                   || format == TextureFormat.RGB24
                   || format == TextureFormat.Alpha8
                   || format.ToString().Contains("Float");
        }

        private static string GetNullKey()
        {
            var nullKey = "AA625B9BDB3AE0E4";
            var nullIcon = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAFBUlEQVRYCdVXa0wcVRT+dvbN7rK0LE8tVBpJTLU/tBFC+OEPhURrfCQ1aSXRxkQwFZOKMdUmNYZqgqbBEEvBV4zGlNYfNlGDIGgULNrq1opoghEIBDbyKrvssju7OzOeMzC8dtqFJU30JGfuuXfuPee7555z5o4BiSTU1dWdThxeGVEUBQMDA+MdHR1v0qhILK+83Zxk0JmeMTc3d1VnHGyYWZZlSJKEeDyOcDiM6elplJaWGmnNpoHoAiClSQFoIKLRqArC5/OhrKxs0yBMejs1mXSHlz3AaxgAe4CJvZKVlYXJyUkpOzubF0vqiw08BL05giDgWmw0GtV33DJQbs1mM6xWK1wuF6amphiV/g50jOkCMBgM0GMGpY2zrIFgmcFYLBY4nU4GESNbZh17CUObArDaOMuaYfYAM/c1EBSYUbJmSbC4bmDDruJgY4rFYur5MwAtIzgWIpEIBgcHEQqFEAwG1cwgEKLH47HTsoi6WOehmwWkeE0WsIGv3zuO/Iw4Pursxf6yvfhrYggfnr2Cintuwvc/jeL5w4cwO34FcOyGYDejrv4k2ru8mJ+fR0lJiYNsL+jYx4YA8K6feOB27CjMRVG+G5+cu4jvBiZAAaF6of7cGDKz8sgzccTFBVh+roEjehX7jrSpHmOvFBcX83FwbKyh6wJgF7OrNWrqilCwWdguIVewGEDykkw1yKBQfkqIdj0DQZnBwaNn1YLFG+AjKigoWFG2pDRpDDR1BmGz2VQjDiuloEEiebHgCdQyEKoEBIZZhiIoaL9wCQfv360GJmcKMwcnEWNeUy11s4BnMvHun6twwkoxZDWIFNIibEIUNmMcaaYYsYQ0MzH1LTTHIodhkxfQ1vk77TikpqaWnksArIuaV57XBaBNSzMrZEiB06LATkbtJplASLCSLMgihHgIl/t9cGEEJ448hNKyclzw71U3sLpekL6EtEx6BAzCZmavyYhJClpbf8X0yC/kdRNM8gwkIR1Gu4CIrw25jz+M2oYP8G1zPb7p/BINd1TgaFU5q1CDlZqEGEgYoEkZq9NwaGgIj1a9A7MkwpBxM6RwACazHYJZxH1V+1B5dw5ciojuzs+x885KDP/Qh8eePoB3G0+hq/s8ent62L5K5I1tJMwtddUmKYBAIID91R/D4XbgpZcPIEyV3kUBpRCgsWgA2+UIFkJR7MpxIduThZgiofGV1/FI9VPI3+ZCjtulHgXHE3ECgKQx4HBQDZn0Ykd2DkZDIooyLch0K/D5w7jNZoYjFsF4/48403QCf7Y3o/bBPZgb6cH5k8fgou/C+lRevXuWk3qAJ/0z40f/rB0FGUYcf7IczZ/2YoLKrX8+iJ62M7joD2KP24K/L/fBmeGBNTyLW2/xQLJtR82Lr2kpmJoHGEBOphtTaSF8deoFFOWmY7zvfXzRcBjDnzXiN28fKnflQYjOI9OTjsDsBNVcCX0DY1STYmoNYB3Xog15gBf7RoeQV1Ck6vF6vbDb7cjNzVXvAJzrr77xFuaGvQgHpiErBqQ7HdhZWIhnj/G1cZH0YmBDAERRXP7casrWt3zWraffxh+Xuqn8yrir7F4cqq5VA1CbmzIATcFWWz0ASbNgq0aTrf//AaipqYHf71/eGPe3Qil5wO12L9tsaWnhS+hyf7NCSgDWG+F/glRpQ2mYqvL16/6TWaDnAb4j8FfrRhDfthf/526E9lR0/gs8uBE8u4t8mgAAAABJRU5ErkJggg==";
            APCache.AddIcon(nullKey, nullIcon);
            return nullKey;
        }

        public static APAsset GetBlackListAPAsset(string path)
        {
            APAsset asset = new APAsset();
            asset.Name = path;
            asset.Path = path;
#if UNITY_2018
            if (path.Equals("Packages/"))
            {
                asset.Icon = GetIconID("Assets");
            }
            else
            {
                asset.Icon = GetIconID(path, path.EndsWith(".ani"));
            }
#else
            asset.Icon = GetIconID(path, path.EndsWith(".ani"));
#endif
            asset.APType = AssetType.BLACKLIST;
            asset.Id = AssetDatabase.AssetPathToGUID(path);

            return asset;
        }

        public static string GetAssetTypeByObject(UnityEngine.Object obj)
        {
            if (obj is Texture)
            {
                if (obj is MovieTextureType)
                {
                    return AssetType.MOVIES;
                }
                else
                {
                    return AssetType.TEXTURES;
                }
            }
            else if (obj is GameObject)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (path.ToLower().EndsWith(".prefab"))
                {
                    return AssetType.PREFABS;
                }
                else
                {
                    return AssetType.MODELS;
                }
            }
            else if (obj is AnimationClip)
            {
                return AssetType.ANIMATIONS;
            }
            else if (obj is AudioClip)
            {
                return AssetType.AUDIOS;
            }
            else if (obj is Shader)
            {
                return AssetType.SHADERS;
            }
            else if (obj is Font)
            {
                return AssetType.FONTS;
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (path.Contains("/StreamingAssets/"))
                {
                    return AssetType.STREAMING_ASSETS;
                }
                else if (path.EndsWith(".mat") || path.EndsWith(".physicmaterial") || path.EndsWith(".physicsMaterial2D"))
                {
                    return AssetType.MATERIALS;
                }
                else if (Utility.IsCodeFile(path))
                {
                    return AssetType.CODE;
                }
            }

            return AssetType.OTHERS;
        }

        public static void RenameAnimationClipInFbx(string modelPath, long[] fileId, string[] newNames, ref int changedCount)
        {
            Utility.DebugLog(string.Format("rename animation clip {1} in model {0}", modelPath, fileId));

            ModelImporter modelImporter = ModelImporter.GetAtPath(modelPath) as ModelImporter;
            if (modelImporter == null)
            {
                changedCount = 0;
                return;
            }

            SerializedObject serializedObject = new SerializedObject(modelImporter);
            var animationTypeProperty = serializedObject.FindProperty("m_AnimationType");
            var previousAnimationType = modelImporter.animationType;
            bool changedToLegacy = false;

            if (previousAnimationType != ModelImporterAnimationType.Legacy && animationTypeProperty != null)
            {
                changedToLegacy = true;
                animationTypeProperty.intValue = (int)ModelImporterAnimationType.Legacy;
                serializedObject.ApplyModifiedProperties();
                ImportAsset(modelPath);
            }

            GameObject modelGO = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            if (fileId != null && newNames != null && fileId.Length == newNames.Length)
            {
                for (int i = 0; i < fileId.Length; i++)
                {
                    int index = -1;
                    var animations = AnimationUtility.GetAnimationClips(modelGO);
                    if (animations != null && animations.Length > 0)
                    {
                        index = 0;
                        foreach (var item in animations)
                        {
                            if (item.name.IndexOf("__preview__") != -1)
                            {
                                continue;
                            }

                            var fid = Utility.GetLocalIndentifierOfObject(item);
                            if (fid == fileId[i])
                            {
                                break;
                            }
                            else
                            {
                                index++;
                            }
                        }
                    }

                    if (index == -1)
                    {
                        continue;
                    }

                    var clipAnimations = serializedObject.FindProperty("m_ClipAnimations");
                    string oldName = clipAnimations.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue;
                    clipAnimations.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue = newNames[i];
                    PatchImportSettingRecycleIDUtility.Path(serializedObject, 74, oldName, newNames[i]);
                    changedCount++;
                }
            }

            if (changedToLegacy && animationTypeProperty != null)
            {
                animationTypeProperty.intValue = (int)previousAnimationType;
            }

            serializedObject.ApplyModifiedProperties();
            ImportAsset(modelPath);
            modelGO = null;
        }

        private static void ImportAsset(string path)
        {
            ImportAssets(new string[] { path });
        }

        private static void ImportAssets(string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                AssetDatabase.WriteImportSettingsIfDirty(path);
            }
            
            try
            {
                AssetDatabase.StartAssetEditing();
                for (int j = 0; j < paths.Length; j++)
                {
                    string path2 = paths[j];
                    AssetDatabase.ImportAsset(path2);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private static HashSet<string> GetTagsInCode(string code)
        {
            HashSet<string> set = new HashSet<string>();
            string pattern = "(CompareTag|FindGameObjectsWithTag|FindGameObjectWithTag)\\(\"(?<TAG>\\S+?)\"\\)|.tag\\s*(==|!=)\\s*\"(?<TAG>\\S+?)\"";
            var matches = Regex.Matches(code, pattern);

            if (matches.Count <= 0)
            {
                return set;
            }

            foreach (Match match in matches)
            {
                var tag = match.Groups["TAG"].Value;
                if (!string.IsNullOrEmpty(tag))
                {
                    set.Add(tag);
                }
            }

            return set;
        }
    }
}
#endif