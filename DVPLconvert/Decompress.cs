
using System;
using System.IO;
using System.Linq;

public static partial class Program
{
    private static void DecompressDVPLFolderRecursively(string path)
    {
        Console.WriteLine("Starting decompressing folder recursively");
        foreach (string file in Directory.GetFiles(path, "*.dvpl", SearchOption.AllDirectories))
        {
            try
            {
                if (Verbose)
                    Console.WriteLine($"Decompressing {file}");
                DecompressDVPLFile(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    private static void DecompressDVPLFolder(string path)
    {

        Console.WriteLine("Starting decompressing folder");
        foreach (string file in Directory.GetFiles(path, "*.dvpl"))
        {
            try
            {
                if (Verbose)
                    Console.WriteLine($"Decompressing {file}");
                DecompressDVPLFile(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    private static unsafe void DecompressDVPLFile(string path)
    {
        byte[] UncompressedData;

        byte[] DVPLFile = File.ReadAllBytes(path);
        DVPLHeader Header = ByteArrayToStructure<DVPLHeader>(DVPLFile[Range.StartAt(DVPLFile.Length-20)]);
        fixed (byte* ptr = &DVPLFile[0]) if (CRC32.calculate_crc32(ptr, Header.sizeCompressed) != Header.crc32Compressed) throw new Exception("CRC hash mismatch");
        if(Verbose) Console.WriteLine(Header);
        switch (Header.storeType)
        {
            case CompressorType.Lz4HC:
            case CompressorType.Lz4:
                UncompressedData = new byte[Header.sizeUncompressed];
                K4os.Compression.LZ4.LZ4Codec.Decode(DVPLFile, 0, Header.sizeCompressed, UncompressedData, 0, Header.sizeUncompressed);
                if (UncompressedData.Length != Header.sizeUncompressed) throw new Exception("Length is wrong");
                break;
            case CompressorType.None:
                UncompressedData = DVPLFile[new Range(0, Header.sizeCompressed)].ToArray();
                break;
            default: throw new NotImplementedException($"{Header.storeType} arcs are not supported yet");
        }
        File.Delete(path);
        File.WriteAllBytes(path.Replace(".dvpl", String.Empty), UncompressedData);
    }
}