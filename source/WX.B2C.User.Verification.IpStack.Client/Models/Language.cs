// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.IpStack.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// An object containing details of language spoken in the country
    /// associated with the IP.
    /// </summary>
    public partial class Language
    {
        /// <summary>
        /// Initializes a new instance of the Language class.
        /// </summary>
        public Language()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Language class.
        /// </summary>
        /// <param name="code">2-letter language code for the given language.
        /// (https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes)</param>
        /// <param name="name">Name (in the API request's main language) of the
        /// given language. (e.g. Portuguese).</param>
        /// <param name="native">Native name of the given language. (e.g.
        /// Portugu�s).</param>
        public Language(string code = default(string), string name = default(string), string native = default(string))
        {
            Code = code;
            Name = name;
            Native = native;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets 2-letter language code for the given language.
        /// (https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes)
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; private set; }

        /// <summary>
        /// Gets name (in the API request's main language) of the given
        /// language. (e.g. Portuguese).
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        /// <summary>
        /// Gets native name of the given language. (e.g. Portugu�s).
        /// </summary>
        [JsonProperty(PropertyName = "native")]
        public string Native { get; private set; }

    }
}