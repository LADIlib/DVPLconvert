namespace DVPLconverter;
public static partial class Program
{
    private enum CompressorType : int
    {
        None,
        Lz4,
        Lz4HC,
        RFC1951, // deflate, inflate
    };

    [StructLayout(LayoutKind.Explicit, Size = 20, Pack = 1)]
    private struct DVPLHeader
    {
        [FieldOffset(0)][MarshalAs(UnmanagedType.I4)] public int sizeUncompressed;

        [FieldOffset(4)][MarshalAs(UnmanagedType.I4)] public int sizeCompressed;

        [FieldOffset(8)][MarshalAs(UnmanagedType.U4)] public uint crc32Compressed;

        [FieldOffset(12)][MarshalAs(UnmanagedType.I4)] public CompressorType storeType;

        [FieldOffset(16)][MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)] public string marker;

        public override string ToString()
        {
            return
@$"Unpacked size: {sizeUncompressed}
Packed size: {sizeCompressed}
CRC hash: {crc32Compressed}
Package type: {storeType}
";
        }
        public byte[] ToByteArray()
        {
            return Array.Empty<byte>()
            .Concat(BitConverter.GetBytes(sizeUncompressed))
            .Concat(BitConverter.GetBytes(sizeCompressed))
            .Concat(BitConverter.GetBytes(crc32Compressed))
            .Concat(BitConverter.GetBytes((int)storeType))
            .Concat(Encoding.ASCII.GetBytes(marker)).ToArray();
        }
    }
}
