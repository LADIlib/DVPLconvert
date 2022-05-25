using System;
using System.Collections.Generic;
using System.IO;

public static partial class Program
{
    static bool Verbose = false;
    static bool Compress = false;
    public static void Main(string[] args)
    {
        if (args.Length <= 0) 
        {
            Console.WriteLine(
                @" Usage: [Arguments] <folder or file path>

 -v == Verbose, will print debug information while do all work
[other arguments] -r [other arguments] <Folder path> == Enables recursive for given filder (required argument!!!)
[other arguments] -c [other arguments] <Folder path> == Forces program to compress non .dvpl files to .dvpl
[other arguments] -d [other arguments] <Folder path> == Forces program to decompress .dvpl files to non .dvpl
 -f <Folder path> == lists count of extentions of all files 
 -C == Enables compress ( MAY BREAK WEBP FILES !!! )
"
                );
            throw new ArgumentException("No input files/folders/arguments given"); 
        }
        bool recursive = false;
        bool ForceCompress = false;
        bool ForceDecompress = false;
        bool CheckFolder = false;
        foreach(var arg in args)
        {
            if (!Verbose)
                Verbose = arg == "-v";
            if (!recursive)
                recursive = arg == "-r";
            if (!ForceCompress)
                ForceCompress = arg == "-c";
            if (!ForceDecompress)
                ForceDecompress = arg == "-d";
            if (!Compress)
                Compress = arg == "-C";
            if(!CheckFolder)
                CheckFolder = arg == "-f";
            else
            {
                Dictionary<string, uint> dic = new Dictionary<string, uint>();
                foreach (string file in Directory.GetFiles(arg, "*", SearchOption.AllDirectories))
                {
                    var ext = GetFileExtention(file);
                    if (!dic.ContainsKey(ext))
                        dic[ext] = 0;
                    dic[ext]++;
                }
                foreach(var kv in dic)
                {
                    Console.WriteLine($"{kv.Key} : {kv.Value}");
                }
                CheckFolder = false;
                continue;
            }
            var ifo = IsFolder(arg);
            if (ifo == 1)
            {
                Dictionary<string, uint> dic = new Dictionary<string, uint>();
                foreach (string file in Directory.GetFiles(arg, "*", SearchOption.AllDirectories))
                {
                    var ext = GetFileExtention(file);
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
                var ext = GetFileExtention(arg);
                if (((ext == "dvpl") || ForceDecompress) && !ForceCompress) DecompressDVPLFile(arg);
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
