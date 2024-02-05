using System;
using System.Collections.Generic;
using System.Linq;
using MbDotNet.Enums;
using MbDotNet.Models.Predicates;
using MbDotNet.Models.Predicates.Fields;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Factories
{
    internal class PredicateFactory
    {
        public static DeepEqualsPredicate<HttpPredicateFields> CreateDeepEqualsPredicate(Method? method = null,
                                                                                 string path = null,
                                                                                 object body = null,
                                                                                 IDictionary<string, object> queryParameters = null,
                                                                                 IDictionary<string, object> headers = null,
                                                                                 Dictionary<string, string> formContent = null)
        {
            if (new[] { method, path, body, queryParameters, headers }.All(o => o is null))
                throw new ArgumentNullException();

            var fields = CreateFields(method, path, body, queryParameters, headers, formContent);
            return new DeepEqualsPredicate<HttpPredicateFields>(fields);
        }

        public static EqualsPredicate<HttpPredicateFields> CreateEqualsPredicate(Method? method = null,
                                                                         string path = null,
                                                                         object body = null,
                                                                         IDictionary<string, object> queryParameters = null,
                                                                         IDictionary<string, object> headers = null,
                                                                         Dictionary<string, string> formContent = null)
        {
            if (new[] { method, path, body, queryParameters, headers }.All(o => o is null))
                throw new ArgumentNullException();

            var fields = CreateFields(method, path, body, queryParameters, headers, formContent);
            return new EqualsPredicate<HttpPredicateFields>(fields);
        }

        public static MatchesPredicate<HttpPredicateFields> CreateMatchesPredicate(Method? method = null,
                                                                                   string path = null,
                                                                                   object body = null,
                                                                                   IDictionary<string, object> queryParameters = null,
                                                                                   IDictionary<string, object> headers = null,
                                                                                   Dictionary<string, string> formContent = null)
        {
            if (new[] { method, path, body, queryParameters, headers }.All(o => o is null))
                throw new ArgumentNullException();

            var fields = CreateFields(method, path, body, queryParameters, headers, formContent);
            return new MatchesPredicate<HttpPredicateFields>(fields);
        }

        public static MatchesPredicate<HttpBooleanPredicateFields> CreateBooleanPredicate(bool? method = null,
                                                                                          bool? path = null,
                                                                                          bool? body = null,
                                                                                          bool? queryParameters = null,
                                                                                          bool? headers = null)
        {
            if (new[] { method, path, body, queryParameters, headers }.All(b => !b.HasValue))
                throw new ArgumentNullException();

            return new MatchesPredicate<HttpBooleanPredicateFields>(new HttpBooleanPredicateFields
            {
                Method = method,
                Path = path,
                RequestBody = body,
                QueryParameters = queryParameters,
                Headers = headers
            });
        }

        public static OrPredicate CreateOrPredicate(IEnumerable<PredicateBase> predicates)
        {
            if (predicates.IsNullOrEmpty()) throw new ArgumentNullException(nameof(predicates));
            return new OrPredicate(predicates);
        }

        public static HttpPredicateFields CreateFields(Method? method = null,
                                                       string path = null,
                                                       object body = null,
                                                       IDictionary<string, object> queryParameters = null,
                                                       IDictionary<string, object> headers = null,
                                                       Dictionary<string, string> formContent = null) =>
            new HttpPredicateFields
            {
                Method = method,
                Path = path,
                RequestBody = body,
                QueryParameters = queryParameters,
                Headers = headers,
                FormContent = formContent
            };        
    }
}
