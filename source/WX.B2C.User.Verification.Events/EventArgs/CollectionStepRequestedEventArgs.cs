using System;
using WX.B2C.User.Verification.Events.Constants;

namespace WX.B2C.User.Verification.Events.EventArgs
{
    public class CollectionStepRequestedEventArgs : System.EventArgs
    {
        public Guid UserId { get; set; }

        /// <summary>
        /// Value from set of constants:<see cref="CollectionStepTypes"/> connected to collection step.
        /// </summary>
        public string StepType { get; set; }
    }

    public class SurveyRequestedEventArgs : CollectionStepRequestedEventArgs
    {
        public Guid TemplateId { get; set; }
    }
}