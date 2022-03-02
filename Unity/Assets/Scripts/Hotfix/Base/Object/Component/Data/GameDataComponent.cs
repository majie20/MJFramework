using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using SimpleJSON;
using UnityEditor;
using UnityEngine;

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
            JsonTables = new cfg.Tables(file => JSON.Parse(Model.Game.Instance.Scene.GetComponent<Model.AssetsComponent>().Load<UnityEngine.TextAsset>($"{Model.FileValue.BUILD_AB_RES_PATH}Config/Json/{file}").text));
        }

        public override void Dispose()
        {
            jsonTables = null;
            Entity = null;
        }
    }
}