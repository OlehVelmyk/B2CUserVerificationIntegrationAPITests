namespace WX.B2C.User.Verification.Events.Internal.Dtos
{
    public class PropertyChangeDto
    {
        public string PropertyName { get; set; }

        public string PreviousValue { get; set; }

        public string NewValue { get; set; }

        public bool IsReset { get; set; }
    }
}
