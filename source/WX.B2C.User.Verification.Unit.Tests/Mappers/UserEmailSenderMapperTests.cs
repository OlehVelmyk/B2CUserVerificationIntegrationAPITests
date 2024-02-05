using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.EmailSender.Mappers;
using WX.EmailSender.Commands.Parameters;

namespace WX.B2C.User.Verification.Unit.Tests.Mappers
{
    [TestFixture]
    public class UserEmailSenderMapperTests
    {
        private IUserEmailSenderMapper _mapper;
        
        [SetUp]
        public void Setup()
        {
            _mapper = new UserEmailSenderMapper();
        }

        [Theory]
        public void Map_ShouldSetEmptyTemplateParameters_WhenTemplateParametersDoesNotExist()
        {
            // arrange
            var senderEmailParametersDto = new Core.Contracts.Dtos.UserEmails.SendEmailParameters()
            {
                Emails = new []{ "abc@gmail.com"},
                FromEmail = "aaa@gmail.com",
                FromName = "from_name",
                TemplateId = "some_template_id"
            };
            var expectedSenderEmailParameters = new SendEmailParameters
            {
                TemplateId = senderEmailParametersDto.TemplateId,
                TemplateParameters = new Dictionary<string, string>(),
                Emails = senderEmailParametersDto.Emails,
                From = senderEmailParametersDto.FromEmail,
                FromName = senderEmailParametersDto.FromName,
            };
            
            // act
            var result = _mapper.Map(senderEmailParametersDto);
            
            // assert
            result.Should().BeEquivalentTo(expectedSenderEmailParameters);
        }
    }
}

