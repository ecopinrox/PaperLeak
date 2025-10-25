using System;
using UnityEngine;

namespace Behaviour
{
    public class BTimer : BTask
    {
        public static Func<float> deltaTime = () => Time.fixedDeltaTime;

        protected readonly Func<float> _initialTime;
        protected float _time;
        protected BTask _next;

        public BTimer(BState state, Action action, Func<float> duration, bool allowTransitions = true) 
            : base(state, action, allowTransitions)
        {
            _initialTime = duration;
        }

        public override void Init()
        {
            base.Init();
            _time = _initialTime();
        }

        public override BTask Tick()
        {
            if(_time <= 0f)
            {
                return _next;
            }
            
            _time -= deltaTime();
            return null;
        }

        public void SetNext(BTask next)
        {
            _next = next;
        }
    }
}