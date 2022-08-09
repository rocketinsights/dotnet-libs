using RocketInsights.Common.Extensions;
using RocketInsights.Common.Models;
using RocketInsights.Contextual.Services;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Providers.Kontent.ApiRunnerEngine;
using RocketInsights.DXP.Providers.Kontent.Common;
using RocketInsights.DXP.Providers.Kontent.Extensions;
using RocketInsights.DXP.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent
{
    public class ContentService : IContentService
    {
        private IContextService ContextService { get; }

        private IKontentApiEngine KontentApiEngine { get; set; }

        private readonly Dictionary<string, Fragment> processedFragments = new Dictionary<string, Fragment>();

        public ContentService(IContextService contextService, IKontentApiEngine kontentApiEngine)
        {
            ContextService = contextService;
            KontentApiEngine = kontentApiEngine;
        }

        public async Task<Fragment> GetFragmentAsync(string id)
        {
            if (!ContextService.TryGetContext(out _))
                throw new Exception("Unable to retrieve a context");

            if (processedFragments.TryGetValue(id, out var existingFragment))
                return existingFragment;
            else
                processedFragments.Add(id, new Fragment());

            var response = await KontentApiEngine.GetContentItem(id).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(response))
                throw new Exception("Unable to retrieve fragment");

            var content = response.Deserialize<Content>();

            if (!(content.GetValueAsObject("item") is Dictionary<string, object> item))
                return new Fragment();

            var systemDictionary = item.GetValueAsDictionary("system");

            var fragment = new Fragment()
            {
                Id = systemDictionary?.GetValueAsObject("codename")?.ToString(),
                Name = systemDictionary?.GetValueAsObject("name")?.ToString(),
                Guid = new Guid(systemDictionary?.GetValueAsObject("id")?.ToString()),
                Template = new Template()
                {
                    Name = systemDictionary?.GetValueAsObject("type")?.ToString(),
                },
                Content = new Content()
            };

            if (!(item.GetValueAsObject("elements") is Dictionary<string, object> elementsDictionary))
                return fragment;

            var fragmentElements = new Dictionary<string, object>();

            foreach (var rootElement in elementsDictionary)
            {
                fragmentElements = await GetFragmentElements(fragmentElements, rootElement.Value as Dictionary<string, object>, rootElement.Key).ConfigureAwait(false);
            }

            foreach (var fragmentElement in fragmentElements)
            {
                fragment.Content.Add(fragmentElement.Key, fragmentElement.Value);
            }

            processedFragments[id] = fragment;
            return fragment;
        }

        private async Task<Dictionary<string, object>> GetFragmentElements(Dictionary<string, object> fragmentElements,
            Dictionary<string, object>? rootElement,
            string key)
        {
            if (rootElement == null)
                return fragmentElements;

            var elementType = rootElement.GetValueAsObject("type")?.ToString();
            var value = rootElement.GetValueAsObject("value");

            if (value != null && !(value is List<object>) || elementType == ElementTypeConstants.Asset)
            {
                if (elementType == ElementTypeConstants.Asset)
                    fragmentElements.Add(key, rootElement.GetValueAsObject("url") ?? new object());
                else
                    fragmentElements.Add(key, value ?? new object());
            }
            else if (value != null)
                fragmentElements = await ProcessElementArray(value, fragmentElements, key, elementType).ConfigureAwait(false);

            return fragmentElements;
        }

        private async Task<Dictionary<string, object>> ProcessElementArray(object value,
            Dictionary<string, object> fragmentElements,
            string key,
            string? elementType)
        {
            if (!(value is List<object> valueList))
                return fragmentElements;

            if (elementType == ElementTypeConstants.MultipleChoice || elementType == ElementTypeConstants.Taxonomy)
            {
                var values = new List<string>();

                foreach (var item in valueList)
                {
                    var name = (item as Dictionary<string, object>).GetValueAsObject("name");
                    if (name != null)
                        values.Add(name.ToString());
                }

                fragmentElements.Add(key, values);
                return fragmentElements;
            }

            foreach (var item in valueList)
            {
                var fragmentDetails = await GetFragmentAsync(item.ToString()).ConfigureAwait(false);
                if (!fragmentElements.TryGetValue(key, out var keyElement))
                {
                    fragmentElements.Add(key, fragmentDetails);
                    continue;
                }

                if (keyElement.GetType().IsArray)
                {
                    var keyElements = ((IEnumerable)keyElement).Cast<Fragment>().ToList();
                    keyElements.Add(fragmentDetails);
                    fragmentElements[key] = keyElement;
                }
                else
                    fragmentElements[key] = new object[] { keyElement, fragmentDetails };
            }

            return fragmentElements;
        }
    }
}
