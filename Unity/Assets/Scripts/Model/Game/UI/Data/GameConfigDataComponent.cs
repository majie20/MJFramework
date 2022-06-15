#if MBuild

using Bright.Serialization;

#else
using SimpleJSON;
#endif

namespace Model
{
    public class GameConfigDataComponent : Model.Component, IAwake
    {
        private cfg.Tables jsonTables;

        public cfg.Tables JsonTables
        {
            private set
            {
                jsonTables = value;
            }
            get
            {
                return jsonTables;
            }
        }

        public void Awake()
        {
#if MBuild
            JsonTables = new cfg.Tables(file => new ByteBuf(Model.Game.Instance.Scene.GetComponent<Model.AssetsComponent>().LoadSync<UnityEngine.TextAsset>($"{Model.FileValue.RES_PATH}Config/JsonConfig/{file}.bytes").bytes));
#else
            JsonTables = new cfg.Tables(file => JSON.Parse(Model.Game.Instance.Scene.GetComponent<Model.AssetsComponent>().LoadSync<UnityEngine.TextAsset>($"{Model.FileValue.RES_PATH}Config/JsonConfig/{file}.json").text));
#endif
        }

        public override void Dispose()
        {
            jsonTables = null;
            base.Dispose();
        }
    }
}