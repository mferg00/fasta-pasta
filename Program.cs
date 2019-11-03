using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Search16s
{
    /* 
     * Main
     * the purpose of this is to try to create and execute all objects, and catch any fatal errors (if any)
     */
    class Program
    {
        // called if program finished successfully message
        static void ExitSuccess()
        {
            Console.WriteLine("\n\tprogram finished successfully");
        }

        // called if program finished unsuccessfully with error message
        static void ExitFailure(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("\n\tprogram did not finish successfully");
        }

        static void Main(string[] args)
        {

            // check if the program is run from visual studio or terminal
            if (Debugger.IsAttached)
            {
                ExitFailure("visual studio detected, please run /Bin/Debug/Search16s.exe in terminal or VS developer terminal");
                return;
            }

            // there are two custom exceptions in this project, ArgException and FileException
            // place everything in a try catch block, as the ArgParser object can throw either of these exceptions
            try
            {
                // creating the new object will throw an ArgException/FileException if any of the provided arguments are invalid
                ArgParser cmdArgs = new ArgParser(args);

                // create fasta parsing object
                FastaParse fastaParse = new FastaParse(cmdArgs.fasta);

                // call the fasta parse method for the chosen level
                switch (cmdArgs.level)
                {
                    case 1:
                        fastaParse.Level1(cmdArgs.lineStart, cmdArgs.lineDepth);
                        break;

                    case 2:
                        fastaParse.Level2(cmdArgs.searchFor);
                        break;

                    case 3:
                        fastaParse.Level3(cmdArgs.query, cmdArgs.results);
                        break;

                    case 4:
                        fastaParse.Level4(cmdArgs.query, cmdArgs.results, cmdArgs.index);
                        break;

                    case 5:
                        fastaParse.Level5(cmdArgs.searchFor);
                        break;

                    case 6:
                        fastaParse.Level6(cmdArgs.searchFor);
                        break;

                    case 7:
                        fastaParse.Level7(cmdArgs.searchFor);
                        break;

                    default:
                        ExitFailure("[error] unkown error");
                        return;
                }

                ExitSuccess();
            }         

            // catch all exceptions
            catch(Exception ex)
            {
                // if they are either of the custom exceptions, print their message
                if(ex is ArgException || ex is FileException || ex is ParseException)
                { 
                    ExitFailure(ex.Message);
                    return;
                }

                throw;
                
            }            
        }
    }
}
