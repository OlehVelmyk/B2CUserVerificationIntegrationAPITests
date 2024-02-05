// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// EventDataAdverseMediaFlag
    /// </summary>
    /// <remarks>
    /// Information related to an adverse media flag event
    /// </remarks>
    [Newtonsoft.Json.JsonObject("ADVERSE_MEDIA_FLAG")]
    public partial class EventDataAdverseMediaFlag : EventData
    {
        /// <summary>
        /// Initializes a new instance of the EventDataAdverseMediaFlag class.
        /// </summary>
        public EventDataAdverseMediaFlag()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the EventDataAdverseMediaFlag class.
        /// </summary>
        /// <param name="matchId">The related match identifier</param>
        /// <param name="gender">gender of match</param>
        /// <param name="deceasedDates">Reported death dates</param>
        /// <param name="deceased">Whether this individual is deceased</param>
        /// <param name="providerName">Name of the verification
        /// provider</param>
        /// <param name="matchName">The name that was matched against</param>
        /// <param name="brandText">Data provider copyright notice</param>
        /// <param name="matchCustomLabel">The provider-specific display name
        /// for this match</param>
        /// <param name="matchDates">Dates of the matched results</param>
        /// <param name="matchDatesData">Dates of the matched sanctions</param>
        /// <param name="matchCountries">Countries of the matched sanctions
        /// relating to national origin</param>
        /// <param name="matchCountriesAddress">Countries of the matched
        /// sanctions relating to location</param>
        /// <param name="matchCountriesData">Countries of the matched sanctions
        /// relating to national origin</param>
        /// <param name="score">The score of the match</param>
        /// <param name="aliases">Aliases used by the matched</param>
        /// <param name="previousNames">Previous names used by the
        /// matched</param>
        /// <param name="associates">Associated to the returned PEPs and
        /// Sanctions</param>
        /// <param name="profileNotes">Profile notes returned by check</param>
        /// <param name="details">More information about the match</param>
        /// <param name="media">Related media</param>
        /// <param name="documents">Documents related to the PEP match</param>
        /// <param name="riskography">Description of PEP or sanctioned persons
        /// background</param>
        /// <param name="sources">Sources used to derive sanctions and PEP
        /// information</param>
        /// <param name="remarks">Remarks about a sanctioned person</param>
        /// <param name="identifications">Identifications such as a
        /// passport</param>
        /// <param name="locations">Locations related to a PEP or sanctioned
        /// entity, such as birth place</param>
        public EventDataAdverseMediaFlag(string matchId = default(string), string gender = default(string), IList<string> deceasedDates = default(IList<string>), bool? deceased = default(bool?), string modifiedDate = default(string), string providerName = default(string), string matchName = default(string), string brandText = default(string), string matchCustomLabel = default(string), IList<string> matchDates = default(IList<string>), IList<DateMatchData> matchDatesData = default(IList<DateMatchData>), IList<string> matchCountries = default(IList<string>), IList<string> matchCountriesAddress = default(IList<string>), IList<CountryMatchData> matchCountriesData = default(IList<CountryMatchData>), double? score = default(double?), IList<string> aliases = default(IList<string>), IList<string> previousNames = default(IList<string>), IList<Associate> associates = default(IList<Associate>), string profileNotes = default(string), IList<MatchDetail> details = default(IList<MatchDetail>), IList<MediaArticle> media = default(IList<MediaArticle>), IList<Document> documents = default(IList<Document>), string riskography = default(string), IList<Source> sources = default(IList<Source>), IList<string> remarks = default(IList<string>), IList<Identification> identifications = default(IList<Identification>), IList<Location> locations = default(IList<Location>))
        {
            MatchId = matchId;
            Gender = gender;
            DeceasedDates = deceasedDates;
            Deceased = deceased;
            ModifiedDate = modifiedDate;
            ProviderName = providerName;
            MatchName = matchName;
            BrandText = brandText;
            MatchCustomLabel = matchCustomLabel;
            MatchDates = matchDates;
            MatchDatesData = matchDatesData;
            MatchCountries = matchCountries;
            MatchCountriesAddress = matchCountriesAddress;
            MatchCountriesData = matchCountriesData;
            Score = score;
            Aliases = aliases;
            PreviousNames = previousNames;
            Associates = associates;
            ProfileNotes = profileNotes;
            Details = details;
            Media = media;
            Documents = documents;
            Riskography = riskography;
            Sources = sources;
            Remarks = remarks;
            Identifications = identifications;
            Locations = locations;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the related match identifier
        /// </summary>
        [JsonProperty(PropertyName = "match_id")]
        public string MatchId { get; set; }

        /// <summary>
        /// Gets or sets gender of match
        /// </summary>
        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets reported death dates
        /// </summary>
        [JsonProperty(PropertyName = "deceased_dates")]
        public IList<string> DeceasedDates { get; set; }

        /// <summary>
        /// Gets or sets whether this individual is deceased
        /// </summary>
        [JsonProperty(PropertyName = "deceased")]
        public bool? Deceased { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modified_date")]
        public string ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets name of the verification provider
        /// </summary>
        [JsonProperty(PropertyName = "provider_name")]
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the name that was matched against
        /// </summary>
        [JsonProperty(PropertyName = "match_name")]
        public string MatchName { get; set; }

        /// <summary>
        /// Gets or sets data provider copyright notice
        /// </summary>
        [JsonProperty(PropertyName = "brand_text")]
        public string BrandText { get; set; }

        /// <summary>
        /// Gets or sets the provider-specific display name for this match
        /// </summary>
        [JsonProperty(PropertyName = "match_custom_label")]
        public string MatchCustomLabel { get; set; }

        /// <summary>
        /// Gets or sets dates of the matched results
        /// </summary>
        [JsonProperty(PropertyName = "match_dates")]
        public IList<string> MatchDates { get; set; }

        /// <summary>
        /// Gets or sets dates of the matched sanctions
        /// </summary>
        [JsonProperty(PropertyName = "match_dates_data")]
        public IList<DateMatchData> MatchDatesData { get; set; }

        /// <summary>
        /// Gets or sets countries of the matched sanctions relating to
        /// national origin
        /// </summary>
        [JsonProperty(PropertyName = "match_countries")]
        public IList<string> MatchCountries { get; set; }

        /// <summary>
        /// Gets or sets countries of the matched sanctions relating to
        /// location
        /// </summary>
        [JsonProperty(PropertyName = "match_countries_address")]
        public IList<string> MatchCountriesAddress { get; set; }

        /// <summary>
        /// Gets or sets countries of the matched sanctions relating to
        /// national origin
        /// </summary>
        [JsonProperty(PropertyName = "match_countries_data")]
        public IList<CountryMatchData> MatchCountriesData { get; set; }

        /// <summary>
        /// Gets or sets the score of the match
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public double? Score { get; set; }

        /// <summary>
        /// Gets or sets aliases used by the matched
        /// </summary>
        [JsonProperty(PropertyName = "aliases")]
        public IList<string> Aliases { get; set; }

        /// <summary>
        /// Gets or sets previous names used by the matched
        /// </summary>
        [JsonProperty(PropertyName = "previous_names")]
        public IList<string> PreviousNames { get; set; }

        /// <summary>
        /// Gets or sets associated to the returned PEPs and Sanctions
        /// </summary>
        [JsonProperty(PropertyName = "associates")]
        public IList<Associate> Associates { get; set; }

        /// <summary>
        /// Gets or sets profile notes returned by check
        /// </summary>
        [JsonProperty(PropertyName = "profile_notes")]
        public string ProfileNotes { get; set; }

        /// <summary>
        /// Gets or sets more information about the match
        /// </summary>
        [JsonProperty(PropertyName = "details")]
        public IList<MatchDetail> Details { get; set; }

        /// <summary>
        /// Gets or sets related media
        /// </summary>
        [JsonProperty(PropertyName = "media")]
        public IList<MediaArticle> Media { get; set; }

        /// <summary>
        /// Gets or sets documents related to the PEP match
        /// </summary>
        [JsonProperty(PropertyName = "documents")]
        public IList<Document> Documents { get; set; }

        /// <summary>
        /// Gets or sets description of PEP or sanctioned persons background
        /// </summary>
        [JsonProperty(PropertyName = "riskography")]
        public string Riskography { get; set; }

        /// <summary>
        /// Gets or sets sources used to derive sanctions and PEP information
        /// </summary>
        [JsonProperty(PropertyName = "sources")]
        public IList<Source> Sources { get; set; }

        /// <summary>
        /// Gets or sets remarks about a sanctioned person
        /// </summary>
        [JsonProperty(PropertyName = "remarks")]
        public IList<string> Remarks { get; set; }

        /// <summary>
        /// Gets or sets identifications such as a passport
        /// </summary>
        [JsonProperty(PropertyName = "identifications")]
        public IList<Identification> Identifications { get; set; }

        /// <summary>
        /// Gets or sets locations related to a PEP or sanctioned entity, such
        /// as birth place
        /// </summary>
        [JsonProperty(PropertyName = "locations")]
        public IList<Location> Locations { get; set; }

    }
}
