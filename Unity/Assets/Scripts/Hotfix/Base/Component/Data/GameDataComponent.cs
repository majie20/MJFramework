#if MBuild

using Bright.Serialization;

#else
using SimpleJSON;
#endif

namespace Hotfix
{
    public class GameDataComponent : Model.Component, IAwake
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
            //NLog.Log.Error(jsonTables.TbItem.Get(1020100001));
            //Model.AsyncTimerHelper.TimeHandle(this, () => { NLog.Log.Error("bbbbbbbb"); }, 1000, 1000);
        }

        public override void Dispose()
        {
            jsonTables = null;
            base.Dispose();
        }
    }
}