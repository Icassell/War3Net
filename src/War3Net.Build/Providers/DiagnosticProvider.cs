﻿// ------------------------------------------------------------------------------
// <copyright file="DiagnosticProvider.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

using War3Net.Build.Audio;
using War3Net.Build.Environment;
using War3Net.Build.Info;
using War3Net.Build.Widget;

namespace War3Net.Build.Providers
{
    internal static class DiagnosticProvider
    {
        internal static readonly DiagnosticDescriptor MissingMapInfo = CreateMissingMapFileDescriptor("W3N1001", MapInfo.FileName, DiagnosticSeverity.Error);
        internal static readonly DiagnosticDescriptor MissingMapEnvironment = CreateMissingMapFileDescriptor("W3N1002", MapEnvironment.FileName, DiagnosticSeverity.Warning);
        internal static readonly DiagnosticDescriptor MissingMapDoodads = CreateMissingMapFileDescriptor("W3N1003", MapDoodads.FileName, DiagnosticSeverity.Info);
        internal static readonly DiagnosticDescriptor MissingMapUnits = CreateMissingMapFileDescriptor("W3N1004", MapUnits.FileName, DiagnosticSeverity.Info);
        internal static readonly DiagnosticDescriptor MissingMapRegions = CreateMissingMapFileDescriptor("W3N1005", MapRegions.FileName, DiagnosticSeverity.Info);
        internal static readonly DiagnosticDescriptor MissingMapSounds = CreateMissingMapFileDescriptor("W3N1006", MapSounds.FileName, DiagnosticSeverity.Info);

        private static DiagnosticDescriptor CreateMissingMapFileDescriptor(string id, string fileName, DiagnosticSeverity severity)
        {
            return new DiagnosticDescriptor(
                id,
                $"Map should contain a '{fileName}' file.",
                $"Make sure that '{fileName}' gets added when building the map.",
                "Usage",
                severity,
                true,
                null,
                null,
                WellKnownDiagnosticTags.Build);
        }
    }
}