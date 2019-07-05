﻿// ------------------------------------------------------------------------------
// <copyright file="FileContent.cs" company="Xalcon @ mmowned.com-Forum">
// Copyright (c) 2011 Xalcon @ mmowned.com-Forum. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.Drawing.Blp
{
    /// <summary>
    /// Indicates the file format of the image(s) embedded in the <see cref="BlpFile"/>.
    /// </summary>
    internal enum FileContent : uint
    {
        /// <summary>
        /// JPEG Compression (JFIF formatted).
        /// </summary>
        JPG = 0,

        /// <summary>
        /// DirectX Compression or Uncompressed (Palettized).
        /// </summary>
        Direct = 1,
    }
}