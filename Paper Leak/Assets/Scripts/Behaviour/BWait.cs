using System;
using System.Diagnostics;

namespace Behaviour
{
    public class BWait : BTask
    {
        protected readonly Func<bool> _condition;
        protected BTask _next;

        public BWait(BState state, Action action, Func<bool> condition, bool allowTransitions = true)
            : base(state, action, allowTransitions) 
        {
            _condition = condition;           
        }

        public override BTask Tick()
        {
            return _condition() ? _next : null;
        }

        public void SetNext(BTask next)
        {
            _next = next;
        }
    }
}