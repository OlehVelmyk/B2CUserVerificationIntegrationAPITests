// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines values for TaskState.
    /// </summary>
    /// <summary>
    /// Determine base value for a given allowed value if exists, else return
    /// the value itself
    /// </summary>
    [JsonConverter(typeof(TaskStateConverter))]
    public struct TaskState : System.IEquatable<TaskState>
    {
        private TaskState(string underlyingValue)
        {
            UnderlyingValue=underlyingValue;
        }

        public static readonly TaskState INCOMPLETE = "INCOMPLETE";

        public static readonly TaskState COMPLETEDPASS = "COMPLETED_PASS";

        public static readonly TaskState COMPLETEDFAIL = "COMPLETED_FAIL";


        /// <summary>
        /// Underlying value of enum TaskState
        /// </summary>
        private readonly string UnderlyingValue;

        /// <summary>
        /// Returns string representation for TaskState
        /// </summary>
        public override string ToString()
        {
            return UnderlyingValue == null ? null : UnderlyingValue.ToString();
        }

        /// <summary>
        /// Compares enums of type TaskState
        /// </summary>
        public bool Equals(TaskState e)
        {
            return UnderlyingValue.Equals(e.UnderlyingValue);
        }

        /// <summary>
        /// Implicit operator to convert string to TaskState
        /// </summary>
        public static implicit operator TaskState(string value)
        {
            return new TaskState(value);
        }

        /// <summary>
        /// Implicit operator to convert TaskState to string
        /// </summary>
        public static implicit operator string(TaskState e)
        {
            return e.UnderlyingValue;
        }

        /// <summary>
        /// Overriding == operator for enum TaskState
        /// </summary>
        public static bool operator == (TaskState e1, TaskState e2)
        {
            return e2.Equals(e1);
        }

        /// <summary>
        /// Overriding != operator for enum TaskState
        /// </summary>
        public static bool operator != (TaskState e1, TaskState e2)
        {
            return !e2.Equals(e1);
        }

        /// <summary>
        /// Overrides Equals operator for TaskState
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is TaskState && Equals((TaskState)obj);
        }

        /// <summary>
        /// Returns for hashCode TaskState
        /// </summary>
        public override int GetHashCode()
        {
            return UnderlyingValue.GetHashCode();
        }

    }
}