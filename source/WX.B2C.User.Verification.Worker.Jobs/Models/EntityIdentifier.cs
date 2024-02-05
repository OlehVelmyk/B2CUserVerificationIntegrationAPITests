using System;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    /// <summary>
    /// Generic model for jobs where only entity id is needed
    /// </summary>
    internal class EntityIdentifier : IJobData
    {
        public Guid Id { get; set; }
    }
}