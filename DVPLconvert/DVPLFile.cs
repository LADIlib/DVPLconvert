namespace DVPLconverter;
public static partial class Program
{
    private enum StoreType
    {
        NotCompressed,
        Compressed = 2
    }

    [StructLayout(LayoutKind.Explicit, Size = 20, Pack = 1)]
    private struct DVPLHeader
    {
        [FieldOffset(0)][MarshalAs(UnmanagedType.I4)] public int _uncompressed_size;

        [FieldOffset(4)][MarshalAs(UnmanagedType.I4)] public int _compressed_size;

        [FieldOffset(8)][MarshalAs(UnmanagedType.U4)] public uint _crc_hash;

        [FieldOffset(12)][MarshalAs(UnmanagedType.I4)] public StoreType _store_type;

        [FieldOffset(16)][MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)] public string _dvpl_foot;

        public override string ToString()
        {
            return
@$"Unpacked size: {_uncompressed_size}
Packed size: {_compressed_size}
CRC hash: {_crc_hash}
Package type: {_store_type}
DVPL Footer: {_dvpl_foot}
";
        }
        public byte[] ToByteArray()
        {
            return Array.Empty<byte>()
            .Concat(BitConverter.GetBytes(_uncompressed_size))
            .Concat(BitConverter.GetBytes(_compressed_size))
            .Concat(BitConverter.GetBytes(_crc_hash))
            .Concat(BitConverter.GetBytes((int)_store_type))
            .Concat(Encoding.ASCII.GetBytes(_dvpl_foot)).ToArray();
        }
    }
}
