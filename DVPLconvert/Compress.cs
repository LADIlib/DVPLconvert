using K4os.Compression.LZ4;
using System;
using System.IO;
using System.Linq;

public static partial class Program
{
    static void CompressDVPLFolderRecursively(string path)
    {
        Console.WriteLine("Starting compressing folder recursively");
        foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
        {
            var ext = GetFileExtention(file);
            if (ext != "dvpl")
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
    }
    static void CompressDVPLFolder(string path)
    {
        Console.WriteLine("Starting compressing folder");
        foreach (string file in Directory.GetFiles(path, "*"))
        {
            var ext = GetFileExtention(file);
            if (ext!="dvpl")
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
    }
    static unsafe void CompressDVPLFile(string path)
    {
        byte[] Data = File.ReadAllBytes(path);
        if (Data == null||Data.Length<0) throw new ArgumentNullException(nameof(Data));
        byte[] OutData;
        DVPLHeader Header;
        if (Compress)
        {
            OutData = new byte[LZ4Codec.MaximumOutputSize(Data.Length)];
            var OutLen = LZ4Codec.Encode(Data, OutData, LZ4Level.L12_MAX);
            if (OutLen < 0) throw new Exception("LZ4 encode failed");
            Array.Resize(ref OutData, OutLen);
            if (OutLen > Data.Length) throw new Exception("WTF?");
            Header = new DVPLHeader()
            {
                sizeCompressed = OutData.Length,
                sizeUncompressed = Data.Length,
                storeType = OutData.Length == Data.Length ? CompressorType.None : CompressorType.Lz4HC,
                crc32Compressed = CalcCRC(OutData),
                marker = "DVPL".ToCharArray()
            };
        }
        else
        {
            OutData = Data;
            Header = new DVPLHeader()
            {
                sizeCompressed = Data.Length,
                sizeUncompressed = Data.Length,
                storeType = CompressorType.None,
                marker = "DVPL".ToCharArray(),
                crc32Compressed = Data.Length == 0 ? 0 : CalcCRC(Data),
            };
        }
        if (Verbose)
            Console.WriteLine(Header);
        File.Delete(path);
        File.WriteAllBytes(path+".dvpl", OutData.Concat(StructuteToByteArray(Header)).ToArray());
    }
}