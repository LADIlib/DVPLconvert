namespace DVPLconverter;
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
    private static unsafe void CompressDVPLFile(string path)
    {
        
        DVPLHeader Header;

        byte[] Data = File.ReadAllBytes(path);
        if (Data.Length == 0) 
        { 
            Header = new DVPLHeader
            {
                sizeUncompressed = 0,
                marker = "DVPL",
                sizeCompressed = 0,
                storeType = CompressorType.None,
                crc32Compressed = 0
            };
            Console.WriteLine($"Warning {path} !! Size of compressed arc is 0");
        }
        else
        {
            fixed (byte* ptr = &Data[0])
                Header = new DVPLHeader
                {
                    sizeUncompressed = Data.Length,
                    marker = "DVPL",
                    sizeCompressed = Data.Length,
                    storeType = Data.Length == Data.Length ? CompressorType.None : CompressorType.Lz4,
                    crc32Compressed = CRC32.calculate_crc32(ptr, Data.Length)
                };
        }
        Console.WriteLine(Header);
        File.Delete(path);
        File.WriteAllBytes(path+".dvpl", Data.Concat(Header.ToByteArray()).ToArray());
    }
}