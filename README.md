# FortnitePakReader
Unpacks the .pak file for the game Fortnite

Fortnites .pak structure


I didnt look into any undocumented entries.



All entries are little endian

//First entry


8 bytes FileList = Filelength-0x24

4 bytes FilePathLengthWithNullTermination = FileList+0x12

string FilePath = FileList+0x16

8 bytes FileContentOffset;

8 bytes FileLength;

//File count is around 220k files atm. It took me 20 minutes to unpack all.
