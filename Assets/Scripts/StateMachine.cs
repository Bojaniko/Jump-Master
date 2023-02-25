namespace Studio28.Utility
{
    public class StateMachine<StateType> where StateType : System.Enum
    {
        public delegate void StateMachineEventHandler(StateType state);

        public event StateMachineEventHandler OnStateChanged;

        private readonly string _name;

        public readonly StateType DefaultState;

        public StateType CurrentState { get; private set; }
        public StateType DeltaState { get; private set; }

        public StateMachine(string state_machine_name, StateType default_state)
        {
            _name = state_machine_name;

            DefaultState = default_state;

            CurrentState = default_state;
            DeltaState = default_state;

            //Debug.Log("Initialized State Machine " + _name);
        }

        public void SetState(StateType movement_state)
        {
            if (CurrentState.Equals(movement_state) == false)
            {
                DeltaState = CurrentState;

                CurrentState = movement_state;

                if (OnStateChanged != null) OnStateChanged(movement_state);

                //Debug.Log(_name + " state set to " + CurrentState);
            }
        }
    }
}