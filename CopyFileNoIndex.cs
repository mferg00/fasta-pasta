using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Search16s
{

    /* 
     * CopyFileNoIndex
     * class to copy a section from one file to another
     * if a desired section to copy is based on a search query, every line of the input file is checked until the file ends or the query is found
     */
    public class CopyFileNoIndex : Helpers
    {
        private readonly StreamWriter outputWriter;
        private readonly string input;

        // check for file existance, ask to delete output before creating new StreamWriter
        public CopyFileNoIndex(string inputPath, string outputPath)
        {
            DoesFileExist(inputPath, "input");
            DoesFileExist(outputPath, "output");

            AskToDelete(outputPath);
            input = inputPath;

            outputWriter = new StreamWriter(outputPath, false);
        }

        // sequence is found by searching every line of the .fasta file
        // .fasta StreamReader needs to be restarted for every new search query to ensure it doesn't miss anything in the next iteration
        public bool FindAndCopy(string searchFor, int depth)
        {
            // using streamreader so it disposes once finished
            using (StreamReader inputReader = new StreamReader(input))
            {
                string line = "";
                int counter = 1;

                while ((line = inputReader.ReadLine()) != null)
                {
                    if (line.Contains(searchFor))
                    {
                        for (int i = 0; i < depth; i++)
                        {
                            outputWriter.WriteLine(line);
                            line = inputReader.ReadLine();
                        }
                        return true;
                    }
                    counter++;
                }

                ResultsFound(0, searchFor);
            }

            return false;
        }

        // once all the searching and copying is done, close the StramWriter
        public void CloseWriter()
        {
            outputWriter.Close();
        }

    }
}
