﻿using System;
using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.YouTube.v3.Data;
using YouTubeApiWrapper.Interfaces;
using YouTubeCleanupTool;
using YouTubeCleanupTool.Domain;

namespace YouTubeApiWrapper
{
    public class YouTubeApi : IYouTubeApi
    {
        private readonly IMapper _mapper;
        private readonly ICredentialManagerWrapper _credentialManagerWrapper;
        private readonly YouTubeServiceCreatorOptions _youTubeServiceCreatorOptions;
        private IYouTubeServiceWrapper _youTubeServiceWrapper;

        public YouTubeApi([NotNull] IMapper mapper,
            [NotNull] ICredentialManagerWrapper credentialManagerWrapper,
            [NotNull] YouTubeServiceCreatorOptions youTubeServiceCreatorOptions)
        {
            _mapper = mapper;
            _credentialManagerWrapper = credentialManagerWrapper;
            _youTubeServiceCreatorOptions = youTubeServiceCreatorOptions;
        }

        public async IAsyncEnumerable<PlaylistData> GetPlaylists()
        {
            var playlists = await HandleSecretRevocation(async getNewToken => await GetYouTubeWrapper(getNewToken).GetPlaylists());
            
            foreach (var playlist in playlists)
            {
                yield return _mapper.Map<PlaylistData>(playlist);
            }
        }

        public async IAsyncEnumerable<PlaylistItemData> GetPlaylistItems(List<PlaylistData> playlists)
        {
            foreach (var playlist in playlists)
            {
                var playlistItems = await HandleSecretRevocation(async getNewToken => await GetYouTubeWrapper(getNewToken).GetPlaylistItems(playlist.Id));
                
                foreach (var playlistItem in playlistItems)
                {
                    yield return _mapper.Map<PlaylistItemData>(playlistItem);
                }
            }
        }

        public async IAsyncEnumerable<VideoData> GetVideos(List<string> videoIdsToGet)
        {
            foreach (var videoId in videoIdsToGet)
            {
                var video = (await HandleSecretRevocation(async getNewToken
                    => await GetYouTubeWrapper(getNewToken).GetVideos(videoId))).FirstOrDefault();

                if (video == null)
                {
                    yield return new VideoData { Id = videoId, Title = "deleted", IsDeletedFromYouTube = true };
                }
                else
                {
                    yield return _mapper.Map<VideoData>(video);
                }
            }
        }

        public async Task AddVideoToPlaylist(string playlistId, string videoId)
        {
            try
            {
                await GetYouTubeWrapper(false).AddVideoToPlaylist(playlistId, videoId);
            }
            catch (Exception ex)
            {

            }
        }

        private async Task<T> HandleSecretRevocation<T>(Func<bool, Task<T>> methodWhichCouldResultInNoAuthentication)
        {
            try
            {
                return await methodWhichCouldResultInNoAuthentication(false);
            }
            catch (TokenResponseException)
            {
                return await methodWhichCouldResultInNoAuthentication(true);
            }
        }

        private IYouTubeServiceWrapper GetYouTubeWrapper(bool getNewToken)
        {
            return CreateYouTubeService(getNewToken).GetAwaiter().GetResult();
        }

        private async Task<IYouTubeServiceWrapper> CreateYouTubeService(bool getNewToken)
        {
            if (_youTubeServiceWrapper != null && !getNewToken)
                return _youTubeServiceWrapper;

            var apiKey = _credentialManagerWrapper.GetApiKey();
            UserCredential credential;
            using (var stream = new FileStream(_youTubeServiceCreatorOptions.ClientSecretPath, FileMode.Open, FileAccess.Read))
            {
                var installedApp = new AuthorizationCodeInstalledApp(
                    new GoogleAuthorizationCodeFlow(
                        new GoogleAuthorizationCodeFlow.Initializer
                        {
                            ClientSecrets = GoogleClientSecrets.Load(stream).Secrets,
                            Scopes = new List<string> { YouTubeService.Scope.YoutubeReadonly },
                            DataStore = new FileDataStore(_youTubeServiceCreatorOptions.FileDataStoreName)
                        }),
                        new LocalServerCodeReceiver());
                credential = await installedApp.AuthorizeAsync("user", CancellationToken.None);
            }

            if (getNewToken)
            {
                var success = await credential.RefreshTokenAsync(CancellationToken.None);
            }

            // Create the service.
            _youTubeServiceWrapper = new YouTubeServiceWrapper(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                HttpClientInitializer = credential,
                ApplicationName = "Youtube cleanup tool",
            });
            apiKey = null;
            return _youTubeServiceWrapper;
        }
    }
}