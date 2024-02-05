using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.PassFort.Client;
using WX.B2C.User.Verification.PassFort.Client.Models;
using WX.B2C.User.Verification.PassFort.Extensions;

namespace WX.B2C.User.Verification.PassFort
{
    public interface IPassFortTagGateway
    {
        Task AddTagAsync(string profileId, string tagName);

        Task AddOrUpdateTagAsync(string profileId, string tagName, IEnumerable<string> existingTagNames);

        Task RemoveTagAsync(string profileId, string tagId);

        Task RemoveTagByName(string profileId, IEnumerable<string> tagNames);

        Task RemoveTagByPrefix(string profileId, string tagPrefix);
    }

    internal class PassFortTagGateway : BasePassFortGateway, IPassFortTagGateway
    {
        private readonly IPassFortApiClientFactory _clientFactory;

        public PassFortTagGateway(IPassFortApiClientFactory clientFactory, ILogger logger) 
            : base(logger)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task AddTagAsync(string profileId, string tagName)
        {
            var newTag = new TagResource { Name = tagName };

            using var client = _clientFactory.Create();
            _ = await HandleAsync(
                requestFactory: () => (profileId, newTag),
                requestInvoker: client.Tags.AddAsync);
        }

        public async Task AddOrUpdateTagAsync(string profileId, string tagName, IEnumerable<string> existingTagNames)
        {
            using var client = _clientFactory.Create();

            var matchedTags = await HandleAsync(
                requestFactory: () => profileId,
                requestInvoker: client.Profiles.GetAsync,
                responseMapper: profile => profile.Tags.FindByTagName(existingTagNames));

            await RemoveTagsAsync(profileId, matchedTags);
            await AddTagAsync(profileId, tagName);
        }

        public async Task RemoveTagAsync(string profileId, string tagId)
        {
            using var client = _clientFactory.Create();
            _ = await HandleAsync(
                requestFactory: () => (profileId, tagId),
                requestInvoker: client.Tags.DeleteAsync);
        }

        public async Task RemoveTagByPrefix(string profileId, string tagPrefix)
        {
            var client = _clientFactory.Create();

            var matchedTags = await HandleAsync(
                requestFactory: () => profileId,
                requestInvoker: client.Profiles.GetAsync,
                responseMapper: profile => profile.Tags.FindByPrefix(tagPrefix));

            await RemoveTagsAsync(profileId, matchedTags);
        }

        public async Task RemoveTagByName(string profileId, IEnumerable<string> tagNames)
        {
            var client = _clientFactory.Create();

            var matchedTags = await HandleAsync(
                requestFactory: () => profileId,
                requestInvoker: client.Profiles.GetAsync,
                responseMapper: profile => profile.Tags.FindByTagName(tagNames));

            await RemoveTagsAsync(profileId, matchedTags);
        }
        
        private async Task RemoveTagsAsync(string profileId, IEnumerable<TagResource> matchedTags)
        {
            foreach (var passFortTag in matchedTags)
                await RemoveTagAsync(profileId, passFortTag.Id);
        }
    }
}