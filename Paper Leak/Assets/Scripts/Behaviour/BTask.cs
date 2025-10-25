using System;

namespace Behaviour
{
    public abstract class BTask
    {
        public readonly BState state;
        public readonly bool allowTransitions;
        protected readonly Action _action;

        public BTask(BState state, Action action, bool allowTransitions = true)
        {
            this.state = state;
            _action = action;
            this.allowTransitions = allowTransitions;
        }

        public BTask(BState state, bool allowTransitions = true) : this(state, null, allowTransitions) 
        { }

        public virtual void Init() => _action();
        public abstract BTask Tick();
    }
}