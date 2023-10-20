using M.ActionPipeline;

namespace Model
{
    [LifeCycle]
    public class ActionPipelineComponent : Component, IAwake
    {
        private ActionPipeline _pipeline;

        public ActionPipeline ActionPipeline => _pipeline;

        public void Awake()
        {
            _pipeline = new ActionPipeline();
        }

        public override void Dispose()
        {
            _pipeline.Dispose();
            _pipeline = null;
            base.Dispose();
        }
    }
}