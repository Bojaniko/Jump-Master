using System;

using UnityEngine;

namespace JumpMaster.Movement
{
    public class InvalidControlException : Exception
    {
        private readonly string _message;

        public InvalidControlException(string control_type)
        {
            _message = $"Control of {control_type} type is not registered!";
        }

        public override string Message => _message;
    }
}