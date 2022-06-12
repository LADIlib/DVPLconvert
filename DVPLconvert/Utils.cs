using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

public static unsafe partial class Program
{
    static int IsFolder(string path)
    {
        try { return ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory) ? 1 : 0; }
        catch { return -1; }
    }
    static string GetFileExtention(string file) => file.Split(new char[]{'.'}, StringSplitOptions.RemoveEmptyEntries).Last();
    static uint CalcCRC(byte[] b) { fixed (byte* ptr = &b[0]) return CRC32.calculate_crc32(ptr, b.Length); }
}