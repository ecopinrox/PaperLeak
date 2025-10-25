namespace Behaviour
{
    public class BController
    {
        BTask _currentTask;

        public BState CurrentState { get { return _currentTask.state; } }

        public BController(BTask initialTask)
        {
            SetCurrentTask(initialTask);
        }

        public void Tick()
        {
            while(true)
            {
                BTask nextTask = _currentTask.Tick();

                if (nextTask == null) 
                    return;

                SetCurrentTask(nextTask);
            }
        }

        public void SetCurrentTask(BTask task)
        {
            _currentTask = task;
            _currentTask.Init();
        }
    }
}