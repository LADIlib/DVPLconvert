namespace DVPLconverter;
public static partial class Program
{
    private static void DecompressDVPLFolderRecursively(string path)
    {
        Console.WriteLine("Starting decompressing folder recursively");
        foreach (string file in Directory.GetFiles(path, "*.dvpl", SearchOption.AllDirectories))
        {
            try
            {
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
        byte[] DVPLFile, UncompressedData;
        DVPLHeader Header;

        DVPLFile = File.ReadAllBytes(path);
        Header = ByteArrayToStructure<DVPLHeader>(DVPLFile[^20..]);
        fixed (byte* ptr = &DVPLFile[0])
            if (CRC32.crc32(0, ptr, Header._compressed_size) != Header._crc_hash)
                throw new Exception("CRC hash mismatch");
        Console.WriteLine(Header);
        if (Header._store_type == StoreType.NotCompressed)
            UncompressedData = DVPLFile[..Header._compressed_size];
        else 
        {
            UncompressedData = new byte[Header._uncompressed_size];
            LZ4Codec.Decode(DVPLFile, 0, Header._compressed_size, UncompressedData, 0, Header._uncompressed_size);
            if (UncompressedData.Length != Header._uncompressed_size) throw new Exception("Length is wrong");
        }
        File.Delete(path);
        File.WriteAllBytes(path.Replace(".dvpl", String.Empty), UncompressedData);
    }
}