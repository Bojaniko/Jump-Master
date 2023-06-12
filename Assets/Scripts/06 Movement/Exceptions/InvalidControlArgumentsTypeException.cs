using System;

namespace JumpMaster.Movement
{
    public class InvalidControlArgumentsTypeException : Exception
    {
        private readonly string _message;

        public InvalidControlArgumentsTypeException()
        {
            _message = $"Invalid arguments object given to movement control.";
        }

        public override string Message => _message;
    }
}
