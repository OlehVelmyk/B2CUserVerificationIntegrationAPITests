using WX.B2C.User.Verification.Domain;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class InitiationDto
    {
        public string Initiator { get; set; }

        public string Reason { get; set; }

        public static InitiationDto CreateSystem(string reason) => new() { Initiator = Initiators.System, Reason = reason };

        /// <summary>
        /// TODO investigate if we need reason at all for user actions. It is obvious that these are some system requirements to provide some data.
        /// </summary>
        public static InitiationDto CreateUser() => new() { Initiator = Initiators.User, Reason = "User action" };

        public static InitiationDto CreateAdmin(string email, string reason) => new() { Initiator = $"{Initiators.Admin}: {email}", Reason = reason };

        public static InitiationDto CreateJob(string jobName) => new() { Initiator = Initiators.Job, Reason = jobName };

        public static InitiationDto Create(string initiator, string reason) => new() { Initiator = initiator, Reason = reason };
    }
}