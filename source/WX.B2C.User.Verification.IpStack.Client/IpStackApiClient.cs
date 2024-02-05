// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.IpStack.Client
{
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;
    using Models;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// ipstack offers a powerful, real-time IP to geolocation API capable of
    /// looking up accurate location data and assessing security threats
    /// originating from risky IP addresses.
    /// (https://ipstack.com/documentation)
    /// </summary>
    public partial class IpStackApiClient : ServiceClient<IpStackApiClient>, IIpStackApiClient
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        public System.Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; private set; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        public JsonSerializerSettings DeserializationSettings { get; private set; }

        /// <summary>
        /// Your API Access Key is your unique authentication key used to gain access
        /// to the ipstack API. In order to authenticate with the API, append the
        /// access_key parameter to the API's base URL and set it to your access key
        /// value.
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the IpStackApiClient class.
        /// </summary>
        /// <param name='httpClient'>
        /// HttpClient to be used
        /// </param>
        /// <param name='disposeHttpClient'>
        /// True: will dispose the provided httpClient on calling IpStackApiClient.Dispose(). False: will not dispose provided httpClient</param>
        public IpStackApiClient(HttpClient httpClient, bool disposeHttpClient) : base(httpClient, disposeHttpClient)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the IpStackApiClient class.
        /// </summary>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        public IpStackApiClient(params DelegatingHandler[] handlers) : base(handlers)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the IpStackApiClient class.
        /// </summary>
        /// <param name='rootHandler'>
        /// Optional. The http client handler used to handle http transport.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        public IpStackApiClient(HttpClientHandler rootHandler, params DelegatingHandler[] handlers) : base(rootHandler, handlers)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the IpStackApiClient class.
        /// </summary>
        /// <param name='baseUri'>
        /// Optional. The base URI of the service.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public IpStackApiClient(System.Uri baseUri, params DelegatingHandler[] handlers) : this(handlers)
        {
            if (baseUri == null)
            {
                throw new System.ArgumentNullException("baseUri");
            }
            BaseUri = baseUri;
        }

        /// <summary>
        /// Initializes a new instance of the IpStackApiClient class.
        /// </summary>
        /// <param name='baseUri'>
        /// Optional. The base URI of the service.
        /// </param>
        /// <param name='rootHandler'>
        /// Optional. The http client handler used to handle http transport.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public IpStackApiClient(System.Uri baseUri, HttpClientHandler rootHandler, params DelegatingHandler[] handlers) : this(rootHandler, handlers)
        {
            if (baseUri == null)
            {
                throw new System.ArgumentNullException("baseUri");
            }
            BaseUri = baseUri;
        }

        /// <summary>
        /// An optional partial-method to perform custom initialization.
        ///</summary>
        partial void CustomInitialize();
        /// <summary>
        /// Initializes client properties.
        /// </summary>
        private void Initialize()
        {
            BaseUri = new System.Uri("http://api.ipstack.com");
            SerializationSettings = new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new  List<JsonConverter>
                    {
                        new Iso8601TimeSpanConverter()
                    }
            };
            DeserializationSettings = new JsonSerializerSettings
            {
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                    {
                        new Iso8601TimeSpanConverter()
                    }
            };
            CustomInitialize();
        }
        /// <summary>
        /// The ipstack's primary endpoint is called Standard Lookup and is used to
        /// look up single IPv4 or IPv6 addresses.
        /// </summary>
        /// <param name='ipAddress'>
        /// Any IPv4 or IPv6 address; you can also enter a domain URL to have ipstack
        /// resolve the domain to the underlying IP address.
        /// </param>
        /// <param name='fields'>
        /// Set to your preferred output field(s) according to the Specify Output
        /// Fields section. (https://ipstack.com/documentation#fields).
        /// </param>
        /// <param name='hostname'>
        /// Set to 1 to enable Hostname Lookup.
        /// (https://ipstack.com/documentation#hostname).
        /// </param>
        /// <param name='security'>
        /// Set to 1 to enable the Security module.
        /// (https://ipstack.com/documentation#security).
        /// </param>
        /// <param name='language'>
        /// Set to a 2-letter language code according to the Specify Output Language
        /// section to change output language.
        /// </param>
        /// <param name='callback'>
        /// Specify a JSONP callback function name according to the JSONP Callbacks
        /// section.
        /// </param>
        /// <param name='output'>
        /// Set to json or xml to choose between output formats.
        /// </param>
        /// <param name='customHeaders'>
        /// Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <return>
        /// A response object containing the response body and response headers.
        /// </return>
        public async Task<HttpOperationResponse<IpAddressDetails>> LookupWithHttpMessagesAsync(string ipAddress, string fields = default(string), bool? hostname = default(bool?), bool? security = default(bool?), string language = default(string), string callback = default(string), string output = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (AccessKey == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "this.AccessKey");
            }
            if (ipAddress == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ipAddress");
            }
            // Tracing
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("ipAddress", ipAddress);
                tracingParameters.Add("fields", fields);
                tracingParameters.Add("hostname", hostname);
                tracingParameters.Add("security", security);
                tracingParameters.Add("language", language);
                tracingParameters.Add("callback", callback);
                tracingParameters.Add("output", output);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(_invocationId, this, "Lookup", tracingParameters);
            }
            // Construct URL
            var _baseUrl = BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "{ip_address}").ToString();
            _url = _url.Replace("{ip_address}", System.Uri.EscapeDataString(ipAddress));
            List<string> _queryParameters = new List<string>();
            if (AccessKey != null)
            {
                _queryParameters.Add(string.Format("access_key={0}", System.Uri.EscapeDataString(AccessKey)));
            }
            if (fields != null)
            {
                _queryParameters.Add(string.Format("fields={0}", System.Uri.EscapeDataString(fields)));
            }
            if (hostname != null)
            {
                _queryParameters.Add(string.Format("hostname={0}", System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(hostname, SerializationSettings).Trim('"'))));
            }
            if (security != null)
            {
                _queryParameters.Add(string.Format("security={0}", System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(security, SerializationSettings).Trim('"'))));
            }
            if (language != null)
            {
                _queryParameters.Add(string.Format("language={0}", System.Uri.EscapeDataString(language)));
            }
            if (callback != null)
            {
                _queryParameters.Add(string.Format("callback={0}", System.Uri.EscapeDataString(callback)));
            }
            if (output != null)
            {
                _queryParameters.Add(string.Format("output={0}", System.Uri.EscapeDataString(output)));
            }
            if (_queryParameters.Count > 0)
            {
                _url += "?" + string.Join("&", _queryParameters);
            }
            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new HttpMethod("GET");
            _httpRequest.RequestUri = new System.Uri(_url);
            // Set Headers


            if (customHeaders != null)
            {
                foreach(var _header in customHeaders)
                {
                    if (_httpRequest.Headers.Contains(_header.Key))
                    {
                        _httpRequest.Headers.Remove(_header.Key);
                    }
                    _httpRequest.Headers.TryAddWithoutValidation(_header.Key, _header.Value);
                }
            }

            // Serialize Request
            string _requestContent = null;
            // Send Request
            if (_shouldTrace)
            {
                ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
            }
            cancellationToken.ThrowIfCancellationRequested();
            _httpResponse = await HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            if (_shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
            }
            HttpStatusCode _statusCode = _httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string _responseContent = null;
            if ((int)_statusCode != 200)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", _statusCode));
                if (_httpResponse.Content != null) {
                    _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else {
                    _responseContent = string.Empty;
                }
                ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
                ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
                if (_shouldTrace)
                {
                    ServiceClientTracing.Error(_invocationId, ex);
                }
                _httpRequest.Dispose();
                if (_httpResponse != null)
                {
                    _httpResponse.Dispose();
                }
                throw ex;
            }
            // Create Result
            var _result = new HttpOperationResponse<IpAddressDetails>();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;
            // Deserialize Response
            if ((int)_statusCode == 200)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = SafeJsonConvert.DeserializeObject<IpAddressDetails>(_responseContent, DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    _httpRequest.Dispose();
                    if (_httpResponse != null)
                    {
                        _httpResponse.Dispose();
                    }
                    throw new SerializationException("Unable to deserialize the response.", _responseContent, ex);
                }
            }
            if (_shouldTrace)
            {
                ServiceClientTracing.Exit(_invocationId, _result);
            }
            return _result;
        }

        /// <summary>
        /// The ipstack's endpoint to request data for multiple IPv4 or IPv6 addresses
        /// at the same time.
        /// </summary>
        /// <param name='ipAddresses'>
        /// A comma-separated list of IPv4 or IPv6 addresses; you can also enter a
        /// domain URLs to have ipstack resolve the domains to their underlying IP
        /// addresses. (Maximum allowed values: 50)
        /// </param>
        /// <param name='fields'>
        /// Set to your preferred output field(s) according to the Specify Output
        /// Fields section. (https://ipstack.com/documentation#fields).
        /// </param>
        /// <param name='hostname'>
        /// Set to 1 to enable Hostname Lookup.
        /// (https://ipstack.com/documentation#hostname).
        /// </param>
        /// <param name='security'>
        /// Set to 1 to enable the Security module.
        /// (https://ipstack.com/documentation#security).
        /// </param>
        /// <param name='language'>
        /// Set to a 2-letter language code according to the Specify Output Language
        /// section to change output language.
        /// </param>
        /// <param name='callback'>
        /// Specify a JSONP callback function name according to the JSONP Callbacks
        /// section.
        /// </param>
        /// <param name='output'>
        /// Set to json or xml to choose between output formats.
        /// </param>
        /// <param name='customHeaders'>
        /// Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <return>
        /// A response object containing the response body and response headers.
        /// </return>
        public async Task<HttpOperationResponse<IList<IpAddressDetails>>> BulkLookupWithHttpMessagesAsync(IList<string> ipAddresses, string fields = default(string), bool? hostname = default(bool?), bool? security = default(bool?), string language = default(string), string callback = default(string), string output = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (AccessKey == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "this.AccessKey");
            }
            if (ipAddresses == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ipAddresses");
            }
            // Tracing
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("ipAddresses", ipAddresses);
                tracingParameters.Add("fields", fields);
                tracingParameters.Add("hostname", hostname);
                tracingParameters.Add("security", security);
                tracingParameters.Add("language", language);
                tracingParameters.Add("callback", callback);
                tracingParameters.Add("output", output);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(_invocationId, this, "BulkLookup", tracingParameters);
            }
            // Construct URL
            var _baseUrl = BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "{ip_addresses}").ToString();
            _url = _url.Replace("{ip_addresses}", System.Uri.EscapeDataString(string.Join(",", ipAddresses)));
            List<string> _queryParameters = new List<string>();
            if (AccessKey != null)
            {
                _queryParameters.Add(string.Format("access_key={0}", System.Uri.EscapeDataString(AccessKey)));
            }
            if (fields != null)
            {
                _queryParameters.Add(string.Format("fields={0}", System.Uri.EscapeDataString(fields)));
            }
            if (hostname != null)
            {
                _queryParameters.Add(string.Format("hostname={0}", System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(hostname, SerializationSettings).Trim('"'))));
            }
            if (security != null)
            {
                _queryParameters.Add(string.Format("security={0}", System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(security, SerializationSettings).Trim('"'))));
            }
            if (language != null)
            {
                _queryParameters.Add(string.Format("language={0}", System.Uri.EscapeDataString(language)));
            }
            if (callback != null)
            {
                _queryParameters.Add(string.Format("callback={0}", System.Uri.EscapeDataString(callback)));
            }
            if (output != null)
            {
                _queryParameters.Add(string.Format("output={0}", System.Uri.EscapeDataString(output)));
            }
            if (_queryParameters.Count > 0)
            {
                _url += "?" + string.Join("&", _queryParameters);
            }
            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new HttpMethod("GET");
            _httpRequest.RequestUri = new System.Uri(_url);
            // Set Headers


            if (customHeaders != null)
            {
                foreach(var _header in customHeaders)
                {
                    if (_httpRequest.Headers.Contains(_header.Key))
                    {
                        _httpRequest.Headers.Remove(_header.Key);
                    }
                    _httpRequest.Headers.TryAddWithoutValidation(_header.Key, _header.Value);
                }
            }

            // Serialize Request
            string _requestContent = null;
            // Send Request
            if (_shouldTrace)
            {
                ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
            }
            cancellationToken.ThrowIfCancellationRequested();
            _httpResponse = await HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            if (_shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
            }
            HttpStatusCode _statusCode = _httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string _responseContent = null;
            if ((int)_statusCode != 200)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", _statusCode));
                if (_httpResponse.Content != null) {
                    _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else {
                    _responseContent = string.Empty;
                }
                ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
                ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
                if (_shouldTrace)
                {
                    ServiceClientTracing.Error(_invocationId, ex);
                }
                _httpRequest.Dispose();
                if (_httpResponse != null)
                {
                    _httpResponse.Dispose();
                }
                throw ex;
            }
            // Create Result
            var _result = new HttpOperationResponse<IList<IpAddressDetails>>();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;
            // Deserialize Response
            if ((int)_statusCode == 200)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = SafeJsonConvert.DeserializeObject<IList<IpAddressDetails>>(_responseContent, DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    _httpRequest.Dispose();
                    if (_httpResponse != null)
                    {
                        _httpResponse.Dispose();
                    }
                    throw new SerializationException("Unable to deserialize the response.", _responseContent, ex);
                }
            }
            if (_shouldTrace)
            {
                ServiceClientTracing.Exit(_invocationId, _result);
            }
            return _result;
        }

    }
}