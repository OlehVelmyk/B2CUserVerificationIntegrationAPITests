using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Unit.Tests.AdminApi
{
    internal class AccessGroupTests
    {
        /// <summary>
        /// Verify if a combination of AccessGroup`s values are enough to satisfy admin api permissions
        /// https://wirexapp.atlassian.net/wiki/spaces/UACQ/pages/3979968530/U+A+domain+Services+-+Admin+API+permissions#Wx.B2C.Survey
        /// </summary>
        [TestCaseSource(nameof(TestCases))]
        public void ShouldVerifyApiPermissionsCorrectness(AccessGroup[] constraints, AccessGroup[] satisfyingValues)
        {
            // Arrange
            var unsatisfyingValues = Enum.GetNames(typeof(AccessGroup))
                .Select(@enum => @enum.To<AccessGroup>())
                .Except(satisfyingValues);

            // Act & Assert
            using (new AssertionScope())
            {
                foreach (var value in satisfyingValues)
                {
                    var isSatisfied = IsSatisfied(value);
                    isSatisfied.Should().BeTrue($"{value} should have flag {string.Join(',', constraints)}");
                }

                foreach (var value in unsatisfyingValues)
                {
                    var isSatisfied = IsSatisfied(value);
                    isSatisfied.Should().BeFalse($"{value} should not have flag {string.Join(',', constraints)}");
                }
            }

            bool IsSatisfied(AccessGroup input) => constraints.Any(constraint => input.HasFlag(constraint));
        }

        private static TestCaseData[] TestCases =>
            new[]
            {
                new TestCaseData(
                    new[] { AccessGroup.Normal, AccessGroup.OperationControlSecurityLevel },
                    new[]
                    {
                        AccessGroup.Normal,
                        AccessGroup.MiddleSecurityLevel,
                        AccessGroup.RiskSecurityLevel,
                        AccessGroup.ProductionLiveSecurityLevel,
                        AccessGroup.ComplianceSecurityLevel,
                        AccessGroup.ComplianceTopSecurityLevel,
                        AccessGroup.OperationsLevel,
                        AccessGroup.OperationControlSecurityLevel,
                        AccessGroup.ArchitectSecurityLevel,
                        AccessGroup.AlmostTopSecurityLevel,
                        AccessGroup.TopSecurityLevel
                    }),
                new TestCaseData(
                    new[] { AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel },
                    new[]
                    {
                        AccessGroup.ComplianceSecurityLevel,
                        AccessGroup.ComplianceTopSecurityLevel,
                        AccessGroup.OperationControlSecurityLevel,
                        AccessGroup.ArchitectSecurityLevel,
                        AccessGroup.TopSecurityLevel
                    }),
                new TestCaseData(
                    new[] { AccessGroup.OperationControlSecurityLevel },
                    new[]
                    {
                        AccessGroup.OperationControlSecurityLevel,
                        AccessGroup.ArchitectSecurityLevel,
                        AccessGroup.TopSecurityLevel
                    })
            };
    }
}
