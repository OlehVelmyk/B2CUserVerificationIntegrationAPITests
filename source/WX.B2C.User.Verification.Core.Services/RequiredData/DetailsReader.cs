using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.XPath;

namespace WX.B2C.User.Verification.Core.Services.RequiredData
{
    internal interface IDetailsReader<in TSource>
    {
        object Read(TSource source, string xPath);
    }

    internal class DetailsReader<TSource> : IDetailsReader<TSource>
    {
        private readonly Dictionary<string, Func<TSource, object>> _readingSchema;

        protected DetailsReader(Dictionary<string, Func<TSource, object>> readingSchema)
        {
            _readingSchema = readingSchema ?? throw new ArgumentNullException(nameof(readingSchema));
        }

        public object Read(TSource source, string xPath)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (xPath == null)
                throw new ArgumentNullException(nameof(xPath));

            if (_readingSchema.TryGetValue(xPath, out var reader))
                return reader(source);

            throw new ArgumentOutOfRangeException(nameof(xPath), xPath, $"Cannot read from {source.GetType().Name} by xPath {xPath}");
        }
    }

    internal class VerificationDetailsReader : DetailsReader<VerificationDetailsDto>
    {
        public VerificationDetailsReader() :
            base(new Dictionary<string, Func<VerificationDetailsDto, object>>
            {
                { XPathes.IpAddress, dto => dto.IpAddress },
                { XPathes.TaxResidence, dto => dto.TaxResidence },
                { XPathes.RiskLevel, dto => dto.RiskLevel },
                { XPathes.IdDocumentNumber, dto => dto.IdDocumentNumber },
                { XPathes.IdDocumentNumber.Type, dto => dto.IdDocumentNumber?.Type },
                { XPathes.IdDocumentNumber.Number, dto => dto.IdDocumentNumber?.Number },
                { XPathes.Tin, dto => dto.Tin },
                { XPathes.Tin.Type, dto => dto.Tin?.Type },
                { XPathes.Tin.Number, dto => dto.Tin?.Number },
                { XPathes.VerifiedNationality, dto => dto.Nationality },
                { XPathes.IsPep, dto => dto.IsPep },
                { XPathes.IsSanctioned, dto => dto.IsSanctioned },
                { XPathes.IsAdverseMedia, dto => dto.IsAdverseMedia },
                { XPathes.Turnover, dto => dto.Turnover },
                { XPathes.PoiIssuingCountry, dto => dto.PoiIssuingCountry },
                { XPathes.PlaceOfBirth, dto => dto.PlaceOfBirth },
                { XPathes.ComprehensiveIndex, dto => dto.ComprehensiveIndex },
                { XPathes.IsIpMatched, dto => dto.IsIpMatched },
                { XPathes.ResolvedCountryCode, dto => dto.ResolvedCountryCode },
            })
        { }
    }

    internal class PersonalDetailsReader : DetailsReader<PersonalDetailsDto>
    {
        public PersonalDetailsReader() :
            base(new Dictionary<string, Func<PersonalDetailsDto, object>>
            {
                { XPathes.FirstName, dto => dto.FirstName },
                { XPathes.LastName, dto => dto.LastName },
                { XPathes.FullName, dto => new FullNameDto { FirstName = dto.FirstName, LastName = dto.LastName} },
                { XPathes.Birthdate, dto => dto.DateOfBirth },
                { XPathes.PersonalNationality, dto => dto.Nationality },
                { XPathes.Email, dto => dto.Email },
                { XPathes.ProfileCreationDate, dto => dto.CreatedAt },
                { XPathes.ResidenceAddress, dto => dto.ResidenceAddress },
                { XPathes.ResidenceAddress.Line1, dto => dto.ResidenceAddress?.Line1 },
                { XPathes.ResidenceAddress.Line2, dto => dto.ResidenceAddress?.Line2 },
                { XPathes.ResidenceAddress.City, dto => dto.ResidenceAddress?.City },
                { XPathes.ResidenceAddress.State, dto => dto.ResidenceAddress?.State },
                { XPathes.ResidenceAddress.Country, dto => dto.ResidenceAddress?.Country },
                { XPathes.ResidenceAddress.ZipCode, dto => dto.ResidenceAddress?.ZipCode },
            })
        { }
    }

}