using UnityEditor;

namespace M.ProductionPipeline
{
    public class ExportAllUISpriteAtlasStep : IStep
    {
        public void Run()
        {
            SpriteAtlasEditor.ExportAllUISpriteAtlas(EditorConst.UI_SPRITE_ATLAS_SETTINGS);
            SpriteAtlasEditor.ExportAllUISpriteAtlas(EditorConst.UNIT_SPRITE_ATLAS_SETTINGS);
        }

        public string EnterText()
        {
            return $"导出所有UI图集 开始！";
        }

        public string ExitText()
        {
            return $"导出所有UI图集 结束！";
        }

        public bool IsTriggerCompile()
        {
            return false;
        }
    }
}