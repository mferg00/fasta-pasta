using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Search16s
{
    /* 
     * CopyFileWithIndex
     * class to copy a section from one file to another using an index file with byte positions
     * if a desired section to copy is based on a search query, the index file is loaded into memory, and the byte position is found and used
     */
    public class CopyFileWithIndex : Helpers
    {
        private readonly Dictionary<string, long> storedIndex;
        private readonly string input;
        private readonly string index;
        private readonly FileStream outputWriter;

        // deletes the old results file, creates a new one, and loads the index file into a dictionary
        public CopyFileWithIndex(string inputPath, string outputPath, string indexPath)
        {
            DoesFileExist(inputPath, "input");
            DoesFileExist(outputPath, "output");
            DoesFileExist(indexPath, "index");

            input = inputPath;
            index = indexPath;

            AskToDelete(outputPath);
            outputWriter = File.Create(outputPath);
            storedIndex = StoreIndex(index);
        }

        // loads an index file into a dictionary
        private Dictionary<string, long> StoreIndex(string index)
        {
            Dictionary<string, long> dict = new Dictionary<string, long>();

            // load up the index file, use using so it disposes once finished
            using (StreamReader inputReader = new StreamReader(index))
            {
                // create variables for each line to fill every loop
                string line = "";
                string[] split = new string[2];

                // line counter, used for error messages
                int counter = 1;

                while ((line = inputReader.ReadLine()) != null)
                {
                    long bytePos;

                    // if the line in the index file is written as "{string} {long}", this will try successfully
                    try
                    {
                        split = line.Split(' ');
                        string seqId = split[0];
                        string bytePosString = split[1];
                        if (!long.TryParse(bytePosString, out bytePos))
                        {
                            throw new FileException($"invalid byte format at line {counter} in index file");
                        }
                    }
                    catch
                    {
                        throw new FileException($"line {counter} invalid in index file");
                    }

                    dict.Add(split[0], bytePos);
                    counter++;
                }
            }
            return dict;
        }

        // searches for a sequence id in the loaded index file, goes to the byte position in the input file, and copies a specified depth to the output file
        public void SeekAndCopy(string searchFor, int depth)
        {
            using (FileStream inputReader = new FileStream(input, FileMode.Open, FileAccess.Read))
            {
                // try to get byte pos from dictionary
                if (storedIndex.TryGetValue(searchFor, out long bytepos))
                {
                    // seek to byte pos in input file
                    inputReader.Seek(bytepos, SeekOrigin.Begin);

                    int nlCount = 0;
                    int chr;

                    // read and write each byte one at a time
                    while ((chr = inputReader.ReadByte()) != -1)
                    {
                        outputWriter.WriteByte(Convert.ToByte(chr));

                        // if the byte is a newline, incrmement the newline counter
                        if (chr == '\n')
                        {
                            nlCount++;
                        }

                        if (nlCount >= depth) return;
                    }

                    // the end of the very last sequence in the input file is not terminated with a '\n' char
                    // this line of code is only reached if the end of the file is reached
                    // add a newline char to ensure the next searched sequence is printed on a new line
                    outputWriter.WriteByte(Convert.ToByte('\n'));
                }

                // if nothing in dictionary, sequence doesn't exist
                else
                {
                    ResultsFound(0, searchFor);
                }
            }
        }

        // once all the searching and copying is done, close the StramWriter
        public void CloseWriter()
        {
            outputWriter.Close();
        }
    }
}
