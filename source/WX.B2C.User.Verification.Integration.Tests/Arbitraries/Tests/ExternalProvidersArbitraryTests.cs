using BridgerReference;
using FluentAssertions;
using FsCheck;
using FsCheck.NUnit;
using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.ExternalProviders;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.LexisNexis.Rdp.Client.Models;
using WX.B2C.User.Verification.Onfido.Client.Models;
using PassFortModels = WX.B2C.User.Verification.PassFort.Client.Models;
using static WX.B2C.User.Verification.Integration.Tests.Constants.LexisNexis;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.Tests
{
    internal class ExternalProvidersArbitraryTests
    {
        public ExternalProvidersArbitraryTests()
        {
            Arb.Register<SearchRequestArbitrary>();
            Arb.Register<PepSearchRequestArbitrary>();
            Arb.Register<InitiateWorkflowRequestArbitrary>();
            Arb.Register<OnfidoApplicantArbitrary>();
            Arb.Register<OnfidoApplicantWithForbiddenCharactersArbitrary>();
            Arb.Register<ReferrerArbitrary>();
            Arb.Register<InvalidReferrerArbitrary>();
            Arb.Register<DocumentContainerArbitrary>();
            Arb.Register<LivePhotoContainerArbitrary>();
            Arb.Register<PassFortProfileArbitrary>();
        }

        [Property(MaxTest = 1, Replay = "4130582757416970114,3475829152630444275")]
        public void SearchRequest_ShouldBeDeterministic(Models.SearchRequest actual)
        {
            var name = new InputName
            {
                First = "Hilario",
                Last = "Keebler"
            };
            var address = new InputAddress()
            {
                Type = AddressType.Current,
                Country = "US",
                PostalCode = "99059",
                City = "North Javon",
                StateProvinceDistrict = "IN",
                Street1 = "659 Earnest Ports",
                Street2 = "Suite 005"
            };
            var dobInfo = new InputAdditionalInfo()
            {
                Type = AdditionalInfoType.DOB,
                Date = new Date
                {
                    Year = 1912,
                    Month = 10,
                    Day = 12
                }
            };
            var emailInfo = new InputAdditionalInfo
            {
                Type = AdditionalInfoType.Other,
                Label = "Email",
                Value = "Jada.Beer@yahoo.com"
            };
            var tin = new InputID
            {
                Type = IDType.SSN,
                Number = "509814479"
            };
            var inputRecord = new InputRecord()
            {
                Entity = new InputEntity
                {
                    EntityType = InputEntityType.Individual,
                    Gender = GenderType.Unknown,
                    Name = name,
                    AdditionalInfo = new[] { dobInfo, emailInfo },
                    Addresses = new[] { address },
                    IDs = new[] { tin }
                }
            };
            var expected = new Models.SearchRequest
            {
                SearchNames = new[] 
                {
                    BridgerSearchModes.Pep,
                    BridgerSearchModes.AdverseMedia,
                    BridgerSearchModes.Sanction
                },
                SearchInput = new SearchInput
                {
                    Records = new[] { inputRecord }
                }
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Property(MaxTest = 1)]
        public void PepSearchRequest_ShouldBeDeterministic(PepSearchRequest actual)
        {
            var name = new InputName
            {
                First = HardcodedPersonalData.BridgerPepResultFirstName,
                Last = HardcodedPersonalData.BridgerPepResultLastName
            };
            var address = new InputAddress()
            {
                Type = AddressType.Current,
                Country = "US",
                PostalCode = HardcodedPersonalData.BridgerPepResultZipCode,
                City = HardcodedPersonalData.BridgerPepResultCity,
                StateProvinceDistrict = HardcodedPersonalData.BridgerPepResultState,
                Street1 = HardcodedPersonalData.BridgerPepResultAddressLine
            };
            var dob = DateTime.Parse(HardcodedPersonalData.BridgerPepResultDob);
            var additionalInfo = new InputAdditionalInfo()
            {
                Type = AdditionalInfoType.DOB,
                Date = new Date
                {
                    Year = dob.Year,
                    Month = dob.Month,
                    Day = dob.Day
                }
            };
            var inputRecord = new InputRecord()
            {
                Entity = new InputEntity
                {
                    EntityType = InputEntityType.Individual,
                    Gender = GenderType.Unknown,
                    Name = name,
                    AdditionalInfo = new[] { additionalInfo },
                    Addresses = new[] { address }
                }
            };
            var input = new SearchInput
            {
                Records = new[] { inputRecord }
            };
            var expected = new PepSearchRequest
            {
                SearchNames = new[] { BridgerSearchModes.Pep },
                SearchInput = input
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Property(MaxTest = 1, Replay = "15328387100742607846,1287417375467550099")]
        public void InitiateWorkflowRequest_ShouldBeDeterministic(InitiateWorkflowRequest actual)
        {
            var address = new Address()
            {
                Context = "primary",
                StreetAddress1 = "887 Brett Spur",
                StreetAddress2 = "Suite 857",
                City = "East Carterport",
                State = "VT",
                Zip5 = "05845",
                Country = "US"
            };
            var dateOfBirth = new DateOfBirth
            {
                Year = "1961",
                Month = "12",
                Day = "12"
            };
            var personName = new PersonName
            {
                FirstName = "Darron",
                LastName = "Feil"
            };
            var person = new Person()
            {
                Context = "primary",
                Name = personName,
                Addresses = new[] { address },
                SSN = new Ssn
                {
                    Number = "524730094",
                    Type = "ssn9"
                },
                DateOfBirth = dateOfBirth
            };
            var expected = new InitiateWorkflowRequest
            {
                Type = "Initiate",
                Settings = null,
                Persons = new[] { person }
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Property(MaxTest = 1, Replay = "10816348743440713249,12326161092628713433")]
        public void NewApplicant_ShouldBeDeterministic(NewApplicant actual)
        {
            var expected = new NewApplicant
            {
                FirstName = "Rebeka",
                LastName = "Schuppe",
                Email = "Mabel.Hyatt60@hotmail.com"
            };

            actual.Should().BeEquivalentTo(expected);
        }


        [Property(MaxTest = 1, Replay = "16156318826218750712,13378744367304074363")]
        public void OnfidoApplicantWithForbiddenCharacters_ShouldBeDeterministic(OnfidoApplicantWithForbiddenCharacters actual)
        {
            var expected = new OnfidoApplicantWithForbiddenCharacters
            {
                FirstName = "Simo<ne",
                LastName = "Trant^ow",
                Email = "Aracely_Buckridge@hotmail.com"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Property(MaxTest = 1, Replay = "7576954213182377576,16406771109228858271")]
        public void Referrer_ShouldBeDeterministic(Referrer actual)
        {
            var expected = new Referrer
            {
                Value = "https://zHkFTZq.cSXFC.com/"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Property(MaxTest = 1, Replay = "4871558164328792476,6010175439319960279")]
        public void InvalidReferrer_ShouldBeDeterministic(InvalidReferrer actual)
        {
            var expected = new InvalidReferrer
            {
                Value = "https"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Property(MaxTest = 1, Replay = "307744126056454742,18341825976967493175")]
        public void DocumentContainer_ShouldBeDeterministic(DocumentContainer actual)
        {
            actual.MetaData.OnfidoType.Should().Be("driving_licence");
            actual.File.FileName.Should().Be("sample_driving_licence.png");
            actual.File.ContentType.Should().Be("image/png");
            actual.File.Data.Length.Should().Be(444838);
        }

        [Property(MaxTest = 1, Replay = "1388973953053693965,1676295103922571641")]
        public void LivePhotoContainer_ShouldBeDeterministic(LivePhotoContainer actual)
        {
            actual.MetaData.OnfidoType.Should().BeNull();
            actual.File.FileName.Should().Be("sample_photo2.jpg");
            actual.File.ContentType.Should().Be("image/jpeg");
            actual.File.Data.Length.Should().Be(54630);
        }

        [Property(MaxTest = 1, Replay = "14102600186128709452,15146171855598760005")]
        public void PassFortProfile_ShouldBeDeterministic(PassFortProfile actual)
        {
            var expected = new PassFortProfile
            {
                Role = "INDIVIDUAL_CUSTOMER",
                CollectedData = new PassFortModels.IndividualData
                {
                    EntityType = "INDIVIDUAL",
                    PersonalDetails = new PassFortModels.PersonalDetails
                    {
                        Dob = "2092-02-27",
                        Name = new PassFortModels.FullName
                        {
                            FamilyName = "Price",
                            GivenNames = new List<string> { "Maximo" }
                        }
                    },
                    ContactDetails = new PassFortModels.IndividualDataContactDetails(null, "Lowell_Gottlieb@hotmail.com")
                }
            };

            actual.Should().BeEquivalentTo(expected);
        }
    }
}