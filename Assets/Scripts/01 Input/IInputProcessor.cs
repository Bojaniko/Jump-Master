namespace JumpMaster.Controls
{
    public delegate void InputStateEventHandler(IInputProcessor processor, InputProcessState state);
    public delegate void InputPerformedEventHandler(IInputProcessor processor, IInputPerformedEventArgs args);

    public enum InputProcessState { STARTED, CANCELED, WAITING }

    public interface IInputProcessor
    {
        public event InputStateEventHandler OnStateChanged;
        public event InputPerformedEventHandler OnInputPerformed;

        /// <summary>
        /// The processor data abstraction.
        /// </summary>
        public InputProcessorDataSO Data { get; }

        /// <summary>
        /// The current state of the processor.
        /// </summary>
        public InputProcessState CurrentState { get; }

        /// <summary>
        /// Called each frame when the Processor is STARTED.
        /// </summary>
        public void UpdateInputProcess();
        /// <summary>
        /// Called when tap input is detected.
        /// </summary>
        public void StartInputProcess();
        /// <summary>
        /// Called when the player lifts his finger from the screen.
        /// </summary>
        public void StopInputProcess();
        /// <summary>
        /// Called by the InputController when an input processor is performed.
        /// </summary>
        public void CancelInputProcess();
    }
}