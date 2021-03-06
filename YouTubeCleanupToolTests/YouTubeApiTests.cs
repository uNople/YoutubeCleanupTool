﻿using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.NUnit3;
using AutoMapper;
using Google.Apis.YouTube.v3.Data;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YoutubeCleanupTool;
using YoutubeCleanupTool.Interfaces;

namespace YouTubeCleanupToolTests
{
    [TestFixture]
    public class YouTubeApiTests
    {
        [Test, AutoNSubstituteData]
        public async Task When_getting_videos_then_they_are_saved(
            [Frozen] IYouTubeServiceWrapper youtubeServiceWrapper,
            YouTubeApi youTubeApi
            )
        {
            // Setup
            youtubeServiceWrapper.GetPlaylists().Returns(new List<Playlist>());

            // Act
            await foreach (var _ in youTubeApi.GetVideos(new List<string>()))
            { 
            }

        }

        public class AutoNSubstituteDataAttribute : AutoDataAttribute
        {
            internal AutoNSubstituteDataAttribute()
                : base(() => new Fixture().Customize(new AutoNSubstituteCustomization()))
            {
            }
        }
    }
}
