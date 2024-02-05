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
    /// An object containing security-related data.
    /// </summary>
    public partial class Security
    {
        /// <summary>
        /// Initializes a new instance of the Security class.
        /// </summary>
        public Security()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Security class.
        /// </summary>
        /// <param name="isProxy">True or false depending on whether or not the
        /// given IP is associated with a proxy.</param>
        /// <param name="proxyType">Type of proxy the IP is associated with.
        /// Possible values include: 'cgi', 'web', 'vpn'</param>
        /// <param name="isCrawler">True or false depending on whether or not
        /// the given IP is associated with a crawler.</param>
        /// <param name="crawlerName">Name of the crawler the IP is associated
        /// with.</param>
        /// <param name="crawlerType">Type of crawler the IP is associated
        /// with. Possible values include: 'unrecognized', 'search_engine_bot',
        /// 'site_monitor', 'screenshot_creator', 'link_checker',
        /// 'wearable_computer', 'web_scraper', 'vulnerability_scanner',
        /// 'virus_scanner', 'speed_tester', 'feed_fetcher', 'tool',
        /// 'marketing'</param>
        /// <param name="isTor">True or false depending on whether or not the
        /// given IP is associated with the anonymous Tor system.</param>
        /// <param name="threatLevel">Type of threat level the IP is associated
        /// with. Possible values include: 'low', 'medium', 'high'</param>
        /// <param name="threatTypes">An object containing all threat types
        /// associated with the IP. Possible values include: 'tor',
        /// 'fake_crawler', 'web_scraper', 'attack_source',
        /// 'attack_source_http', 'attack_source_mail',
        /// 'attack_source_ssh'</param>
        public Security(bool isProxy = default(bool), ProxyType? proxyType = default(ProxyType?), bool isCrawler = default(bool), string crawlerName = default(string), CrawlerType? crawlerType = default(CrawlerType?), bool? isTor = default(bool?), ThreatLevel? threatLevel = default(ThreatLevel?), ThreatType? threatTypes = default(ThreatType?))
        {
            IsProxy = isProxy;
            ProxyType = proxyType;
            IsCrawler = isCrawler;
            CrawlerName = crawlerName;
            CrawlerType = crawlerType;
            IsTor = isTor;
            ThreatLevel = threatLevel;
            ThreatTypes = threatTypes;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets true or false depending on whether or not the given IP is
        /// associated with a proxy.
        /// </summary>
        [JsonProperty(PropertyName = "is_proxy")]
        public bool IsProxy { get; private set; }

        /// <summary>
        /// Gets type of proxy the IP is associated with. Possible values
        /// include: 'cgi', 'web', 'vpn'
        /// </summary>
        [JsonProperty(PropertyName = "proxy_type")]
        public ProxyType? ProxyType { get; private set; }

        /// <summary>
        /// Gets true or false depending on whether or not the given IP is
        /// associated with a crawler.
        /// </summary>
        [JsonProperty(PropertyName = "is_crawler")]
        public bool IsCrawler { get; private set; }

        /// <summary>
        /// Gets name of the crawler the IP is associated with.
        /// </summary>
        [JsonProperty(PropertyName = "crawler_name")]
        public string CrawlerName { get; private set; }

        /// <summary>
        /// Gets type of crawler the IP is associated with. Possible values
        /// include: 'unrecognized', 'search_engine_bot', 'site_monitor',
        /// 'screenshot_creator', 'link_checker', 'wearable_computer',
        /// 'web_scraper', 'vulnerability_scanner', 'virus_scanner',
        /// 'speed_tester', 'feed_fetcher', 'tool', 'marketing'
        /// </summary>
        [JsonProperty(PropertyName = "crawler_type")]
        public CrawlerType? CrawlerType { get; private set; }

        /// <summary>
        /// Gets true or false depending on whether or not the given IP is
        /// associated with the anonymous Tor system.
        /// </summary>
        [JsonProperty(PropertyName = "is_tor")]
        public bool? IsTor { get; private set; }

        /// <summary>
        /// Gets type of threat level the IP is associated with. Possible
        /// values include: 'low', 'medium', 'high'
        /// </summary>
        [JsonProperty(PropertyName = "threat_level")]
        public ThreatLevel? ThreatLevel { get; private set; }

        /// <summary>
        /// Gets an object containing all threat types associated with the IP.
        /// Possible values include: 'tor', 'fake_crawler', 'web_scraper',
        /// 'attack_source', 'attack_source_http', 'attack_source_mail',
        /// 'attack_source_ssh'
        /// </summary>
        [JsonProperty(PropertyName = "threat_types")]
        public ThreatType? ThreatTypes { get; private set; }

    }
}