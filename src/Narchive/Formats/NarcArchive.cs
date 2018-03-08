using Narchive.Exceptions;
using Narchive.IO;
using Narchive.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Narchive.Formats
{
    public class NarcArchive
    {
        public static void Create(string inputPath, string outputPath, bool hasFilenames = true)
        {
            var rootDirectory = new NarcArchiveDirectoryEntry
            {
                ParentDirectoryIndex = -1,
                NameEntryOffset = 0,
            };
            var directories = new List<NarcArchiveDirectoryEntry>
            {
                rootDirectory,
            };
            var files = new List<NarcArchiveFileEntry>();

            var directoryIndex = 1; // Root is 0
            var fileIndex = 0;
            var position = 0;
            for (var i = 0; i < directories.Count; i++)
            {
                var directory = directories[i];

                var directoryNames = Directory.EnumerateDirectories(Path.Combine(inputPath, directory.FullName));
                foreach (var directoryName in directoryNames)
                {
                    var directoryEntry = new NarcArchiveDirectoryEntry
                    {
                        Index = directoryIndex,
                        ParentDirectoryIndex = i,
                        DirectoryName = directory.FullName,
                        Name = Path.GetFileName(directoryName), // Technically returns just the part of the directory name we need
                        FirstFileIndex = fileIndex,
                    };

                    directories.Add(directoryEntry);
                    directory.Entries.Add(directoryEntry);

                    directoryIndex++;
                }

                var fileNames = Directory.EnumerateFiles(Path.Combine(inputPath, directory.FullName));
                foreach (var fileName in fileNames)
                {
                    var fileInfo = new FileInfo(fileName);
                    var length = (int)fileInfo.Length;

                    var fileEntry = new NarcArchiveFileEntry
                    {
                        DirectoryIndex = i,
                        DirectoryName = directory.FullName,
                        Name = Path.GetFileName(fileName),
                        Offset = position,
                        Length = length,
                    };

                    position += ((length + 3) / 4) * 4; // Offsets must be a multiple of 4

                    files.Add(fileEntry);
                    directory.Entries.Add(fileEntry);

                    fileIndex++;
                }
            }
            var fimgLength = position;

            using (var output = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(output))
            {
                try
                {
                    // Write out the NARC header
                    writer.Write((byte)'N');
                    writer.Write((byte)'A');
                    writer.Write((byte)'R');
                    writer.Write((byte)'C');
                    writer.Write((byte)0xFE);
                    writer.Write((byte)0xFF);
                    writer.Write((byte)0);
                    writer.Write((byte)1);

                    writer.Write(0); // This will be written to later

                    writer.Write((short)16); // Header length (always 16)
                    writer.Write((short)3); // Number of sections (always 3)

                    // Write out the FATB section
                    writer.Write((byte)'B');
                    writer.Write((byte)'T');
                    writer.Write((byte)'A');
                    writer.Write((byte)'F');

                    writer.Write(12 + (files.Count * 8)); // Section length
                    writer.Write(files.Count); // Number of file entries

                    foreach (var file in files)
                    {
                        writer.Write(file.Offset); // Start position
                        writer.Write(file.Offset + file.Length); // End position
                    }

                    // Write out the FNTB section
                    writer.Write((byte)'B');
                    writer.Write((byte)'T');
                    writer.Write((byte)'N');
                    writer.Write((byte)'F');

                    if (hasFilenames)
                    {
                        var fntbPosition = (int)output.Position - 4;

                        writer.Write(0); // This will be written to later

                        writer.Write(0); // This will be written to later
                        writer.Write((short)0); // Always 0
                        writer.Write((short)directories.Count); // Number of directories, including the root directory

                        for (var i = 1; i < directories.Count; i++)
                        {
                            writer.Write(0); // This will be written to later
                            writer.Write((short)directories[i].FirstFileIndex); // Index of the first file in this directory
                            writer.Write((short)(directories[i].ParentDirectoryIndex | 0xF000)); // Parent directory
                        }

                        position = directories.Count * 8;
                        foreach (var directory in directories)
                        {
                            directory.NameEntryOffset = position;

                            foreach (var entry in directory.Entries)
                            {
                                var nameAsBytes = Encoding.UTF8.GetBytes(entry.Name);

                                if (entry is NarcArchiveDirectoryEntry)
                                {
                                    writer.Write((byte)(nameAsBytes.Length | 0x80)); // Length of the directory name
                                    writer.Write(nameAsBytes);
                                    writer.Write((short)((entry as NarcArchiveDirectoryEntry).Index | 0xF000));

                                    position += nameAsBytes.Length + 3;
                                }
                                else
                                {
                                    writer.Write((byte)nameAsBytes.Length); // Length of the file name
                                    writer.Write(nameAsBytes);

                                    position += nameAsBytes.Length + 1;
                                }
                            }

                            writer.Write((byte)0);

                            position++;
                        }

                        while (output.Length % 4 != 0)
                        {
                            writer.Write((byte)0xFF);
                        }

                        var fntbLength = (int)output.Position - fntbPosition;

                        // Go back and write the name entry offsets for each directory
                        output.Position = fntbPosition + 4;
                        writer.Write(fntbLength);
                        foreach (var directory in directories)
                        {
                            writer.Write(directory.NameEntryOffset);
                            output.Position += 4;
                        }
                        output.Position = fntbPosition + fntbLength;
                    }
                    else
                    {
                        // The FNTB section is always the same if there are no filenames
                        writer.Write(16); // Section length (always 16)
                        writer.Write(4); // Always 4
                        writer.Write((short)0); // First file index (always 0)
                        writer.Write((short)1); // Directory count (always 1)
                    }

                    // Write out the FIMG section
                    writer.Write((byte)'G');
                    writer.Write((byte)'M');
                    writer.Write((byte)'I');
                    writer.Write((byte)'F');

                    writer.Write(fimgLength + 8); // Section length

                    foreach (var file in files)
                    {
                        using (var input = new FileStream(Path.Combine(inputPath, file.FullName), FileMode.Open, FileAccess.Read))
                        {
                            input.CopyTo(output);
                        }

                        while (output.Length % 4 != 0)
                        {
                            writer.Write((byte)0xFF);
                        }
                    }

                    // Go back and write out the file length
                    output.Position = 8;
                    writer.Write((int)output.Length); // File length
                    output.Position = output.Length;
                }
                catch (Exception e)
                {
                    output.SetLength(0);
                    throw e;
                }
            }
        }

        public static void Extract(string inputPath, string outputPath)
        {
            using (var input = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(input))
            {
                // Read the NARC header
                if (!(reader.ReadByte() == 'N'
                    && reader.ReadByte() == 'A'
                    && reader.ReadByte() == 'R'
                    && reader.ReadByte() == 'C'
                    && reader.ReadByte() == 0xFE
                    && reader.ReadByte() == 0xFF
                    && reader.ReadByte() == 0
                    && reader.ReadByte() == 1
                    && reader.ReadInt32() == input.Length))
                {
                    throw new InvalidFileTypeException(string.Format(ErrorMessages.NotANarcFile, Path.GetFileName(inputPath)));
                }

                var headerLength = reader.ReadInt16();
                var fatbPosition = headerLength;

                // Read the FATB section
                input.Position = fatbPosition;
                if (!(reader.ReadByte() == 'B'
                    && reader.ReadByte() == 'T'
                    && reader.ReadByte() == 'A'
                    && reader.ReadByte() == 'F'))
                {
                    throw new InvalidFileTypeException(string.Format(ErrorMessages.NotANarcFile, Path.GetFileName(inputPath)));
                }

                var fatbLength = reader.ReadInt32();
                var fntbPosition = fatbPosition + fatbLength;

                var fileEntryCount = reader.ReadInt32();
                var fileEntries = new List<NarcArchiveFileEntry>(fileEntryCount);
                var entries = new List<NarcArchiveEntry>(fileEntryCount);
                for (var i = 0; i < fileEntryCount; i++)
                {
                    var offset = reader.ReadInt32();
                    var length = reader.ReadInt32() - offset;
                    fileEntries.Add(new NarcArchiveFileEntry
                    {
                        Offset = offset,
                        Length = length,
                    });
                }

                // Read the FNTB section
                input.Position = fntbPosition;
                if (!(reader.ReadByte() == 'B'
                    && reader.ReadByte() == 'T'
                    && reader.ReadByte() == 'N'
                    && reader.ReadByte() == 'F'))
                {
                    throw new InvalidFileTypeException(string.Format(ErrorMessages.NotANarcFile, Path.GetFileName(inputPath)));
                }

                var fntbLength = reader.ReadInt32();
                var fimgPosition = fntbPosition + fntbLength;

                var rootNameEntryOffset = reader.ReadInt32();
                var rootFirstFileIndex = reader.ReadInt16();
                var rootDirectoryEntry = new NarcArchiveDirectoryEntry
                {
                    Index = 0,
                    ParentDirectoryIndex = -1,
                    NameEntryOffset = rootNameEntryOffset,
                    FirstFileIndex = rootFirstFileIndex,
                };

                var directoryEntryCount = reader.ReadInt16(); // This includes the root directory
                var directoryEntries = new List<NarcArchiveDirectoryEntry>(directoryEntryCount)
                {
                    rootDirectoryEntry,
                };

                if (rootNameEntryOffset != 4)
                {
                    // This NARC contains filenames and directory names, so read them
                    for (var i = 1; i < directoryEntryCount; i++)
                    {
                        var nameEntryTableOffset = reader.ReadInt32();
                        var firstFileIndex = reader.ReadInt16();
                        var parentDirectoryIndex = reader.ReadInt16() & 0xFFF;

                        directoryEntries.Add(new NarcArchiveDirectoryEntry
                        {
                            Index = i,
                            ParentDirectoryIndex = parentDirectoryIndex,
                            NameEntryOffset = nameEntryTableOffset,
                            FirstFileIndex = firstFileIndex,
                        });
                    }

                    var currentDirectory = rootDirectoryEntry;
                    var directoryIndex = 0;
                    var fileIndex = 0;
                    while (directoryIndex < directoryEntryCount)
                    {
                        var entryNameLength = reader.ReadByte();
                        if ((entryNameLength & 0x80) != 0)
                        {
                            // This is a directory name entry
                            var entryName = reader.ReadString(entryNameLength & 0x7F);
                            var entryDirectoryIndex = reader.ReadInt16() & 0xFFF;
                            var directoryEntry = directoryEntries[entryDirectoryIndex];

                            directoryEntry.DirectoryName = directoryEntries[directoryEntry.ParentDirectoryIndex].FullName;
                            directoryEntry.Name = entryName;
                        }
                        else if (entryNameLength != 0)
                        {
                            // This is a file name entry
                            var entryName = reader.ReadString(entryNameLength);
                            var fileEntry = fileEntries[fileIndex];

                            fileEntry.DirectoryIndex = directoryIndex;
                            fileEntry.DirectoryName = currentDirectory.FullName;
                            fileEntry.Name = entryName;

                            entries.Add(new NarcArchiveEntry(input, fileEntry.FullName, fimgPosition + 8 + fileEntry.Offset, fileEntry.Length));

                            fileIndex++;
                        }
                        else
                        {
                            // This is the end of a directory
                            directoryIndex++;
                            if (directoryIndex >= directoryEntryCount)
                            {
                                break;
                            }
                            currentDirectory = directoryEntries[directoryIndex];
                        }
                    }
                }
                else
                {
                    // This NARC doesn't contain filenames and directory names, so just copy fileEntries over to entries
                    var index = 0;
                    var numOfDigits = Math.Floor(Math.Log10(fileEntryCount) + 1);
                    foreach (var fileEntry in fileEntries)
                    {
                        var outputFilename = $"{Path.GetFileNameWithoutExtension(inputPath)}_{index.ToString($"D{numOfDigits}")}";
                        entries.Add(new NarcArchiveEntry(input, outputFilename, fimgPosition + 8 + fileEntry.Offset, fileEntry.Length));

                        index++;
                    }
                }

                // Read the FIMG section
                input.Position = fimgPosition;
                if (!(reader.ReadByte() == 'G'
                    && reader.ReadByte() == 'M'
                    && reader.ReadByte() == 'I'
                    && reader.ReadByte() == 'F'))
                {
                    throw new InvalidFileTypeException(string.Format(ErrorMessages.NotANarcFile, Path.GetFileName(inputPath)));
                }

                foreach (var entry in entries)
                {
                    var entryOutputPath = Path.Combine(outputPath, entry.FullName);
                    var entryOutputDirectory = Path.GetDirectoryName(entryOutputPath);
                    if (!Directory.Exists(entryOutputDirectory))
                    {
                        Directory.CreateDirectory(entryOutputDirectory);
                    }

                    using (var output = new FileStream(entryOutputPath, FileMode.Create, FileAccess.Write))
                    {
                        entry.Open().CopyTo(output);
                    }
                }
            }
        }
    }
}
