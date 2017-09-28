using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace FortnitePakReader
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine("Drag and drop a file onto the executable to extract");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Fortnite .pak unpacker by Nexusphobiker");
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("File " + args[0] + " not found");
                Console.ReadKey();
                return;
            }

            //Create output folder
            Directory.CreateDirectory(@"Unpacked");
            Console.WriteLine("Unpacking "+args[0]);

            //Open file for read
            Stream fileStream = File.OpenRead(args[0]);
            Stream testStream = File.OpenRead(args[0]);

            //Read dictionary base offset
            fileStream.Seek(fileStream.Length - 0x24, SeekOrigin.Begin);
            byte[] dictionaryBase = new byte[8];
            fileStream.Read(dictionaryBase, 0, dictionaryBase.Length);
            long dictBase = BitConverter.ToInt64(dictionaryBase, 0);
            Console.WriteLine("DictBase:" + dictBase.ToString("X"));

            //Skip to file path
            dictBase = dictBase + 0x12;
            fileStream.Seek(dictBase, SeekOrigin.Begin);

            long files = 0;

            while (fileStream.Position != fileStream.Length-0x24)
            {
                //Read path length
                byte[] strLen = new byte[4];
                fileStream.Read(strLen, 0, strLen.Length);
                int pathLength = BitConverter.ToInt32(strLen, 0);

                //Read path
                string fileName = "";
                while (pathLength > 1)
                {
                    fileName = fileName + (char)fileStream.ReadByte();
                    pathLength--;
                }

                //Throw the last 0 away
                fileStream.ReadByte();

                //Read offset and file length
                byte[] contentOffset = new byte[8];
                fileStream.Read(contentOffset, 0, contentOffset.Length);
                byte[] fileLength = new byte[8];
                fileStream.Read(fileLength, 0, fileLength.Length);
                
                //Write file to disk (in case of too big files this should be done via another file stream) but skipping that for now
                byte[] content = new byte[BitConverter.ToInt64(fileLength, 0)];
                long tempStreamoffset = fileStream.Position;
                fileStream.Seek(BitConverter.ToInt64(contentOffset, 0) + 0x35, SeekOrigin.Begin);
                fileStream.Read(content, 0, content.Length);
                Directory.CreateDirectory(Path.GetDirectoryName("Unpacked/"+fileName));
                File.WriteAllBytes("Unpacked/" + fileName, content);

                //Read next file
                fileStream.Seek(tempStreamoffset + 0x25, SeekOrigin.Begin);

                Console.WriteLine("Offset:" + fileStream.Position.ToString("X"));
                files++;
                Console.WriteLine("File:" + files);
                Console.WriteLine(fileName);
            }
            Console.ReadKey();
        }
    }
}
