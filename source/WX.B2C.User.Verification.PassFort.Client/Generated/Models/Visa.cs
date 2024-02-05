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
    /// Visa
    /// </summary>
    /// <remarks>
    /// Details about a visa
    /// </remarks>
    public partial class Visa
    {
        /// <summary>
        /// Initializes a new instance of the Visa class.
        /// </summary>
        public Visa()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Visa class.
        /// </summary>
        /// <param name="countryCode">Possible values include: 'AFG', 'ALA',
        /// 'ALB', 'DZA', 'ASM', 'AND', 'AGO', 'AIA', 'ATA', 'ATG', 'ARG',
        /// 'ARM', 'ABW', 'AUS', 'AUT', 'AZE', 'BHS', 'BHR', 'BGD', 'BRB',
        /// 'BLR', 'BEL', 'BLZ', 'BEN', 'BMU', 'BTN', 'BOL', 'BES', 'BIH',
        /// 'BWA', 'BVT', 'BRA', 'IOT', 'BRN', 'BGR', 'BFA', 'BDI', 'KHM',
        /// 'CMR', 'CAN', 'CPV', 'CYM', 'CAF', 'TCD', 'CHL', 'CHN', 'CXR',
        /// 'CCK', 'COL', 'COM', 'COG', 'COD', 'COK', 'CRI', 'CIV', 'HRV',
        /// 'CUB', 'CUW', 'CYP', 'CZE', 'DNK', 'DJI', 'DMA', 'DOM', 'ECU',
        /// 'EGY', 'SLV', 'GNQ', 'ERI', 'EST', 'ETH', 'FLK', 'FRO', 'FJI',
        /// 'FIN', 'FRA', 'GUF', 'PYF', 'ATF', 'GAB', 'GMB', 'GEO', 'DEU',
        /// 'GHA', 'GIB', 'GRC', 'GRL', 'GRD', 'GLP', 'GUM', 'GTM', 'GGY',
        /// 'GIN', 'GNB', 'GUY', 'HTI', 'HMD', 'VAT', 'HND', 'HKG', 'HUN',
        /// 'ISL', 'IND', 'IDN', 'IRN', 'IRQ', 'IRL', 'IMN', 'ISR', 'ITA',
        /// 'JAM', 'JPN', 'JEY', 'JOR', 'KAZ', 'KEN', 'KIR', 'PRK', 'KOR',
        /// 'KWT', 'KGZ', 'LAO', 'LVA', 'LBN', 'LSO', 'LBR', 'LBY', 'LIE',
        /// 'LTU', 'LUX', 'MAC', 'MKD', 'MDG', 'MWI', 'MYS', 'MDV', 'MLI',
        /// 'MLT', 'MHL', 'MTQ', 'MRT', 'MUS', 'MYT', 'MEX', 'FSM', 'MDA',
        /// 'MCO', 'MNG', 'MNE', 'MSR', 'MAR', 'MOZ', 'MMR', 'NAM', 'NRU',
        /// 'NPL', 'NLD', 'NCL', 'NZL', 'NIC', 'NER', 'NGA', 'NIU', 'NFK',
        /// 'MNP', 'NOR', 'OMN', 'PAK', 'PLW', 'PSE', 'PAN', 'PNG', 'PRY',
        /// 'PER', 'PHL', 'PCN', 'POL', 'PRT', 'PRI', 'QAT', 'REU', 'RKS',
        /// 'ROU', 'RUS', 'RWA', 'BLM', 'SHN', 'KNA', 'LCA', 'MAF', 'SPM',
        /// 'VCT', 'WSM', 'SMR', 'STP', 'SAU', 'SEN', 'SRB', 'SYC', 'SLE',
        /// 'SGP', 'SXM', 'SVK', 'SVN', 'SLB', 'SOM', 'ZAF', 'SGS', 'ESP',
        /// 'LKA', 'SDN', 'SUR', 'SSD', 'SJM', 'SWZ', 'SWE', 'CHE', 'SYR',
        /// 'TWN', 'TJK', 'TZA', 'THA', 'TLS', 'TGO', 'TKL', 'TON', 'TTO',
        /// 'TUN', 'TUR', 'TKM', 'TCA', 'TUV', 'UGA', 'UKR', 'ARE', 'GBR',
        /// 'USA', 'UMI', 'URY', 'UZB', 'VUT', 'VEN', 'VNM', 'VGB', 'VIR',
        /// 'WLF', 'ESH', 'XXX', 'YEM', 'ZMB', 'ZWE', 'UNK', 'ZZZ'</param>
        /// <param name="expiryDate">Date when this visa expires, if
        /// any</param>
        /// <param name="name">The name of this visa, as given by the
        /// provider</param>
        /// <param name="entitlement">The type of rights given by this visa.
        /// Possible values include: 'WORK', 'STUDY'</param>
        /// <param name="source">The database that was interrogated to get the
        /// data</param>
        /// <param name="details">Details about the type of visa and
        /// identification (these are different depending on the
        /// country).</param>
        /// <param name="holder">Details about the visa holder</param>
        /// <param name="fieldChecks">These checks ascertain whether the
        /// details extracted from the provider are valid, and match those
        /// submitted on the profile</param>
        public Visa(string countryCode = default(string), string grantDate = default(string), string expiryDate = default(string), string name = default(string), string entitlement = default(string), string source = default(string), IList<NameValueField> details = default(IList<NameValueField>), VisaHolder holder = default(VisaHolder), IList<VisaFieldChecksItem> fieldChecks = default(IList<VisaFieldChecksItem>))
        {
            CountryCode = countryCode;
            GrantDate = grantDate;
            ExpiryDate = expiryDate;
            Name = name;
            Entitlement = entitlement;
            Source = source;
            Details = details;
            Holder = holder;
            FieldChecks = fieldChecks;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'AFG', 'ALA', 'ALB', 'DZA',
        /// 'ASM', 'AND', 'AGO', 'AIA', 'ATA', 'ATG', 'ARG', 'ARM', 'ABW',
        /// 'AUS', 'AUT', 'AZE', 'BHS', 'BHR', 'BGD', 'BRB', 'BLR', 'BEL',
        /// 'BLZ', 'BEN', 'BMU', 'BTN', 'BOL', 'BES', 'BIH', 'BWA', 'BVT',
        /// 'BRA', 'IOT', 'BRN', 'BGR', 'BFA', 'BDI', 'KHM', 'CMR', 'CAN',
        /// 'CPV', 'CYM', 'CAF', 'TCD', 'CHL', 'CHN', 'CXR', 'CCK', 'COL',
        /// 'COM', 'COG', 'COD', 'COK', 'CRI', 'CIV', 'HRV', 'CUB', 'CUW',
        /// 'CYP', 'CZE', 'DNK', 'DJI', 'DMA', 'DOM', 'ECU', 'EGY', 'SLV',
        /// 'GNQ', 'ERI', 'EST', 'ETH', 'FLK', 'FRO', 'FJI', 'FIN', 'FRA',
        /// 'GUF', 'PYF', 'ATF', 'GAB', 'GMB', 'GEO', 'DEU', 'GHA', 'GIB',
        /// 'GRC', 'GRL', 'GRD', 'GLP', 'GUM', 'GTM', 'GGY', 'GIN', 'GNB',
        /// 'GUY', 'HTI', 'HMD', 'VAT', 'HND', 'HKG', 'HUN', 'ISL', 'IND',
        /// 'IDN', 'IRN', 'IRQ', 'IRL', 'IMN', 'ISR', 'ITA', 'JAM', 'JPN',
        /// 'JEY', 'JOR', 'KAZ', 'KEN', 'KIR', 'PRK', 'KOR', 'KWT', 'KGZ',
        /// 'LAO', 'LVA', 'LBN', 'LSO', 'LBR', 'LBY', 'LIE', 'LTU', 'LUX',
        /// 'MAC', 'MKD', 'MDG', 'MWI', 'MYS', 'MDV', 'MLI', 'MLT', 'MHL',
        /// 'MTQ', 'MRT', 'MUS', 'MYT', 'MEX', 'FSM', 'MDA', 'MCO', 'MNG',
        /// 'MNE', 'MSR', 'MAR', 'MOZ', 'MMR', 'NAM', 'NRU', 'NPL', 'NLD',
        /// 'NCL', 'NZL', 'NIC', 'NER', 'NGA', 'NIU', 'NFK', 'MNP', 'NOR',
        /// 'OMN', 'PAK', 'PLW', 'PSE', 'PAN', 'PNG', 'PRY', 'PER', 'PHL',
        /// 'PCN', 'POL', 'PRT', 'PRI', 'QAT', 'REU', 'RKS', 'ROU', 'RUS',
        /// 'RWA', 'BLM', 'SHN', 'KNA', 'LCA', 'MAF', 'SPM', 'VCT', 'WSM',
        /// 'SMR', 'STP', 'SAU', 'SEN', 'SRB', 'SYC', 'SLE', 'SGP', 'SXM',
        /// 'SVK', 'SVN', 'SLB', 'SOM', 'ZAF', 'SGS', 'ESP', 'LKA', 'SDN',
        /// 'SUR', 'SSD', 'SJM', 'SWZ', 'SWE', 'CHE', 'SYR', 'TWN', 'TJK',
        /// 'TZA', 'THA', 'TLS', 'TGO', 'TKL', 'TON', 'TTO', 'TUN', 'TUR',
        /// 'TKM', 'TCA', 'TUV', 'UGA', 'UKR', 'ARE', 'GBR', 'USA', 'UMI',
        /// 'URY', 'UZB', 'VUT', 'VEN', 'VNM', 'VGB', 'VIR', 'WLF', 'ESH',
        /// 'XXX', 'YEM', 'ZMB', 'ZWE', 'UNK', 'ZZZ'
        /// </summary>
        [JsonProperty(PropertyName = "country_code")]
        public string CountryCode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "grant_date")]
        public string GrantDate { get; set; }

        /// <summary>
        /// Gets or sets date when this visa expires, if any
        /// </summary>
        [JsonProperty(PropertyName = "expiry_date")]
        public string ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the name of this visa, as given by the provider
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of rights given by this visa. Possible values
        /// include: 'WORK', 'STUDY'
        /// </summary>
        [JsonProperty(PropertyName = "entitlement")]
        public string Entitlement { get; set; }

        /// <summary>
        /// Gets or sets the database that was interrogated to get the data
        /// </summary>
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets details about the type of visa and identification
        /// (these are different depending on the country).
        /// </summary>
        [JsonProperty(PropertyName = "details")]
        public IList<NameValueField> Details { get; set; }

        /// <summary>
        /// Gets or sets details about the visa holder
        /// </summary>
        [JsonProperty(PropertyName = "holder")]
        public VisaHolder Holder { get; set; }

        /// <summary>
        /// Gets or sets these checks ascertain whether the details extracted
        /// from the provider are valid, and match those submitted on the
        /// profile
        /// </summary>
        [JsonProperty(PropertyName = "field_checks")]
        public IList<VisaFieldChecksItem> FieldChecks { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Holder != null)
            {
                Holder.Validate();
            }
        }
    }
}
