using Cysharp.Threading.Tasks;

namespace M.ActionPipeline
{
    public class Action
    {
        private UniTask        _task;
        private ActionPipeline _pipeline;

        public Action(UniTask task, ActionPipeline pipeline)
        {
            _task = task;
            _pipeline = pipeline;
        }

        public async UniTaskVoid Play()
        {
            var b = await _task.SuppressCancellationThrow();

            if (b)
            {
                return;
            }

            _pipeline.Finish();
        }
    }
}