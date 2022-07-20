using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Search;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Contentful.Proxy
{
    public class ContentfulProxy : IContentfulProxy
    {
        private IContentfulClient ContentfulClient { get; }

        public ContentfulProxy(IContentfulClient contentfulClient)
        {
            ContentfulClient = contentfulClient;
        }

        //TODO: Handle API errors, including rate limiting

        public async Task<Entry<T>> GetEntryAsync<T>(string entryId, string locale = "*")
        {
            var query = QueryBuilder<Entry<T>>.New;
            query.LocaleIs(locale);
            var entry = await ContentfulClient.GetEntry<Entry<T>>(entryId, query);
            return entry;
        }
    }
}
