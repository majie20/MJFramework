using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Model
{
    public class Component : IDisposable
    {
        protected long guid;

        public long Guid
        {
            protected set
            {
                guid = value;
            }
            get
            {
                return guid;
            }
        }

        protected Entity entity;

        public Entity Entity
        {
            set
            {
                entity = value;
            }
            get
            {
                return entity;
            }
        }

        protected bool isRuning;

        public bool IsRuning
        {
            set
            {
                isRuning = value;
            }
            get
            {
                return isRuning;
            }
        }

        protected HashSet<IEvent> eventList = new HashSet<IEvent>();

        public HashSet<IEvent> EventList
        {
            protected set
            {
                eventList = value;
            }
            get
            {
                return eventList;
            }
        }

        protected HashSet<EventGroup<uint>> eventGroupList = new HashSet<EventGroup<uint>>();

        public HashSet<EventGroup<uint>> EventGroupList
        {
            protected set
            {
                eventGroupList = value;
            }
            get
            {
                return eventGroupList;
            }
        }

        public bool awakeCalled = false;
        public bool called = false;
        protected CancellationTokenSource cancellationTokenSource;

        public CancellationToken CancellationToken
        {
            get
            {
                if (cancellationTokenSource == null)
                {
                    cancellationTokenSource = new CancellationTokenSource();
                }

                if (!awakeCalled)
                {
                    PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, new AwakeMonitor(this));
                }

                return cancellationTokenSource.Token;
            }
        }

        public Component()
        {
            Guid = GuidHelper.GuidToLongID();
        }

        public virtual void Dispose()
        {
            Entity = null;
            foreach (var e in eventList)
            {
                e.RemoveListener2(this);
            }
            foreach (var e in eventGroupList)
            {
                e.RemoveListener(this);
            }
            eventList.Clear();
            eventGroupList.Clear();

            called = true;
            awakeCalled = false;

            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }

        private class AwakeMonitor : IPlayerLoopItem
        {
            private readonly Component trigger;

            public AwakeMonitor(Component trigger)
            {
                this.trigger = trigger;
            }

            public bool MoveNext()
            {
                if (trigger.called) return false;
                if (trigger == null)
                {
                    trigger.Dispose();
                    return false;
                }
                return true;
            }
        }
    }
}