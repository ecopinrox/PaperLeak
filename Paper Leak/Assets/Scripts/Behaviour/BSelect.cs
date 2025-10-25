using System;

namespace Behaviour
{
    public class BSelect : BTask
    {
        protected readonly Func<bool> _condition;
        protected BTask _successTask;
        protected BTask _failureTask;

        public BSelect(BState state, Action action, Func<bool> condition, bool allowTransitions = true)
            : base(state, action, allowTransitions) 
        { 
            _condition = condition;
        }

        public override BTask Tick()
        {
            return _condition() ? _successTask : _failureTask;
        }

        public void SetNext(BTask successTask, BTask failureTask)
        {
            _successTask = successTask;
            _failureTask = failureTask;
        }
    }
}