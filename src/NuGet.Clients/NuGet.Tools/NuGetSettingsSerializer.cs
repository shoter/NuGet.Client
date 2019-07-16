// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NuGetVSExtension
{
    internal sealed class NuGetSettingsSerializer
    {
        internal ValueTask<NuGetSettings> DeserializeAsync(Stream stream)
        {
            return JsonSerializer.ReadAsync<NuGetSettings>(stream);
        }

        internal Task SerializeAsync(Stream stream, NuGetSettings settings)
        {
            return JsonSerializer.WriteAsync(settings, stream);
        }
    }
}