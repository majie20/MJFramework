#if MBuild
using Bright.Serialization;
#else
using SimpleJSON;
#endif

using UnityEngine;

namespace Model
{
    public class GameConfigDataComponent : Model.Component, IAwake
    {
        private cfg.Tables _jsonTables;

        public cfg.Tables JsonTables
        {
            get { return _jsonTables; }
        }

        private cfg.Const.TbGameConst _gameConst;

        public cfg.Const.TbGameConst GameConst
        {
            get { return _gameConst; }
        }

        public void Awake()
        {
#if MBuild
            _jsonTables = new cfg.Tables(file =>
                new ByteBuf(Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<TextAsset>($"{ConstData.JSON_CONFIG_PATH}{file}.bytes").bytes));
#else
            _jsonTables = new cfg.Tables(file =>
                JSON.Parse(Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<TextAsset>($"{ConstData.JSON_CONFIG_PATH}{file}.json").text));
#endif

            _gameConst = _jsonTables.TbGameConst;
        }

        public override void Dispose()
        {
            _gameConst = null;
            _jsonTables = null;

            base.Dispose();
        }
    }
}