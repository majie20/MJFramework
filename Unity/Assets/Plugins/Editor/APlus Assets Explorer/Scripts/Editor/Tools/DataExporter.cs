//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System;

namespace APlus
{
    public class DataExporter
    {
        enum DataType
        {
            CSV,
        }

        [MenuItem("Tools/A+ Assets Explorer/Data Exporter/Export as CSV...", false, 33)]
        public static void ExportToCSV()
        {
            string title = "Export as CSV";
            SaveDataWithDialog(title, DataType.CSV);
        }

        private static void SaveDataWithDialog(string title, DataType type)
        {
            string folderPath = EditorUtility.OpenFolderPanel(title, Application.dataPath, "");
            Utility.DebugLog(folderPath);
            if (!string.IsNullOrEmpty(folderPath))
            {
                switch (type)
                {
                    case DataType.CSV:
                        SaveCSV(folderPath);
                        break;
                }
            }
        }

        private static void SaveCSV(string folderPath)
        {
            SaveToLocal(Path.Combine(folderPath, "textures.csv"), GetTextureCSV());
            SaveToLocal(Path.Combine(folderPath, "models.csv"), GetModlesCSV());
            SaveToLocal(Path.Combine(folderPath, "animationClips.csv"), GetAnimationClipCSV());
            SaveToLocal(Path.Combine(folderPath, "material.csv"), GetMaterialsCSV());
            SaveToLocal(Path.Combine(folderPath, "prefabs.csv"), GetPrefabsCSV());
            SaveToLocal(Path.Combine(folderPath, "shaders.csv"), GetShadersCSV());
            SaveToLocal(Path.Combine(folderPath, "audios.csv"), GetAudiosCSV());
            SaveToLocal(Path.Combine(folderPath, "fonts.csv"), GetFontCSV());
            SaveToLocal(Path.Combine(folderPath, "movies.csv"), GetMoviesCSV());
            SaveToLocal(Path.Combine(folderPath, "streamingAssets.csv"), GetStreamingAssetsCSV());
            SaveToLocal(Path.Combine(folderPath, "others.csv"), GetOtherFilesCSV());

            string message = string.Format("Saved to folder {0}", folderPath);
            if(EditorUtility.DisplayDialog("Done!", message, "OK"))
            {
                EditorUtility.RevealInFinder(folderPath);
            }
        }

        private static void SaveToLocal(string filePath, string data)
        {
            File.WriteAllText(filePath, data);
        }

        private static string GetOtherFilesCSV()
        {
            string header = "Name,FileSize,Used,Path,MD5";
            return GenerateCSV<APOtherFile>(header, APAssetType.Others, file => 
                string.Format("{0},{1},{2},{3},{4}", file.Name, file.FileSize, file.Used, file.Path, file.Hash)
            );
        }

        private static string GetStreamingAssetsCSV()
        {
            string header = "Name,FileSize,Used,Path,MD5";
            return GenerateCSV<APStreamingAssetsFile>(header, APAssetType.StreamingAssets, file => 
                string.Format("{0},{1},{2},{3},{4}", file.Name, file.FileSize, file.Used, file.Path, file.Hash)
            );
        }

        private static string GetMoviesCSV()
        {
            string header = "Name,Approx,FileSize,Quality,TextureSize,Used,Path,MD5";
            return GenerateCSV<APMovieTexture>(header, APAssetType.MovieTexture, movie => 
                string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", 
                    movie.Name, 
                    movie.Approx,
                    movie.FileSize,
                    movie.Quality,
                    movie.Size,
                    movie.Used,
                    movie.Path,
                    movie.Hash)
            );
        }

