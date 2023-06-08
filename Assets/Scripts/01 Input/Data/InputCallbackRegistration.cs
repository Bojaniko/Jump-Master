namespace JumpMaster.Controls
{
    internal readonly struct InputCallbackRegistration
    {
        public readonly object Registrator;
        public readonly InputProcessorPerformed CallbackAction;

        public InputCallbackRegistration(InputProcessorPerformed callback_action, object registrator)
        {
            Registrator = registrator;
            CallbackAction = callback_action;
        }
    }
}