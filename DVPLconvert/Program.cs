namespace DVPLconverter;
public static partial class Program 
{
    private static unsafe T ByteArrayToStructure<T>(byte[] bytes) where T : struct{ fixed (byte* ptr = &bytes[0]) return (T)Marshal.PtrToStructure((IntPtr)ptr, typeof(T));}

    private static int IsFolder(string path) 
    {
        try { return ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)?1:0; }
        catch { return -1; }
    }
    private static bool Verbose = false;

    public static void Main(string[] args)
    {
        if (args.Length <= 0) throw new ArgumentException("No input files/folders given");
        bool recursive = false;
        bool ForceCompress = false;
        bool ForceDecompress = false;
        bool CheckFolder = false;
        foreach(var arg in args)
        {
            if (!Verbose)
                Verbose = arg == "-v";
            if(!recursive)
                recursive = arg == "-r";
            if (!ForceCompress)
                ForceCompress = arg == "-c";
            if (!ForceDecompress)
                ForceDecompress = arg == "-d";
            if(!CheckFolder)
                CheckFolder = arg == "-f";
            else
            {
                Dictionary<string, uint> dic = new();
                foreach (string file in Directory.GetFiles(arg, "*", SearchOption.AllDirectories))
                {
                    var ext = file.Split('.', StringSplitOptions.RemoveEmptyEntries).Last();
                    if (!dic.ContainsKey(ext))
                        dic[ext] = 0;
                    dic[ext]++;
                }
                foreach(var kv in dic)
                {
                    Console.WriteLine($"{kv.Key} : {kv.Value}");
                }
                continue;
            }
            var ifo = IsFolder(arg);
            if (ifo == 1)
            {
                Dictionary<string, uint> dic = new();
                foreach (string file in Directory.GetFiles(arg, "*", SearchOption.AllDirectories))
                {
                    var ext = file.Split('.', StringSplitOptions.RemoveEmptyEntries).Last();
                    if (!dic.ContainsKey(ext))
                        dic[ext] = 0;
                    dic[ext]++;
                }
                if ((dic.ContainsKey("dvpl") || ForceDecompress) && !ForceCompress)
                    if (recursive) DecompressDVPLFolderRecursively(arg);
                    else DecompressDVPLFolder(arg);
                else
                    if (recursive) CompressDVPLFolderRecursively(arg);
                else CompressDVPLFolder(arg);
                recursive = false;
                ForceCompress = false;
                ForceDecompress = false;
            }
            else if (ifo == 0)
            {
                var ext = arg.Split('.', StringSplitOptions.RemoveEmptyEntries).Last();
                if (ext == "dvpl") DecompressDVPLFile(arg);
                else CompressDVPLFile(arg);
            }
            else if (ifo == -1)
            {
                if (Verbose) Console.WriteLine("Got argument: " + arg);
            }
            else throw new Exception("args broken");
        }
        Console.WriteLine("All work done");
        Console.ReadLine();
    }
}
