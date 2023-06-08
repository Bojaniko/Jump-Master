namespace JumpMaster.Controls
{
    internal readonly struct InputStateRegistration
    {
        public readonly object Registrator;
        public readonly InputStateChanged StateAction;

        public InputStateRegistration(InputStateChanged state_action, object registrator)
        {
            StateAction = state_action;
            Registrator = registrator;
        }
    }
}