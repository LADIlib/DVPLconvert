# DVPLconvert
Tool that can compress and decompress DVPL formatted files of WoTB.

## Requirements

For build you need
`.NET 4.7.2` or `.NET 5.0` or `.NET 6.0` and `Visual Studio 2022`

If you dont have any of them - you can download latest release from **[Releases](https://github.com/LADIlib/DVPLconvert/releases "Releases")**

## Usage

`DVPLconvert.exe [Arguments] <folder/file>`

#### Command line arguments

|Argument|Meaning|Description|
|:-:|-|-| 
|`-v`| **Verbose**|Program will print debug information in Console|
|`-r`| **Recursive**|Will Compress or decompress folder recursively, sets to **false** after folder parsed|
|`-c`|**Compress**|Forces program to compress Non-.dvpl files to .dvpl|
|`-d`|**Decompress**|Forces program to decompress .dvpl files to Non-.dvpl|
|`-f`| **Folders extention list**|Lists count of extentions of all files |
|`-C`| **Compression type**|Enables compression ( MAY BREAK WEBP FILES !!! ) |