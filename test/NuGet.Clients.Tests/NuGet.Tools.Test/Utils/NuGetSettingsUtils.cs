// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using NuGet.PackageManagement.UI;
using NuGetVSExtension;
using Xunit;

namespace NuGet.Tools.Test.Utils
{
    internal static class NuGetSettingsUtils
    {
        internal static void AssertAreEquivalent(NuGetSettings expectedSettings, NuGetSettings actualSettings)
        {
            Assert.Equal(expectedSettings.WindowSettings.Count, actualSettings.WindowSettings.Count);

            IReadOnlyList<PropertyInfo> properties = GetSerializableProperties<UserSettings>();

            foreach (string key in expectedSettings.WindowSettings.Keys)
            {
                Assert.True(actualSettings.WindowSettings.ContainsKey(key));

                UserSettings expectedUserSettings = expectedSettings.WindowSettings[key];
                UserSettings actualUserSettings = actualSettings.WindowSettings[key];

                foreach (PropertyInfo property in properties)
                {
                    object expectedValue = property.GetValue(expectedUserSettings);
                    object actualValue = property.GetValue(actualUserSettings);

                    Assert.Equal(expectedValue, actualValue);
                }
            }
        }

        internal static NuGetSettings CreateNuGetSettings()
        {
            var settings = new NuGetSettings();

            var userSettings = new UserSettings()
            {
                SourceRepository = "a",
                ShowPreviewWindow = true,
                ShowDeprecatedFrameworkWindow = false,
                RemoveDependencies = true,
                ForceRemove = false,
                IncludePrerelease = true,
                SelectedFilter = ItemFilter.Installed,
                DependencyBehavior = Resolver.DependencyBehavior.HighestMinor,
                FileConflictAction = ProjectManagement.FileConflictAction.Overwrite,
                OptionsExpanded = true,
                SortPropertyName = "b",
                SortDirection = System.ComponentModel.ListSortDirection.Descending
            };

            settings.WindowSettings.Add("c", userSettings);

            return settings;
        }

        private static IReadOnlyList<PropertyInfo> GetSerializableProperties<T>()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => !property.GetCustomAttributes<JsonIgnoreAttribute>().Any())
                .ToArray();
        }
    }
}