        private static string GetFontCSV()
        {
            string header = "Name,FileSize,FontNames,Character,RenderingMode,Used,Path,MD5";
            return GenerateCSV<APFont>(header, APAssetType.Font, font => 
                string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", 
                    font.Name, 
                    font.FileSize, 
                    font.FontNames,
                    font.Character,
                    font.RenderingMode,
                    font.Used,
                    font.Path,
                    font.Hash)
            );
        }

        private static string GetAudiosCSV()
        {
            string header = "Name,FileSize,ImportedSize,Duration,Compress,Frequency,Ratio,Quality,Background,Used,Path,MD5";
            return GenerateCSV<APAudio>(header, APAssetType.AudioClip, audio => 
                string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", 
                    audio.Name, 
                    audio.FileSize, 
                    audio.ImportedSize,
                    audio.Duration, 
                    audio.Compress, 
                    audio.Frequency,
                    audio.Ratio,
                    audio.Quality,
                    audio.Background,
                    audio.Used,
                    audio.Path,
                    audio.Hash)
            );
        }

        private static string GetShadersCSV()
        {
            string header = "Name,FileSize,FileName,RenderQueue,RenderQueueText,LOD,CastShadows,DisableBatching,IgnoreProjector,SurfaceShader,VariantsTotal,VariantsIncluded,Used,Path,MD5";
            return GenerateCSV<APShader>(header, APAssetType.Shader, shader => 
                string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}", 
                    shader.Name,
                    shader.FileSize,
                    shader.FileName,
                    shader.RenderQueue,
                    shader.RenderQueueText,
                    shader.LOD,
                    shader.CastShadows,
                    shader.DisableBatching,
                    shader.IgnoreProjector,
                    shader.SurfaceShader,
                    shader.VariantsTotal,
                    shader.VariantsIncluded,
                    shader.Used,
                    shader.Path,
                    shader.Hash)
            );
        }

        private static string GetCodeCSV()
        {
            string header = "Name,FileSize,FileType,Used,Path,MD5";
            return GenerateCSV<APCodeFile>(header, APAssetType.Script, code => 
                string.Format("{0},{1},{2},{3},{4},{5}", code.Name, code.FileSize, code.FileType, code.Used, code.Path, code.Hash)
            );
        }

        private static string GetPrefabsCSV()
        {
            string header = "Name,FileSize,Used,Path,MD5";
            return GenerateCSV<APPrefab>(header, APAssetType.Prefab, prefab => 
                string.Format("{0},{1},{2},{3},{4}", prefab.Name, prefab.FileSize, prefab.Used, prefab.Path, prefab.Hash)
            );
        }

        private static string GetMaterialsCSV()
        {
            string header = "Name,FileSize,Shader,Type,Used,Path,MD5";
            return GenerateCSV<APMaterial>(header, APAssetType.Material, material => 
                string.Format("{0},{1},{2},{3},{4},{5},{6}", 
                    material.Name,
                    material.FileSize,
                    material.Shader,
                    material.Type,
                    material.Used,
                    material.Path,
                    material.Hash)
            );
        }

        private static string GetAnimationClipCSV()
        {
            string header = "Name,InFile,FileSize,Length,FPS,CycleOffset,LoopTime,LoopPose,Used,Path,MD5";
            return GenerateCSV<APAnimation>(header, APAssetType.AnimationClip, clip =>
                string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                        clip.Name,
                        clip.InFile,
                        clip.FileSize,
                        clip.Length,
                        clip.FPS,
                        clip.CycleOffset,
                        clip.LoopTime,
                        clip.LoopPose,
                        clip.Used,
                        clip.Path,
                        clip.Hash)
            );
        }

        private static string GetModlesCSV()
        {
            string header = "Name,FileSize,Vertexes,Tris,ScaleFactor,MeshCompression,ReadWrite,ImportBlendShapes,GenerateColliders,LightmapToUV2,OptimizeMesh,SwapUVs,Used,Path,MD5";
            return GenerateCSV<APModel>(header, APAssetType.Model, model =>
                string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
                            model.Name,
                            model.FileSize,
                            model.Vertexes,
                            model.Tris,
                            model.ScaleFactor,
                            model.MeshCompression,
                            model.ReadWrite,
                            model.ImportBlendShapes,
                            model.GenerateColliders,
                            model.LightmapToUV2,
                            model.OptimizeMesh,
                            model.SwapUVs,
                            model.Used,
                            model.Path,
                            model.Hash));
        }

        private static string GetTextureCSV()
        {
            string header = "Name,FileSize,StorageSize,RuntimeSize,MaxSize,MipMap,ReadWrite,TextureFormat,TextureType,Compression,CrunchedCompression,CompressionQuality,Width,Height,WidthInPixel,HeightInPixel,Used,Path,MD5";
            return GenerateCSV<APTexture>(header, APAssetType.Texture, texture =>
                string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}",
                            texture.Name,
                            texture.FileSize,
                            texture.StorageSize,
                            texture.RuntimeSize,
                            texture.MaxSize,
                            texture.MipMap,
                            texture.ReadWrite,
                            texture.TextureFormat,
                            texture.TextureType,
                            texture.Compress,
                            texture.CrunchedCompression,
                            texture.CompressionQuality,
                            texture.Width,
                            texture.Height,
                            texture.WidthInPixel,
                            texture.HeightInPixel,
                            texture.Used,
                            texture.Path,
                            texture.Hash)
            );

        }

        private static string GenerateCSV<T>(string header, APAssetType type, Func<T, string> rowDataGenerator) where T : APAsset
        {
            var dataSet = APCache.GetAssetsListByTypeFromCache<T>(type);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(header);
            foreach (var item in dataSet)
            {
                sb.AppendLine(rowDataGenerator(item));
            }

            return sb.ToString();
        }
    }
}
#endif
