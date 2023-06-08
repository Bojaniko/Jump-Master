using UnityEngine;

namespace Studio28.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class MinMaxAttribute : PropertyAttribute
    {
        public readonly float MinValue;
        public readonly float MaxValue;

        public MinMaxAttribute(float min_value, float max_value)
        {
            MinValue = min_value;
            MaxValue = max_value;
        }
    }
}