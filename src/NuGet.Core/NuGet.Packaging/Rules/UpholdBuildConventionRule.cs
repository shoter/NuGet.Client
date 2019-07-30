// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Client;
using NuGet.Common;
using NuGet.ContentModel;
using NuGet.Packaging.Core;
using NuGet.RuntimeModel;

namespace NuGet.Packaging.Rules
{
    internal class UpholdBuildConventionRule : IPackageRule
    {
        public string MessageFormat { get; }

        public IEnumerable<PackagingLogMessage> Validate(PackageArchiveReader builder)
        {
            var files = builder.GetFiles().Where(t => t.StartsWith(PackagingConstants.Folders.Build));
            var packageId = builder.NuspecReader.GetId();
            return Validate(files, packageId);
        }

        public IEnumerable<PackagingLogMessage> Validate(IEnumerable<string> files, string packageId)
        {
            var managedCodeConventions = new ManagedCodeConventions(new RuntimeGraph());
            var collection = new ContentItemCollection();
            collection.Load(files.Where(t => PathUtility.GetPathWithDirectorySeparator(t).Count(m => m == Path.DirectorySeparatorChar) > 1));

            var buildItems = ContentExtractor.GetContentForPattern(collection, managedCodeConventions.Patterns.MSBuildFiles);
            var tfms = ContentExtractor.GetGroupFrameworks(buildItems).Select(m => m.GetShortFolderName());

            var conventionViolators = new Dictionary<string, string>();
            var hello = managedCodeConventions.Properties;
            var correctProps = packageId + ".props";
            var correctTargets = packageId + ".targets";

            var filesUnderTFM = files.Select(t => t.Remove(0, (PackagingConstants.Folders.Build + '/').Length));
            var filesUnderBuild = files.Where(t => PathUtility.GetPathWithDirectorySeparator(t).Count(m => m == Path.DirectorySeparatorChar) == 1 && t.StartsWith(PackagingConstants.Folders.Build));
            if (files.Count() != 0)
            {
                if(filesUnderBuild.Count() != 0)
                {
                    var propsFilesUnderBuild = filesUnderBuild.Where(t => t.EndsWith(".props")).Select(t => t.Remove(0, (PackagingConstants.Folders.Build + '/').Length));
                    var targetsFilesUnderBuild = filesUnderBuild.Where(t => t.EndsWith(".props")).Select(t => t.Remove(0, (PackagingConstants.Folders.Build + '/').Length));
                    if(!propsFilesUnderBuild.Contains(correctProps) && propsFilesUnderBuild.Count() > 0)
                    {
                        conventionViolators.Add(PackagingConstants.Folders.Build, correctProps);
                    }

                    if(!targetsFilesUnderBuild.Contains(correctTargets) && targetsFilesUnderBuild.Count() > 0)
                    {
                        conventionViolators.Add(PackagingConstants.Folders.Build, correctTargets);
                    }
                }

                foreach(var tfm in tfms)
                {
                    var propsFiles = filesUnderTFM.Where(t => t.EndsWith(".props") && t.StartsWith(tfm)).Select(t => t.Remove(0, (tfm + '/').Length));
                    var targetsFiles = filesUnderTFM.Where(t => t.EndsWith(".targets") && t.StartsWith(tfm)).Select(t => t.Remove(0, (tfm + '/').Length));

                    if (!propsFiles.Contains(correctProps) && propsFiles.Count() != 0)
                    {
                        conventionViolators.Add(tfm, correctProps);
                    }

                    if (!targetsFiles.Contains(correctTargets) && targetsFiles.Count() != 0)
                    {
                        conventionViolators.Add(tfm, correctTargets);
                    }
                }
            }

            return Array.Empty<PackagingLogMessage>();
        }
    }
}
