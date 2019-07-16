// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Threading;
using Moq;
using NuGet.Tools.Test.Utils;
using NuGet.VisualStudio;
using NuGetVSExtension;
using Xunit;

namespace NuGet.Tools.Test
{
    public class SolutionUserOptionsTests
    {
        public SolutionUserOptionsTests()
        {
            if (NuGetUIThreadHelper.JoinableTaskFactory == null)
            {
                var joinableTaskContext = new JoinableTaskContext(Thread.CurrentThread, SynchronizationContext.Current);
                NuGetUIThreadHelper.SetCustomJoinableTaskFactory(joinableTaskContext.Factory);
            }
        }

        [Fact]
        public void SerializationAndDeserialization_Succeeds()
        {
            // Arrange
            var serviceProvider = new Mock<IServiceProvider>();
            var target = new SolutionUserOptions(serviceProvider.Object);
            var settings = NuGetSettingsUtils.CreateNuGetSettings();
            foreach (var kvp in settings.WindowSettings)
            {
                target.AddSettings(kvp.Key, kvp.Value);
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var iStream = new VsIStreamWrapper(memoryStream))
                {
                    // Act
                    var result = target.WriteUserOptions(iStream, null);
                    Assert.Equal(VSConstants.S_OK, result);
                }

                memoryStream.Position = 0;

                using (var iStream = new VsIStreamWrapper(memoryStream))
                {
                    var result = target.ReadUserOptions(iStream, null);
                    Assert.Equal(VSConstants.S_OK, result);
                }
            }

            // Assert
            var newSettings = new NuGetSettings();
            foreach (var kvp in settings.WindowSettings)
            {
                var value = target.GetSettings(kvp.Key);
                Assert.NotSame(kvp.Value, value);
                newSettings.WindowSettings[kvp.Key] = value;
            }

            NuGetSettingsUtils.AssertAreEquivalent(settings, newSettings);
        }
    }
}
