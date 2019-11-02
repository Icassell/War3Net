// ------------------------------------------------------------------------------
// <copyright file="MpqStream.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;
using System.IO;
using System.Text;

using War3Net.IO.Compression;

namespace War3Net.IO.Mpq
{
    /// <summary>
    /// A Stream based class for reading a file from an <see cref="MpqArchive"/>.
    /// </summary>
    public class MpqStream : Stream
    {
        private readonly Stream _stream;
        private readonly MpqEntry _entry;
        private readonly int _blockSize;
        private readonly MpqStreamMode _mode;
        private readonly uint[] _blockPositions;

        private byte[]? _currentData;
        private long _position;
        private int _currentBlockIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="MpqStream"/> class in Read mode.
        /// </summary>
        /// <param name="archive">The archive from which to load a file.</param>
        /// <param name="entry">The file's entry in the <see cref="BlockTable"/>.</param>
        internal MpqStream(MpqArchive archive, MpqEntry entry)
            : this(entry, archive.BaseStream, archive.BlockSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpqStream"/> class in Read mode.
        /// </summary>
        /// <param name="entry">The file's entry in the <see cref="BlockTable"/>.</param>
        /// <param name="baseStream">The <see cref="MpqArchive"/>'s stream.</param>
        /// <param name="blockSize">The <see cref="MpqArchive.BlockSize"/>.</param>
        internal MpqStream(MpqEntry entry, Stream baseStream, int blockSize)
        {
            _mode = MpqStreamMode.Read;
            _entry = entry;

            _stream = baseStream;
            _blockSize = blockSize;

            if (!_entry.IsSingleUnit)
            {
                _currentBlockIndex = -1;

                // Compressed files start with an array of offsets to make seeking possible
                if (_entry.IsCompressed)
                {
                    var blockposcount = (int)((_entry.FileSize + _blockSize - 1) / _blockSize) + 1;

                    // Files with metadata have an extra block containing block checksums
                    if ((_entry.Flags & MpqFileFlags.FileHasMetadata) != 0)
                    {
                        blockposcount++;
                    }

                    _blockPositions = new uint[blockposcount];

                    lock (_stream)
                    {
                        _stream.Seek((uint)_entry.FilePosition, SeekOrigin.Begin);
                        using (var br = new BinaryReader(_stream, new UTF8Encoding(), true))
                        {
                            for (var i = 0; i < blockposcount; i++)
                            {
                                _blockPositions[i] = br.ReadUInt32();
                            }
                        }
                    }

                    var blockpossize = (uint)blockposcount * 4;

                    /*
                    if (_blockPositions[0] != blockpossize)
                    {
                        // _entry.Flags |= MpqFileFlags.Encrypted;
                        throw new MpqParserException();
                    }
                     */

                    if (_entry.IsEncrypted && blockposcount > 1)
                    {
                        if (_entry.EncryptionSeed == 0)
                        {
                            // This should only happen when the file name is not known.
                            if (!_entry.TryUpdateEncryptionSeed(_blockPositions[0], _blockPositions[1], blockpossize))
                            {
                                throw new MpqParserException("Unable to determine encyption seed");
                            }
                        }

                        StormBuffer.DecryptBlock(_blockPositions, _entry.EncryptionSeed - 1);

                        if (_blockPositions[0] != blockpossize)
                        {
                            throw new MpqParserException("Decryption failed");
                        }

                        if (_blockPositions[1] > _blockSize + blockpossize)
                        {
                            throw new MpqParserException("Decryption failed");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpqStream"/> class in Write mode.
        /// </summary>
        internal MpqStream(MpqFile file)
        {
            throw new NotImplementedException();
        }

        private enum MpqStreamMode
        {
            Read = 0,
            Write = 1,
        }

        /// <inheritdoc/>
        public override bool CanRead => _mode == MpqStreamMode.Read;

        /// <inheritdoc/>
        public override bool CanSeek => _mode == MpqStreamMode.Read;

        /// <inheritdoc/>
        public override bool CanWrite => _mode == MpqStreamMode.Write;

        /// <inheritdoc/>
        public override long Length => _entry.FileSize;

        /// <inheritdoc/>
        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        /// <inheritdoc/>
        public override void Flush()
        {
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!CanSeek)
            {
                throw new NotSupportedException();
            }

            var target = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => Position + offset,
                SeekOrigin.End => Length + offset,
                _ => throw new ArgumentException("Invalid SeekOrigin", nameof(origin)),
            };

            if (target < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Attempted to Seek before the beginning of the stream");
            }

            if (target > Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Attempted to Seek beyond the end of the stream");
            }

            return _position = target;
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotSupportedException("SetLength is not supported");
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead)
            {
                throw new NotSupportedException();
            }

            if (_entry.IsSingleUnit)
            {
                return ReadInternal(buffer, offset, count);
            }

            var toread = count;
            var readtotal = 0;

            while (toread > 0)
            {
                var read = ReadInternal(buffer, offset, toread);
                if (read == 0)
                {
                    break;
                }

                readtotal += read;
                offset += read;
                toread -= read;
            }

            return readtotal;
        }

        /// <inheritdoc/>
        public override int ReadByte()
        {
            if (!CanRead)
            {
                throw new NotSupportedException();
            }

            if (_position >= Length)
            {
                return -1;
            }

            BufferData();
            return _currentData![_entry.IsSingleUnit ? _position++ : (int)(_position++ & (_blockSize - 1))];
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
            {
                throw new NotSupportedException();
            }

            throw new NotImplementedException();
        }

        private static byte[] DecompressMulti(byte[] input, int outputLength)
        {
            using var memoryStream = new MemoryStream(input);
            return GetDecompressionFunction((MpqCompressionType)memoryStream.ReadByte(), outputLength).Invoke(memoryStream);
        }

        private static Func<Stream, byte[]> GetDecompressionFunction(MpqCompressionType compressionType, int outputLength)
        {
            return compressionType switch
            {
                MpqCompressionType.Huffman => HuffmanCoding.Decompress,
                MpqCompressionType.ZLib => (stream) => ZLibCompression.Decompress(stream, outputLength),
                MpqCompressionType.PKLib => (stream) => PKDecompress(stream, outputLength),
                MpqCompressionType.BZip2 => (stream) => BZip2Compression.Decompress(stream, outputLength),
                MpqCompressionType.Lzma => throw new MpqParserException("LZMA compression is not yet supported"),
                MpqCompressionType.Sparse => throw new MpqParserException("Sparse compression is not yet supported"),
                MpqCompressionType.ImaAdpcmMono => (stream) => AdpcmCompression.Decompress(stream, 1),
                MpqCompressionType.ImaAdpcmStereo => (stream) => AdpcmCompression.Decompress(stream, 2),

                MpqCompressionType.Sparse | MpqCompressionType.ZLib => throw new MpqParserException("Sparse compression + Deflate compression is not yet supported"),
                MpqCompressionType.Sparse | MpqCompressionType.BZip2 => throw new MpqParserException("Sparse compression + BZip2 compression is not yet supported"),

                MpqCompressionType.ImaAdpcmMono | MpqCompressionType.Huffman => (stream) => AdpcmCompression.Decompress(HuffmanCoding.Decompress(stream), 1),
                MpqCompressionType.ImaAdpcmMono | MpqCompressionType.PKLib => (stream) => AdpcmCompression.Decompress(PKDecompress(stream, outputLength), 1),

                MpqCompressionType.ImaAdpcmStereo | MpqCompressionType.Huffman => (stream) => AdpcmCompression.Decompress(HuffmanCoding.Decompress(stream), 2),
                MpqCompressionType.ImaAdpcmStereo | MpqCompressionType.PKLib => (stream) => AdpcmCompression.Decompress(PKDecompress(stream, outputLength), 2),

                _ => throw new MpqParserException($"Compression of type 0x{compressionType.ToString("X")} is not yet supported"),
            };
        }

        private static byte[] PKDecompress(Stream data, int expectedLength)
        {
            var b1 = data.ReadByte();
            var b2 = data.ReadByte();
            var b3 = data.ReadByte();
            if (b1 == 0 && b2 == 0 && b3 == 0)
            {
                using (var reader = new BinaryReader(data))
                {
                    var expectedStreamLength = reader.ReadUInt32();
                    if (expectedStreamLength != data.Length)
                    {
                        throw new InvalidDataException("Unexpected stream length value");
                    }

                    if (expectedLength + 8 == expectedStreamLength)
                    {
                        // Assume data is not compressed.
                        return reader.ReadBytes(expectedLength);
                    }

                    var comptype = (MpqCompressionType)reader.ReadByte();
                    if (comptype != MpqCompressionType.ZLib)
                    {
                        throw new NotImplementedException();
                    }

                    return ZLibCompression.Decompress(data, expectedLength);
                }
            }
            else
            {
                data.Seek(-3, SeekOrigin.Current);
                return PKLibCompression.Explode(data, expectedLength);
            }
        }

        private int ReadInternal(byte[] buffer, int offset, int count)
        {
            // OW: avoid reading past the contents of the file
            if (_position >= Length)
            {
                return 0;
            }

            BufferData();

            var localposition = _entry.IsSingleUnit ? _position : (_position & (_blockSize - 1));
            var canRead = (int)(_currentData!.Length - localposition);
            var bytestocopy = canRead > count ? count : canRead;
            if (bytestocopy <= 0)
            {
                return 0;
            }

            Array.Copy(_currentData, localposition, buffer, offset, bytestocopy);

            _position += bytestocopy;
            return bytestocopy;
        }

        private void BufferData()
        {
            if (_entry.IsSingleUnit)
            {
                LoadSingleUnit();
            }
            else
            {
                var requiredblock = (int)(_position / _blockSize);
                if (requiredblock != _currentBlockIndex)
                {
                    var expectedlength = (int)Math.Min(Length - (requiredblock * _blockSize), _blockSize);
                    _currentData = LoadBlock(requiredblock, expectedlength);
                    _currentBlockIndex = requiredblock;
                }
            }
        }

        private byte[] LoadBlock(int blockIndex, int expectedLength)
        {
            uint offset;
            int toread;

            if (_entry.IsCompressed)
            {
                offset = _blockPositions[blockIndex];
                toread = (int)(_blockPositions[blockIndex + 1] - offset);
            }
            else
            {
                offset = (uint)(blockIndex * _blockSize);
                toread = expectedLength;
            }

            offset += _entry.FilePosition!.Value;

            var data = new byte[toread];
            lock (_stream)
            {
                _stream.Seek(offset, SeekOrigin.Begin);
                var read = _stream.Read(data, 0, toread);
                if (read != toread)
                {
                    throw new MpqParserException("Insufficient data or invalid data length");
                }
            }

            if (_entry.IsEncrypted && _entry.FileSize > 3)
            {
                if (_entry.EncryptionSeed == 0)
                {
                    throw new MpqParserException("Unable to determine encryption key");
                }

                StormBuffer.DecryptBlock(data, (uint)(blockIndex + _entry.EncryptionSeed));
            }

            if (_entry.IsCompressed && (toread != expectedLength))
            {
                if (toread > expectedLength)
                {
                    throw new MpqParserException("Block's compressed data is larger than decompressed, so it should have been stored in uncompressed format.");
                }

                data = _entry.Flags.HasFlag(MpqFileFlags.CompressedPK)
                    ? PKLibCompression.Explode(data, expectedLength)
                    : DecompressMulti(data, expectedLength);
            }

            return data;
        }

        private void LoadSingleUnit()
        {
            if (_currentData != null)
            {
                return;
            }

            // Read the entire file into memory
            var filedata = new byte[_entry.CompressedSize!.Value];
            lock (_stream)
            {
                _stream.Seek((uint)_entry.FilePosition, SeekOrigin.Begin);
                var read = _stream.Read(filedata, 0, filedata.Length);
                if (read != filedata.Length)
                {
                    throw new MpqParserException("Insufficient data or invalid data length");
                }
            }

            if (_entry.IsEncrypted && _entry.FileSize > 3)
            {
                if (_entry.EncryptionSeed == 0)
                {
                    throw new MpqParserException("Unable to determine encryption key");
                }

                StormBuffer.DecryptBlock(filedata, _entry.EncryptionSeed);
            }

            _currentData = _entry.CompressedSize == _entry.FileSize
                ? filedata
                : DecompressMulti(filedata, (int)_entry.FileSize);
        }
    }
}