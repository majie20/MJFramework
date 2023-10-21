using System.Collections.Generic;

namespace Model
{
    public static class LoadInfo
    {
        public static Dictionary<int, List<AssetReferenceSettings.Info>> InfosMap = new();

        static LoadInfo()
        {
            //InfosMap.Add(1,
            //    new List<AssetReferenceSettings.Info>()
            //    {
            //        new(typeof(AssetReferenceSettings).FullName, null, "Assets/Res/Config/ScriptableObject/AssetReference/Init_ARS.asset", LoadType.Normal),
            //    });

            InfosMap.Add(1,
                new List<AssetReferenceSettings.Info>()
                {
                    new(typeof(AssetReferenceSettings).FullName, null, "Assets/Res/Config/ScriptableObject/AssetReference/GameLoadComplete_ARS.asset", LoadType.Normal),
                });

            InfosMap.Add(10003,
                new List<AssetReferenceSettings.Info>()
                {
                    new(typeof(AssetReferenceSettings).FullName, null, "Assets/Res/Environment/Level/Level1/Level1_ARS.asset", LoadType.Normal),
                });
        }
    }
}