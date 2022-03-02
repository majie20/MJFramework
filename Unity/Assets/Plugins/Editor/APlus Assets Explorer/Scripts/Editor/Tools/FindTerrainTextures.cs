//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace APlus
{
    public class FindTerrainTextures
    {
        [MenuItem("Tools/A+ Assets Explorer/Tools/Find Terrain Textures In Current Scene", false, 111)]
        public static void FindTerrains()
        {
			if (APlusWindow.Instance == null)
			{
				EditorUtility.DisplayDialog("Error", "Please start Assets Explorer first.", "Ok");
				return;
			}

            var gos = Resources.FindObjectsOfTypeAll<GameObject>().Where(IsTerrainObject).ToArray();

            var ids = new HashSet<string>();
            foreach (var go in gos)
            {
                var terrain = go.GetComponent<Terrain>();

                #pragma warning disable
                var splatPrototypes = terrain.terrainData.splatPrototypes;
                #pragma warning restore

                for (var i = 0; i < splatPrototypes.Length; i++)
                {
                    var tex = splatPrototypes[i].texture;
                    if (tex != null)
                    {
                        var path = AssetDatabase.GetAssetPath(tex);
                        ids.Add(AssetDatabase.AssetPathToGUID(path));
                    }
                }
            }

            if (ids.Count() > 0)
            {
                APlusWindow.LoadWindow();
                AssetNotification.webCommunicationService.SetCurrentURL(string.Format("res/{0}", "textures"), string.Format("Id:{0}", string.Join("|", ids.ToArray())));
            }
            else
            {
                EditorUtility.DisplayDialog("Not Results Found", "No terrain textures in current secne", "Ok");
            }
        }

		private static bool IsTerrainObject(GameObject go)
		{
			return string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go))
                   && go.hideFlags == HideFlags.None 
				   && go.GetComponent<Terrain>() != null;
		}
    }
}
#endif