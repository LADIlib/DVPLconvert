using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

public static partial class Program
{
    private static unsafe T ByteArrayToStructure<T>(byte[] bytes) where T : struct { fixed (byte* ptr = &bytes[0]) return (T)Marshal.PtrToStructure((IntPtr)ptr, typeof(T)); }

    private static int IsFolder(string path)
    {
        try { return ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory) ? 1 : 0; }
        catch { return -1; }
    }
    private static string GetFileExtention(string file) => file.Split(new char[]{'.'}, StringSplitOptions.RemoveEmptyEntries).Last();
}

namespace System.Runtime.CompilerServices
{
    internal static class RuntimeHelpers
    {
        /// <summary>
        /// Taken from .NET 
        /// </summary>
        public static T[] GetSubArray<T>(T[] array, Range range)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }

            (int offset, int length) = range.GetOffsetAndLength(array.Length);

            if (default(T) != null || typeof(T[]) == array.GetType())
            {
                if (length == 0)
                {
                    return Array.Empty<T>();
                }

                var dest = new T[length];
                Array.Copy(array, offset, dest, 0, length);
                return dest;
            }
            else
            {
                T[] dest = (T[])Array.CreateInstance(array.GetType().GetElementType(), length);
                Array.Copy(array, offset, dest, 0, length);
                return dest;
            }
        }
    }
}