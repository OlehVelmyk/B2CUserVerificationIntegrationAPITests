using System;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;

namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal class GbUserInfo : UserInfo
    {
        public GbUserInfo(UserInfo userInfo) : base(userInfo) {  }
    }

    internal class NotGbUserInfo : UserInfo
    {
        public NotGbUserInfo(UserInfo userInfo) : base(userInfo) {  }
    }

    internal class UsUserInfo : UserInfo
    {
        public UsUserInfo(UserInfo userInfo) : base(userInfo) {  }
    }

    internal class NotUsUserInfo : UserInfo
    {
        public NotUsUserInfo(UserInfo userInfo) : base(userInfo) {  }
    }

    internal class EeaUserInfo : UserInfo
    {
        public EeaUserInfo(UserInfo userInfo) : base(userInfo) {  }
    }

    internal class ApacUserInfo : UserInfo
    {
        public ApacUserInfo(UserInfo userInfo) : base(userInfo) {  }
    }

    internal class RoWUserInfo : UserInfo
    {
        public RoWUserInfo(UserInfo userInfo) : base(userInfo) {  }
    }

    internal class GlobalUserInfo : UserInfo
    {
        public GlobalUserInfo(UserInfo userInfo) : base(userInfo) { }
    }

    internal class NotGlobalUserInfo : UserInfo
    {
        public NotGlobalUserInfo(UserInfo userInfo) : base(userInfo) { }
    }

    internal class RuUserInfo : UserInfo
    {
        public RuUserInfo(UserInfo userInfo) : base(userInfo) {  }
    }

    internal class UnsupportedStateUsUserInfo : UserInfo
    {
        public UnsupportedStateUsUserInfo(UserInfo userInfo) : base(userInfo) { }
    }

    internal class UnsupportedUserInfo : UserInfo
    {
        public UnsupportedUserInfo(UserInfo userInfo) : base(userInfo) { }
    }

    internal class UserWithOnboardingTriggersInfo : UserInfo
    {
        public UserWithOnboardingTriggersInfo(UserInfo userInfo) : base(userInfo) { }
    }

    internal class UserWithoutOnboardingTriggersInfo : UserInfo
    {
        public UserWithoutOnboardingTriggersInfo(UserInfo userInfo) : base(userInfo) { }
    }

    internal class UserWithMonitoringTriggersInfo : UserInfo
    {
        public UserWithMonitoringTriggersInfo(UserInfo userInfo) : base(userInfo) { }
    }

    internal class UserWithoutMonitoringTriggersInfo : UserInfo
    {
        public UserWithoutMonitoringTriggersInfo(UserInfo userInfo) : base(userInfo) { }
    }

    internal class EmptyUserInfo : UserInfo
    {  }

    // Test entity (specimen) that represents PersonalDetails + VerificationDetails
    internal class UserInfo
    {
        public UserInfo() { }

        public UserInfo(UserInfo userInfo)
        {
            UserId = userInfo.UserId;
            Email = userInfo.Email;
            DateOfBirth = userInfo.DateOfBirth;
            Policy = userInfo.Policy;
            Nationality = userInfo.Nationality;
            FirstName = userInfo.FirstName;
            LastName = userInfo.LastName;
            Address = userInfo.Address;
            IpAddress = userInfo.IpAddress;
        }

        public Guid UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Address Address { get; set; }

        public VerificationPolicy Policy { get; set; }

        public string Nationality { get; set; }

        public string Email { get; set; }

        public string IpAddress { get; set; }
    }
}
