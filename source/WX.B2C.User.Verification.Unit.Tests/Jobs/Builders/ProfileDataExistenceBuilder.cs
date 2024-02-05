using System;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;

namespace WX.B2C.User.Verification.Unit.Tests.Jobs.Builders
{
    internal interface IProfileDataExistenceBuilder : IUserConsistencyBuilder
    {
        IProfileDataExistenceBuilder FullName(bool exists = true);

        IProfileDataExistenceBuilder DateOfBirth(bool exists = true);

        IProfileDataExistenceBuilder Address(bool exists = true);

        IProfileDataExistenceBuilder IpAddress(bool exists = true);

        IProfileDataExistenceBuilder TaxResidence(bool exists = true);

        IProfileDataExistenceBuilder IdDocumentNumber(bool exists = true);

        IProfileDataExistenceBuilder IdDocumentNumberType(bool exists = true);

        IProfileDataExistenceBuilder Tin(bool exists = true);

        IProfileDataExistenceBuilder Nationality(bool exists = true);

        IProfileDataExistenceBuilder RiskLevel(bool exists = true);

        IProfileDataExistenceBuilder IdentityDocuments(bool exists = true);

        IProfileDataExistenceBuilder AddressDocuments(bool exists = true);

        IProfileDataExistenceBuilder ProofOfFundsDocuments(bool exists = true);

        IProfileDataExistenceBuilder W9Form(bool exists = true);
    }

    internal class ProfileDataExistenceBuilder : UserConsistencyBuilder, IProfileDataExistenceBuilder
    {
        private readonly ProfileDataExistence _profileDataExistence;

        public ProfileDataExistenceBuilder(UserConsistency user) 
            : base(user)
        {
            _profileDataExistence = _user.ProfileDataExistence ??= new ProfileDataExistence();
        }

        public IProfileDataExistenceBuilder Address(bool exists = true) =>
            Update(data => data.Address = exists);

        public IProfileDataExistenceBuilder AddressDocuments(bool exists = true) =>
            Update(data => data.AddressDocuments = exists);

        public IProfileDataExistenceBuilder DateOfBirth(bool exists = true) =>
            Update(data => data.DateOfBirth = exists);

        public IProfileDataExistenceBuilder FullName(bool exists = true) =>
            Update(data => data.FullName = exists);

        public IProfileDataExistenceBuilder IdDocumentNumber(bool exists = true) =>
            Update(data => data.IdDocumentNumber = exists);

        public IProfileDataExistenceBuilder IdDocumentNumberType(bool exists = true) =>
            Update(data => data.IdDocumentNumberType = exists);

        public IProfileDataExistenceBuilder IdentityDocuments(bool exists = true) =>
            Update(data => data.IdentityDocuments = exists);

        public IProfileDataExistenceBuilder IpAddress(bool exists = true) =>
            Update(data => data.IpAddress = exists);

        public IProfileDataExistenceBuilder Nationality(bool exists = true) =>
            Update(data => data.Nationality = exists);

        public IProfileDataExistenceBuilder ProofOfFundsDocuments(bool exists = true) =>
            Update(data => data.ProofOfFundsDocuments = exists);

        public IProfileDataExistenceBuilder RiskLevel(bool exists = true) =>
            Update(data => data.RiskLevel = exists);

        public IProfileDataExistenceBuilder TaxResidence(bool exists = true) =>
            Update(data => data.TaxResidence = exists);

        public IProfileDataExistenceBuilder Tin(bool exists = true) =>
            Update(data => data.Tin = exists);

        public IProfileDataExistenceBuilder W9Form(bool exists = true) => 
            Update(data => data.W9Form = exists);

        private IProfileDataExistenceBuilder Update(Action<ProfileDataExistence> update)
        {
            update(_profileDataExistence);
            return this;
        }
    }
}
