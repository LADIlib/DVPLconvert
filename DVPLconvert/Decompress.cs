
using System;
using System.IO;
using System.Linq;
using K4os.Compression.LZ4;
using UStream;


public static unsafe partial class Program
{
    static void DecompressDVPLFolderRecursively(string path)
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
    static void DecompressDVPLFolder(string path)
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
    static void DecompressDVPLFile(string path)
    {
        UnmanagedStream stream = new UnmanagedStream(File.OpenRead(path));
        stream.Seek(SeekOrigin.End, -20);
        DVPLHeader header = stream.ReadValue<DVPLHeader>();
        if (header.marker != 0x4C505644) throw new Exception("Invalid magic");
        stream.Seek(SeekOrigin.Begin, 0);
        byte[] CompressedData = stream.ReadValues<byte>(header.sizeCompressed);
        stream.Dispose();
        byte[] UncompressedData = new byte[header.sizeUncompressed];
        if (Verbose) Console.WriteLine(header);
        if (UncompressedData.Length!=0&&CompressedData.Length!=0)
            if (CalcCRC(CompressedData) == header.crc32Compressed) 
                fixed (byte* CD = &CompressedData[0], UD = &UncompressedData[0])
                    switch (header.storeType)
                    {
                        case cType.Lz4HC:
                        case cType.Lz4: if (LZ4Codec.Decode(CD, CompressedData.Length, UD, UncompressedData.Length) != header.sizeUncompressed) throw new Exception($"{path} Length is wrong"); break;
                        case cType.None: UncompressedData = CompressedData; break;
                        default: throw new NotImplementedException($"{path} {header.storeType} arcs are not supported yet");
                    }
            else throw new Exception($"{path} CRC hash mismatch");
        File.Delete(path);
        File.WriteAllBytes(path.Replace(".dvpl", String.Empty), UncompressedData);
    }
}