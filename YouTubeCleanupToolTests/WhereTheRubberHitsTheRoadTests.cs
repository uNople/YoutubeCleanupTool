﻿using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.NUnit3;
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
    public class WhereTheRubberHitsTheRoadTests
    {
        [Test, AutoNSubstituteData]
        public async Task When_getting_videos_then_they_are_saved(
            [Frozen] IPersister persister,
            [Frozen] IYouTubeServiceWrapper youtubeServiceWrapper,
            WhereTheRubberHitsTheRoad whereTheRubberHitsTheRoad
            )
        {
            // Setup
            youtubeServiceWrapper.GetPlaylists().Returns(new List<Playlist>());

            // Act
            await foreach (var _ in whereTheRubberHitsTheRoad.GetVideos(new List<PlaylistItem>()))
            { 
            }

            // Assert
            persister.Received(1).SaveData("videosFile.json", Arg.Any<List<Video>>());
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
