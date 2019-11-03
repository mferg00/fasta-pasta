using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Search16s
{


    /* 
     * ArgParser
     * class to parse the command line arguments given in Main()
     * catches these issues: invalid commands, missing commands, files that don't exist, etc...
     * doesn't catch these issue: invalid file formats and contents, invalid search queries
     */
    class ArgParser : Helpers
    {

        public int level;
        public string searchFor;
        public string query;
        public string fasta;
        public string index;
        public string results;
        public int lineStart;
        public int lineDepth;

        public ArgParser(params string[] args)
        {
            Initialise(args);
        }

        // when the object first is created with the command line arguments proivided, this function runs
        // if it encounters anything invalid, it will throw an ArgException with suitable error message
        private void Initialise(string[] args)
        {

            // smallest amount of cmd arguments is 3, which is in levels 2, 5, 6, 7: e.g. "-level2 16s.fasta NR12412.1"
            if (args.Length < 3)
            {
                throw new ArgException("not enough cmd line arguments");
            }

            fasta = args[1];

            // check if fasta file exists
            DoesFileExist(fasta, "fasta");

            // get the level from command args
            level = GetLevel(args[0]);

            // choose what to fill each variable with based on the level
            switch (level)
            {
                case 1:
                    CheckArgLength(4, args, "170 10");

                    // check if numbers provided are actually true integers
                    if (!int.TryParse(args[2], out lineStart))
                    {
                        throw new ArgException("line number not valid");
                    }

                    if (!int.TryParse(args[3], out lineDepth))
                    {
                        throw new ArgException("line depth not valid");
                    }

                    // catch start and depth values that are below the minimum
                    if (lineStart < 0)
                    {
                        throw new ArgException("starting line number must be non-negative");
                    }

                    if (lineStart < 1)
                    {
                        throw new ArgException("sequence depth must be greater than 0");
                    }

                    break;

                case 2:
                    CheckArgLength(3, args, "NR1235631.1");
                    searchFor = args[2];
                    break;

                case 3:
                    CheckArgLength(4, args, "query.txt results.txt");
                    query = args[2];
                    DoesFileExist(query, "query");
                    results = args[3];
                    break;

                case 4:
                    CheckArgLength(5, args, "16S.index query.txt results.txt");
                    index = args[2];
                    DoesFileExist(index, "index");
                    query = args[3];
                    DoesFileExist(query, "query");
                    results = args[4];
                    break;

                case 5:
                    CheckArgLength(3, args, "CTGGTACGGT");
                    searchFor = args[2];
                    break;

                case 6:
                    CheckArgLength(3, args, "Steptomyces");
                    searchFor = args[2];
                    break;

                case 7:
                    CheckArgLength(3, args, "ACTG*GTAC*CA");
                    searchFor = args[2];
                    break;

                default:
                    
                    break;
            }
        }

        // takes the '-level_' flag (valid or not) and error checks it
        private static int GetLevel(string arg)
        {
            // filter anything without '-level'
            if (!arg.StartsWith("-level"))
            {
                throw new ArgException("'-level' flag not found");
            }

            // -level* is still valid input (* = any amount of chars)
            // filter by proper length
            if (arg.Length != 7)
            {
                throw new ArgException("level number must be an integer from 1-7");
            }

            // -level_ is valid input (_ = one char)
            // check if the character after '-level' is a valid level number (1-7)
            if (int.TryParse(arg[6].ToString(), out int checkLevel) 
                && (checkLevel > 0 && checkLevel <= 7))
            {
                return checkLevel;
            }

            else
            {
                throw new ArgException("level number must be an integer from 1-7");
            }
        }

        // takes an input of command line arguments, and checks if it is the right length given it's level. If not, it prints an error msg
        private void CheckArgLength(int len, string[] args, string errmsg)
        {
            if (args.Length < len)
            {
                throw new ArgException("not enough cmd line arguments, example args are 'Search16s -levelN 16s.fasta " + errmsg + "'");
            }
            else if(args.Length > len)
            {
                throw new ArgException("too many cmd line arguments, example args are 'Search16s -levelN 16s.fasta " + errmsg + "'");
            }
        }




    }
}
