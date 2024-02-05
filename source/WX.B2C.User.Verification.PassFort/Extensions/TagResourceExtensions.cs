using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.PassFort.Client.Models;

namespace WX.B2C.User.Verification.PassFort.Extensions
{
    internal static class TagResourceExtensions
    {
        public static IEnumerable<TagResource> FindByPrefix(this IEnumerable<TagResource> tags, string prefix)
        {
            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            return tags.Where(tag => tag.Name != null && tag.Name.StartsWith(prefix));
        }

        public static IEnumerable<TagResource> FindByTagName(this IEnumerable<TagResource> tags, IEnumerable<string> existingTagNames)
        {
            if (tags == null)
                throw new ArgumentNullException(nameof(tags));
            if (existingTagNames == null)
                throw new ArgumentNullException(nameof(existingTagNames));

            return existingTagNames.Join(
                tags,
                tagName => tagName,
                tag => tag.Name,
                (_, tag) => tag);
        }
    }
}