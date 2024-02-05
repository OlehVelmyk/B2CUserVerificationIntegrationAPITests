using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface IBridgerCredentialsMapper
    {
        BridgerCredentials Map(BridgerCredentialsDto credentialsDto);
    }

    internal class BridgerCredentialsMapper : IBridgerCredentialsMapper
    {
        public BridgerCredentials Map(BridgerCredentialsDto credentialsDto)
        {
            if (credentialsDto == null)
                throw new ArgumentNullException(nameof(credentialsDto));

            return new BridgerCredentials
            {
                UserId = credentialsDto.UserId,
                EncryptedPassword = credentialsDto.EncryptedPassword
            };
        }
    }
}
