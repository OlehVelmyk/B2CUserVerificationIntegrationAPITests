using System.Collections.Generic;
using System.IO;
using System.Linq;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Constants;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Other;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Providers
{
    internal static class SourceProvider
    {
        private static Dictionary<string, string> _sources = new();

        public static string GetDecorateFunction(string name, IDictionary<string, object> parameters = null)
        {
            var path = Path.Combine(Global.RootPath, DecorateFunctions.Folder, name);
            return GetSource(path, parameters);
        }

        public static string GetTemplate(string name, IDictionary<string, object> parameters = null)
        {
            var path = Path.Combine(Global.RootPath, Templates.Folder, name);
            return GetSource(path, parameters);
        }

        public static T GetTemplate<T>(string name, IDictionary<string, object> parameters = null)
        {
            var rawSource = GetTemplate(name, parameters);
            return Serializer.Deserialize<T>(rawSource);
        }

        private static string GetSource(string path, IDictionary<string, object> parameters)
        {
            if (_sources.ContainsKey(path))
                return Replace(_sources[path], parameters);

            var source = File.ReadAllText(path);
            _sources.Add(path, source);

            return Replace(source, parameters);
        }

        private static string Replace(string source, IDictionary<string, object> parameters)
        {
            if (parameters is null)
                return source;

            return parameters.Aggregate(
                source,
                (source, pair) => source.Replace(pair.Key, pair.Value.ToString()));
        }
    }
}
