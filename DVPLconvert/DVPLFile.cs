using System;
using System.Linq;
using System.Runtime.InteropServices;
public static unsafe partial class Program
{
    enum cType : uint
    {
        None,
        Lz4,
        Lz4HC,
        RFC1951 // zip
    };

    [StructLayout(0u,Pack = 1)]
    struct DVPLHeader
    {
        public int sizeUncompressed;
        public int sizeCompressed;
        public uint crc32Compressed;
        public cType storeType;
        public int marker;
        public override string ToString() => $"Unpacked size: {sizeUncompressed}\nPacked size: {sizeCompressed}\nCRC hash: {crc32Compressed}\nPackage type: {storeType}";
    }
}
