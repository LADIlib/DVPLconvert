using K4os.Compression.LZ4;
using System;
using System.IO;
using System.Linq;

public static unsafe partial class Program
{
    static void CompressDVPLFolderRecursively(string path)
    {
        Console.WriteLine("Starting compressing folder recursively");
        foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            if (GetFileExtention(file) != "dvpl")
                try
                {
                    if (Verbose)
                        Console.WriteLine($"Compressing {file}");
                    CompressDVPLFile(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
    }
    static void CompressDVPLFolder(string path)
    {
        Console.WriteLine("Starting compressing folder");
        foreach (string file in Directory.GetFiles(path, "*"))
            if (GetFileExtention(file) != "dvpl")
                try
                {
                    if (Verbose)
                        Console.WriteLine($"Compressing {file}");
                    CompressDVPLFile(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
    }
    static void CompressDVPLFile(string path)
    {
        UnmanagedStream stream = new UnmanagedStream(File.OpenRead(path));
        byte[] UncompressedData = stream.ReadValues<byte>(stream.Length);
        stream.Dispose();
        byte[] OutData = new byte[LZ4Codec.MaximumOutputSize(UncompressedData.Length)];
        if (UncompressedData.Length != 0 && Compress)
            fixed (byte* OD = &OutData[0], UD = &UncompressedData[0]) 
            {
                var OutLen = LZ4Codec.Encode(UD, UncompressedData.Length, OD, OutData.Length, LZ4Level.L12_MAX);
                if (OutLen < 0 || OutLen > UncompressedData.Length) throw new Exception($"{path} LZ4 encode failed");
                Array.Resize(ref OutData, OutLen);
            }
        else OutData = UncompressedData;
        DVPLHeader Header = new DVPLHeader()
        {
            sizeCompressed = OutData.Length,
            storeType = OutData.Length == UncompressedData.Length ? cType.None : cType.Lz4HC,
            sizeUncompressed = UncompressedData.Length,
            crc32Compressed = OutData.Length is 0 ? 0 : CalcCRC(OutData),
            marker = 0x4C505644 // LPVD
        };
        if (Verbose) Console.WriteLine(Header);
        byte[] buf = new byte[OutData.Length + 20];
        stream = new UnmanagedStream(buf);
        stream.WriteValues(OutData);
        stream.WriteValue(Header);
        File.Delete(path);
        File.WriteAllBytes(path + ".dvpl", buf);
    }
}