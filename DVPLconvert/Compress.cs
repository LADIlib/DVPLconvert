﻿namespace DVPLconverter;
public static partial class Program
{
    private static void CompressDVPLFolderRecursively(string path)
    {
        Console.WriteLine("Starting compressing folder recursively");
        foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
        {
            var ext = file.Split('.', StringSplitOptions.RemoveEmptyEntries).Last();
            if (ext != "dvpl")
                try
                {
                    Console.WriteLine($"Compressing {file}");
                    CompressDVPLFile(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
        }
    }
    private static void CompressDVPLFolder(string path)
    {
        Console.WriteLine("Starting compressing folder");
        foreach (string file in Directory.GetFiles(path, "*"))
        {
            var ext = file.Split('.', StringSplitOptions.RemoveEmptyEntries).Last();
            if (ext!="dvpl")
                try
                {
                    Console.WriteLine($"Compressing {file}");
                    CompressDVPLFile(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
        }
    }
    private static unsafe uint CalcCRC(byte[] b) { fixed (byte* ptr = &b[0]) return CRC32.calculate_crc32(ptr, b.Length); }
    private static unsafe void CompressDVPLFile(string path)
    {
        byte[] Data = File.ReadAllBytes(path);
        DVPLHeader Header = new()
        {
            sizeCompressed = Data.Length,
            sizeUncompressed = Data.Length,
            storeType = CompressorType.None,
            marker = "DVPL",
            crc32Compressed = Data.Length == 0 ? 0 : CalcCRC(Data),
        };
        Console.WriteLine(Header);
        File.Delete(path);
        File.WriteAllBytes(path+".dvpl", Data.Concat(Header.ToByteArray()).ToArray());
    }
}