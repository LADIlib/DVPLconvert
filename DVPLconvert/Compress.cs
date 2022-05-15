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
        byte[] CompressedData, UncompressedData,DVPLFile;
        
        DVPLHeader Header;

        UncompressedData = File.ReadAllBytes(path);
        byte[] _compressedData = new byte[LZ4Codec.MaximumOutputSize(UncompressedData.Length)];
        var len = LZ4Codec.Encode(UncompressedData,_compressedData, LZ4Level.L12_MAX);
        if (len == 0 || UncompressedData.Length == 0) 
        { 
            CompressedData = Array.Empty<byte>();
            Header = new DVPLHeader
            {
                _uncompressed_size = 0,
                _dvpl_foot = "DVPL",
                _compressed_size = 0,
                _store_type = StoreType.NotCompressed,
                _crc_hash = 0
            };
            Console.WriteLine($"Warning {path} !! Size of compressed arc is 0");
        }
        else
        {
            CompressedData = _compressedData[0..len];
            fixed (byte* ptr = &CompressedData[0])
                Header = new DVPLHeader
                {
                    _uncompressed_size = UncompressedData.Length,
                    _dvpl_foot = "DVPL",
                    _compressed_size = CompressedData.Length,
                    _store_type = UncompressedData.Length == CompressedData.Length ? StoreType.NotCompressed : StoreType.Compressed,
                    _crc_hash = CRC32.crc32(0, ptr, CompressedData.Length)
                };
        }
        Console.WriteLine(Header);
        DVPLFile = CompressedData.Concat(Header.ToByteArray()).ToArray();
        File.Delete(path);
        File.WriteAllBytes(path+".dvpl", DVPLFile);
    }
}