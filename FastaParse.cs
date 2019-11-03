using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Search16s
{

    /* 
      * FastaParse
      * class to provide functions for each search level
      * takes input from the ArgParse object checked command line arguments
      * output is the user's desired command (if entered correctly)
      */
    public class FastaParse : Helpers
    {
        // file paths
        private readonly string fasta;

        public FastaParse(string fastaPath)
        {
            fasta = fastaPath;
        }

        // will parse the line the sequence ID is on and print only the ID
        private void PrintSeqIDs(string line)
        {
            // example line: '>NR_112345.1 Streptomyces ... >NR_123314.2 Streptomyces ...'
            // will be split as: '', 'NR_112345.1', 'Streptomyces ...', 'NR_123314.2', 'Streptomyces ...'
            string[] lineSplit = Regex.Split(line, @">(.*?) ");
            
            // skip the intial whitespace, and then print every sequence ID
            for (int i = 1; i < lineSplit.Length; i += 2)
            {
                Console.WriteLine(lineSplit[i]);
            }
        }

        // there are 3 things that the user can search for:
        // 1: sequence id - "NR_1234.1", contains letters and numbers
        // 2: sequence name - "Streptomyces", contains only letters
        // 3: full sequence - "GTAGCTAT", contains only uppercase letters

        // rules out seq name and full seq
        private bool IsSeqID(string line)
        {
            return HasDigitAndLetter(line);
        }
        
        // rules out 
        private bool IsSeqName(string line)
        {
            return !HasDigitOrSymbol(line);
        }

        private bool IsFullSeq(string line)
        {
            return IsAllCaps(line);
        }

        public void Level1(int start, int depth)
        {
            // ensures the line starts at the sequence id line and not the full sequence line
            // i.e. it starts at ">NR_1234..." instead of "GTACTGATCGA..."
            if (start % 2 == 0)     
            {
                start--;
                PrintWarning($"starting line corrected from {start + 1} to {start}");
            }

            // double the depth to grab the full sequence (two lines)
            depth *= 2;

            ReadFile fastaReader = new ReadFile(fasta);
            fastaReader.ReadSection(start, depth);

        }

        public void Level2(string searchFor)
        {

            if (!IsSeqID(searchFor))
            {
                throw new ParseException(searchFor);
            }

            ReadFile fastaReader = new ReadFile(fasta);
            fastaReader.ReadSection(searchFor, 2);          
        }

        public void Level3(string query, string results)
        {
            using(StreamReader queryReader = new StreamReader(query))
            {
                CopyFileNoIndex fastaCopier = new CopyFileNoIndex(fasta, results);
                string searchFor;
                while ((searchFor = queryReader.ReadLine()) != null)
                {
                    if (!IsSeqID(searchFor))
                    {
                        PrintWarning($"invalid search query {searchFor}");
                        continue;
                    }

                    fastaCopier.FindAndCopy(searchFor, 2);
                }
                fastaCopier.CloseWriter();
            }
        }

        public void Level4(string query, string results, string index)
        {
            using (StreamReader queryReader = new StreamReader(query))
            {
                CopyFileWithIndex fastaCopier = new CopyFileWithIndex(fasta, results, index);
                string searchFor;
                while ((searchFor = queryReader.ReadLine()) != null)
                {
                    if (!IsSeqID(searchFor))
                    {
                        PrintWarning($"invalid search query {searchFor}");
                        continue;
                    }

                    fastaCopier.SeekAndCopy(searchFor, 2);
                }
                fastaCopier.CloseWriter();
            }
        }

        public void Level5(string searchFor)
        {

            if (!IsFullSeq(searchFor))
            {
                throw new ParseException(searchFor);
            }

            using (StreamReader fastaReader = new StreamReader(fasta))
            {
                string seqIdLine;
                string seqFullLine;
                int found = 0;

                while ((seqIdLine = fastaReader.ReadLine()) != null)
                {
                    if ((seqFullLine = fastaReader.ReadLine()) == null)
                    {
                        break;
                    }

                    if (seqFullLine.Contains(searchFor))
                    {
                        found++;
                        PrintSeqIDs(seqIdLine);
                    }
                }

                ResultsFound(found, searchFor);
            }
        }
        public void Level6(string searchFor)
        {
            if (!IsSeqName(searchFor))
            {
                throw new ParseException(searchFor);
            }

            using (StreamReader fastaReader = new StreamReader(fasta))
            {
                string seqIdLine;
                int found = 0;
                
                // read lines 0, 2, 4, 6...
                while ((seqIdLine = fastaReader.ReadLine()) != null)
                {
                    // check if sequence contains search query
                    if (seqIdLine.Contains(searchFor))
                    {
                        found++;
                        PrintSeqIDs(seqIdLine);
                    }

                    // skip past the full sequence lines (lines 1, 3, 5...)
                    if (fastaReader.ReadLine() == null)
                    {
                        break;
                    }
                }

                ResultsFound(found, searchFor);
            }            
        }

        public void Level7(string searchFor)
        {
            if (HasDigitOrSymbol(searchFor, '*'))
            {
                throw new ParseException(searchFor);
            }

            string pattern = "^" + Regex.Escape(searchFor).Replace("\\*", ".*") + "$";
            string seqFull;
            int found = 0;

            using (StreamReader fastaReader = new StreamReader(fasta))
            {
                fastaReader.ReadLine();

                while ((seqFull = fastaReader.ReadLine()) != null)
                {
                    Match seq = Regex.Match(seqFull, pattern);
                    if (seq.Success)
                    {
                        found++;
                        Console.WriteLine($"{seq.Value}");
                    }

                    if(fastaReader.ReadLine() == null)
                    {
                        break;
                    }
                }

                ResultsFound(found, searchFor);
            }
               
        }



    }
}
