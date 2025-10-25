using System;

namespace Behaviour
{
    public class BAction : BTask
    {
        protected BTask _next;

        public BAction(BState state, Action action, bool allowTransitions = true) 
            : base(state, action, allowTransitions) 
        { }

        public override BTask Tick()
        {
            return _next;
        }

        public void SetNext(BTask next)
        {
            _next = next;
        }
    }
}