﻿using RocketInsights.Contextual.Services;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Services;
using System;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent
{
    public class KontentService : ILayoutService, IContentService
    {
        private IContextService ContextService { get; }

        public KontentService(IContextService contextService)
        {
            ContextService = contextService;
        }

        public Task<Composition> GetCompositionAsync()
        {
            if(!ContextService.TryGetContext(out var context))
            {
                throw new Exception("Unable to retrieve a context");
            }

            var composition = new Composition()
            {
                Name = $"This came from a Kontent provider ({context.Culture.DisplayName})."
            };

            return Task.FromResult(composition);
        }

        public Task<Fragment> GetFragmentAsync(string id)
        {
            if (!ContextService.TryGetContext(out var context))
            {
                throw new Exception("Unable to retrieve a context");
            }

            var fragment = new Fragment()
            {
                Id = id,
                Name = $"This came from a Kontent provider ({context.Culture.DisplayName})."
            };

            return Task.FromResult(fragment);
        }
    }
}
