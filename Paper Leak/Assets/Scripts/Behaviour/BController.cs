namespace Behaviour
{
    public class BController
    {
        BTask _currentTask;

        public BState CurrentState { get { return _currentTask.state; } }

        bool _isRunning = true;

        public BController(BTask initialTask)
        {
            SetCurrentTask(initialTask);
        }

        public void Tick()
        {
            while(_isRunning)
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

        public void SetActive(bool state)
        {
            _isRunning = state;
        }
    }
}