﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using UnitTests.Common;

namespace YouTubeApiWrapperTests
{
    public class AutoNSubstituteDataAttribute : AutoDataAttribute
    {
        public AutoNSubstituteDataAttribute()
            : base(() => new Fixture().Customize(new CompositeCustomization(
                new AutoNSubstituteCustomization() { ConfigureMembers = false }
            )))
        {
        }
    }
}