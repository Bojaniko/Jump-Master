using System.Collections.Generic;

using UnityEngine;

using JumpMaster.Core;

using Studio28.Utility;

namespace JumpMaster.Controls
{
    public delegate void InputProcessorPerformed(IInputPerformedEventArgs args);
    public delegate void InputStateChanged(InputProcessState state, InputProcessorDataSO processor_data);

    public class InputController : LevelController
    {

        private static InputController s_instance;

        public static InputController Instance
        {
            get
            {
                return s_instance;
            }
            private set
            {
                if (s_instance == null)
                    s_instance = value;
                else
                    Debug.LogError("You can have only one instance of an Input Controller!");
            }
        }

        protected override void Initialize()
        {
            Instance = this;

            _input = new();
            _input.Enable();
            _input.Movement.Contact.performed += ctx => StartTouch();
            _input.Movement.Contact.canceled += ctx => EndTouch();

            GenerateProcessors();

            _stateController = new("Input Controller", InputState.WAITING);
            _stateController.OnStateChanged += ManageStateChange;
        }

        private InputActions _input;
        [SerializeField] private InputControllerDataSO _data;

        private void FixedUpdate()
        {
            if (!_stateController.CurrentState.Equals(InputState.STARTED))
                return;
            UpdateProcessors();
        }

        // ##### STATE CALLBACKS ##### \\

        private Dictionary<System.Type, List<InputStateRegistration>> _registeredStateChanges;

        /// <summary>
        /// Register a listener to the change of a state of a input processor. 
        /// </summary>
        /// <typeparam name="InputProcessor">The input processor to which to bind the action.</typeparam>
        /// <param name="callback_action">The function which will be called when the processor state changes.</param>
        /// <param name="registrator">The key-pair binding object to the callback action.</param>
        public void RegisterInputStateChangeListener<InputProcessor>(InputStateChanged callback_action, object registrator)
            where InputProcessor : IInputProcessor =>
            _registeredStateChanges[typeof(InputProcessor)].Add(new(callback_action, registrator));

        /// <summary>
        /// Unregister a listener from the input state change callback pool.
        /// Complexity of !O(n2) where n is the number of registrations per processor.
        /// </summary>
        /// <typeparam name="InputProcessor">The input processor to which the registrator has been paired to.</typeparam>
        /// <param name="registrator">The key-pair object binded to the inpur processor.</param>
        public void UnregisterInputStateChangeListener<InputProcessor>(object registrator)
            where InputProcessor : IInputProcessor
        {
            foreach (InputStateRegistration registration in _registeredStateChanges[typeof(InputProcessor)])
            {
                if (registration.Registrator.Equals(registrator))
                {
                    _registeredStateChanges[typeof(InputProcessor)].Remove(registration);
                    return;
                }
            }
        }
        private void ProcessState(IInputProcessor processor, InputProcessState state)
        {
            foreach (InputStateRegistration registration in _registeredStateChanges[processor.GetType()])
                registration.StateAction?.Invoke(state, processor.Data);
        }

        // ##### PERFORM CALLBACKS ##### \\

        private Dictionary<System.Type, List<InputCallbackRegistration>> _registeredInputCallbacks;
        /// <summary>
        /// Register a listener to be called when a specified input processor is performed.
        /// </summary>
        /// <typeparam name="InputProcessor">The input processor to which to bind the action.</typeparam>
        /// <param name="callback_action">The function which will be called when the processor is performed.</param>
        /// <param name="registrator">The key-pair binding object to the callback action.</param>
        public void RegisterInputPerformedListener<InputProcessor>(InputProcessorPerformed callback_action, object registrator)
            where InputProcessor : IInputProcessor =>
            _registeredInputCallbacks[typeof(InputProcessor)].Add(new(callback_action, registrator));

