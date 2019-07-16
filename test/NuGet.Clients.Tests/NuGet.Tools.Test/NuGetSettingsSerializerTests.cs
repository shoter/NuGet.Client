// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Threading.Tasks;
using NuGet.Tools.Test.Utils;
using NuGetVSExtension;
using Xunit;

namespace NuGet.Tools.Test
{
    public class NuGetSettingsSerializerTests
    {
        [Fact]
        public async Task Serialization_WhenDeserializing_Succeeds()
        {
            NuGetSettings expectedSettings = NuGetSettingsUtils.CreateNuGetSettings();

            var serializer = new NuGetSettingsSerializer();

            using (var stream = new MemoryStream())
            {
                await serializer.SerializeAsync(stream, expectedSettings);

                Assert.NotEqual(0, stream.Length);

                stream.Seek(offset: 0, loc: SeekOrigin.Begin);

                NuGetSettings actualSettings = await serializer.DeserializeAsync(stream);

                Assert.NotSame(expectedSettings, actualSettings);
                NuGetSettingsUtils.AssertAreEquivalent(expectedSettings, actualSettings);
            }
        }
    }
}