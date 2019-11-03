using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Search16s
{
    /* 
     * ReadFile
     * simple class to search/read a user provided file to the terminal
     * can read from a specified line with a specified depth
     * can find a search query, and print from the line to a specified depth as well
     * purpose is to provide extra functions for the StreamReader class
     */
    public class ReadFile : Helpers
    {
        private readonly string input;

        public ReadFile(string inputPath)
        {
            DoesFileExist(inputPath, "input file");

            input = inputPath;
        }

        // will read a user specified portion of the input file to the console
        // takes line start and line depth as input
        public void ReadSection(int start, int depth)
        {
            // using streamreader so it disposes once finished
            using (StreamReader inputReader = new StreamReader(input))
            {
                string line = "";
                int counter = 1;
                int end = start + depth;

                while ((line = inputReader.ReadLine()) != null)
                {
                    // will print the user specified lines
                    if (counter >= start && counter < end)
                    {
                        Console.WriteLine(line);
                    }

                    counter++;
                }

                // wil check if the user tried to read past the end of the file
                if (counter <= end)
                {
                    PrintWarning("attempted to access non-existant lines");
                }
            }
        }

        // will search for a line, and then print a user specified line depth starting at that line to the console
        // takes a search query and line depth as input
        public void ReadSection(string searchFor, int depth)
        {
            using (StreamReader inputReader = new StreamReader(input))
            {
                string line = "";

                // read line by line
                while ((line = inputReader.ReadLine()) != null)
                {
                    if (line.Contains(searchFor))
                    {
                        // write the line that contains the search query
                        Console.WriteLine(line);

                        // write the lines after fro a user specified amount, or until the end of the file is reached
                        for (int i = 0; i < depth - 1; i++)
                        {
                            if ((line = inputReader.ReadLine()) != null)
                            {
                                Console.WriteLine(line);
                            }
                            else
                            {
                                PrintWarning("attempted to access non-existant lines");
                                return;
                            }
                        }
                        return;
                    }
                }

                ResultsFound(0, searchFor);
            }
        }

    }
}
