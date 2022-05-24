using System;
using System.Linq;
using System.Runtime.InteropServices;
public static partial class Program
{
    enum CompressorType : uint
    {
        None,
        Lz4,
        Lz4HC,
        RFC1951 // zip
    };

    [StructLayout(LayoutKind.Sequential,Size = 20,Pack = 1)]
    struct DVPLHeader
    {
        public int sizeUncompressed;

        public int sizeCompressed;

        public uint crc32Compressed;

        public CompressorType storeType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public char[] marker;

        public override string ToString()
        {
            return
$@"Unpacked size: {sizeUncompressed}
Packed size: {sizeCompressed}
CRC hash: {crc32Compressed}
Package type: {storeType}
";
        }
    }
}