        /// <summary>
        /// Unregister a listener from the input callback pool.
        /// Complexity of !O(n2) where n is the number of registrations per processor.
        /// </summary>
        /// <typeparam name="InputProcessor">The input processor to which the registrator has been paired to.</typeparam>
        /// <param name="registrator">The key-pair object binded to the inpur processor.</param>
        public void UnregisterInputPerformedListener<InputProcessor>(object registrator)
            where InputProcessor : IInputProcessor
        {
            foreach (InputCallbackRegistration registration in _registeredInputCallbacks[typeof(InputProcessor)])
            {
                if (registration.Registrator.Equals(registrator))
                {
                    _registeredInputCallbacks[typeof(InputProcessor)].Remove(registration);
                    return;
                }
            }
        }
        private void ProcessorPerformed(IInputProcessor processor, IInputPerformedEventArgs args)
        {
            CancelProcessorsExcept(processor);
            foreach (InputCallbackRegistration registration in _registeredInputCallbacks[processor.GetType()])
                registration.CallbackAction?.Invoke(args);
        }

        // ##### STATE CONTROLLER ##### \\

        private void StartTouch() => _stateController.SetState(InputState.STARTED);
        private void EndTouch() => _stateController.SetState(InputState.ENDED);

        private enum InputState { WAITING, STARTED, ENDED, CANCELED, PERFORMED }
        private StateMachine<InputState> _stateController;
        private void ManageStateChange(InputState state)
        {
            if (state.Equals(InputState.STARTED))
                StartProcessors();
            else if (state.Equals(InputState.ENDED))
                EndProcessors();
            else if (state.Equals(InputState.CANCELED))
                CancelProcessors();
        }

        // ##### PROCESSORS GENERATION ##### \\

        private List<IInputProcessor> _processors;
        private void GenerateProcessors()
        {
            _processors = new();
            _processors.Add(new TapProcessor(_data.TapData, _input.Movement.Position));
            _processors.Add(new SwipeProcessor(_data.SwipeData, _input.Movement.Position));
            _processors.Add(new HoldProcessor(_data.HoldData, _input.Movement.Position));
            _processors.Add(new DelayedSwipeProcessor(_data.SwipeData, _input.Movement.Position));

            GenerateCallbackLists();
        }
        private void GenerateCallbackLists()
        {
            _registeredStateChanges = new();
            _registeredInputCallbacks = new();
            foreach (IInputProcessor processor in _processors) {
                _registeredStateChanges.TryAdd(processor.GetType(), new List<InputStateRegistration>());
                _registeredInputCallbacks.TryAdd(processor.GetType(), new List<InputCallbackRegistration>());
                processor.OnInputPerformed += ProcessorPerformed;
                processor.OnStateChanged += ProcessState;
            }

        }

        // ##### PROCESSOR CONTROLS ##### \\

        private void StartProcessors()
        {
            foreach (IInputProcessor processor in _processors)
            {
                if (processor.CurrentState.Equals(InputProcessState.WAITING) ||
                    processor.CurrentState.Equals(InputProcessState.CANCELED))
                    processor.StartInputProcess();
            }
        }
        private void EndProcessors()
        {
            foreach (IInputProcessor processor in _processors)
            {
                if (!processor.CurrentState.Equals(InputProcessState.WAITING))
                    processor.StopInputProcess();
            }
        }
        private void CancelProcessors()
        {
            foreach (IInputProcessor processor in _processors)
            {
                if (!processor.CurrentState.Equals(InputProcessState.CANCELED))
                    processor.CancelInputProcess();
            }
        }
        private void CancelProcessorsExcept(IInputProcessor exception)
        {
            foreach (IInputProcessor processor in _processors)
            {
                if (processor != exception)
                    processor.CancelInputProcess();
            }
        }
        private void UpdateProcessors()
        {
            foreach (IInputProcessor processor in _processors)
            {
                if (!processor.CurrentState.Equals(InputProcessState.STARTED))
                    continue;
                processor.UpdateInputProcess();
            }
        }

        /// <summary>
        /// Call this from User Interface scripts when they are tapped.
        /// </summary>
        public void RegisterUserInterfaceInteraction()
        {
            if (_stateController.CurrentState.Equals(InputState.STARTED))
                _stateController.SetState(InputState.CANCELED);
        }
    }
}