using System;

namespace JumpMaster.Movement
{
    public class InvalidControlOfStateException : Exception
    {
        private readonly string _message;

        public InvalidControlOfStateException(MovementState state)
        {
            _message = $"There is no registered control of state {state}.";
        }

        public override string Message => _message;
    }
}