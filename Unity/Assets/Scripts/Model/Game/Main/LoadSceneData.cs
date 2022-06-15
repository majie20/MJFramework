using System;
using Cysharp.Threading.Tasks;

namespace Model
{
    public class LoadSceneData
    {
        public string ScenePath;
        public string SettingsPath;
        public Func<UniTask> Call;
    }
}