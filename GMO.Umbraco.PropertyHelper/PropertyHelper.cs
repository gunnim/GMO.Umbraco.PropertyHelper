using Our.Umbraco.Vorto.Extensions;
using Our.Umbraco.Vorto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace GMO.Umbraco
{
    /// <summary>
    /// Extension class for <see cref="IPublishedProperty"/>'s
    /// </summary>
    public static class PropertyHelper
    {
        /// <summary>
        /// Gets an object value from property.
        /// Handles Vorto values, <see cref="IPublishedContent"/> values,
        /// <see cref="GuidUdi"/>'s and collections of nodes.
        /// </summary>
        /// <param name="umbracoHelper">Umbraco Helper</param>
        /// <param name="prop">Property to retrieve values from</param>
        /// <param name="content">Parent content of published property</param>
        /// <param name="culture">Culture of vorto property</param>
        /// <param name="forceUrl">Always return Url from <see cref="IPublishedContent"/></param>
        /// <param name="forceKey">Always return <see cref="Guid"/> or Id from <see cref="IPublishedContent"/></param>
        /// <returns></returns>
        public static object GetValueFromProperty(
            this UmbracoHelper umbracoHelper,
            IPublishedProperty prop,
            IPublishedContent content,
            string culture = null,
            bool forceUrl = false,
            bool forceKey = false
        )
        {
            // Has vorto value returns false for empty vorto properties.
            // instead of returning a vorto object with dtdGuid and value null we simply return null val.
            if (IsVortoProperty(content, prop.PropertyTypeAlias))
            {
                if (content.HasVortoValue(prop.PropertyTypeAlias, culture))
                {
                    var vortoValue = content.GetVortoValue(prop.PropertyTypeAlias, culture);
                    if (vortoValue is GuidUdi guidValue)
                    {
                        var nodeValue = umbracoHelper.TypedContent(guidValue);

                        return GetNodeValue(nodeValue, forceUrl, forceKey);
                    }
                    else
                    {
                        return vortoValue;
                    }
                }
                else
                {
                    return null;
                }
            }
            else if (prop.Value is IPublishedContent contentValue)
            {
                return GetNodeValue(contentValue, forceUrl, forceKey);
            }
            else if (prop.Value is IEnumerable<IPublishedContent> contentValueCollection)
            {
                return contentValueCollection.Select(x => GetNodeValue(x, forceUrl, forceKey));
            }
            else if (prop.Value is IHtmlString htmlString)
            {
                return htmlString.ToHtmlString();
            }
            else
            {
                return prop.Value;
            }
        }

        private static object GetNodeValue(IPublishedContent node, bool forceUrl, bool forceKey)
        {
            if (forceUrl)
            {
                return node.Url;
            }
            else if (forceKey)
            {
                return GetKeyOrId(node);
            }
            else if (node.ItemType == PublishedItemType.Media)
            {
                return node.Url;
            }
            else
            {
                return GetKeyOrId(node);
            }
        }

        private static object GetKeyOrId(IPublishedContent node)
        {
            var key = node.GetKey();

            if (key != Guid.Empty)
            {
                return key.ToString();
            }
            else
            {
                return node.Id.ToString();
            }
        }

        private static bool IsVortoProperty(IPublishedContent content, string propertyAlias)
        {
            if (content.HasValue(propertyAlias))
            {
                var prop = content.GetProperty(propertyAlias);
                if (prop.Value is VortoValue) return true;
            }

            return false;
        }
    }
}
