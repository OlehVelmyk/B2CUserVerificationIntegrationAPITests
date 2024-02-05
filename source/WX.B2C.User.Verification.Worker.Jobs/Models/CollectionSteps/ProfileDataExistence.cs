using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models.CollectionSteps
{
    internal class ProfileDataExistence : IJobData
    {
        public Guid UserId { get; set; }

        public Guid PolicyId { get; set; }

        public ApplicationState State{ get; set; }
        
        public bool FullName { get; set; }

        public bool DateOfBirth { get; set; }

        public bool Address { get; set; }

        public bool IpAddress { get; set; }

        public bool TaxResidence { get; set; }

        public bool IdDocumentNumber { get; set; }

        public bool Tin { get; set; }

        public bool Nationality { get; set; }
    }
}