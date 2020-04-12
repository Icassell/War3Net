﻿// ------------------------------------------------------------------------------
// <copyright file="MapAudioTest.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using War3Net.Build.Audio;
using War3Net.Build.Providers;
using War3Net.Common.Testing;

namespace War3Net.Build.Tests
{
    [TestClass]
    public class MapAudioTest
    {
        [DataTestMethod]
        [DynamicData(nameof(GetDefaultAudioFiles), DynamicDataSourceType.Method)]
        public void TestParseMapAudio(string mapSoundsFilePath)
        {
            using var original = FileProvider.GetFile(mapSoundsFilePath);
            using var recreated = new MemoryStream();

            MapSounds.Parse(original, true).SerializeTo(recreated, true);
            StreamAssert.AreEqual(original, recreated, true);
        }

        private static IEnumerable<object[]> GetDefaultAudioFiles()
        {
            return TestDataProvider.GetDynamicData(
                MapSounds.FileName.GetSearchPattern(),
                SearchOption.AllDirectories,
                Path.Combine("Audio"))

            .Concat(TestDataProvider.GetDynamicArchiveData(
                MapSounds.FileName,
                SearchOption.TopDirectoryOnly,
                "Maps"));
        }
    }
}