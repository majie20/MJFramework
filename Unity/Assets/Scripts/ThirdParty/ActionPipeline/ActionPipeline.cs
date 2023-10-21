using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace M.ActionPipeline
{
    public class ActionPipeline : IDisposable
    {
        private Queue<Action>           _actions;
        private bool                    _isRunning;
        private CancellationTokenSource _cancellationTokenSource;

        public ActionPipeline()
        {
            _isRunning = false;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Append(UniTask task)
        {
            task.AttachExternalCancellation(_cancellationTokenSource.Token);
            _actions.Enqueue(new Action(task, this));

            if (!_isRunning)
            {
                _isRunning = true;
                _actions.Peek().Play().Forget();
            }
        }

        public void Finish()
        {
            _actions.Dequeue();

            if (_actions.Count > 0)
            {
                _actions.Peek().Play().Forget();
            }
            else
            {
                _isRunning = false;
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            _actions = null;
        }
    }
}