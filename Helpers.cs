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
     * Helpers
     * generic functions to be used in other classes
    */
    public class Helpers
    {

        // throw file exception with error message if file doesn't exist
        public static void DoesFileExist(string path, string name)
        {
            if (!File.Exists(path))
            {
                throw new FileException($"{name} file '{path}' does not exist");
            }
        }


        // the following functions are designed to replace the use of LINQ expressions to reduce memory footprint
        // if the functions starts with "Is", then it will check if the entire string is of a char type
        // if the function starts with "Has", then it will check is the string has a char type somewhere

        public static bool IsAllCaps(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!Char.IsLetter(input[i]) && !Char.IsUpper(input[i]))
                    return false;
            }

            return true;
        }

        public static bool HasLowerCase(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsLetter(input[i]) && !Char.IsUpper(input[i]))
                    return true;
            }

            return false;
        }

        public static bool HasDigitAndLetter(string input)
        {
            bool letter = false;
            bool digit = false;
 

            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsLetter(input[i]))
                {
                    letter = true;
                    continue;
                }

                if (Char.IsDigit(input[i]))
                {
                    digit = true;
                    continue;
                }

            }

            return (letter && digit);
        }

        public static bool HasDigitOrSymbol(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!Char.IsLetter(input[i]))
                    return true;
            }

            return false;
        }

        public static bool HasDigitOrSymbol(string input, char ignore)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == ignore)
                {
                    continue;
                }

                if (!Char.IsLetter(input[i]))
                {
                    return true;
                }
                    
            }

            return false;
        }

        public static bool HasLetter(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsLetter(input[i]))
                    return true;
            }

            return false;
        }

        public static bool IsCharSet(string input, string chars)
        {
            string regCheck = "^[" + chars + "]+$";
            Regex Validator = new Regex(regCheck);
            
            return Validator.IsMatch(input);            
        }


        // throw file exception with error message if user doesn't want to delete an existing file
        public static void AskToDelete(string path)
        {
            if (File.Exists(path))
            {
                PrintWarning($"output file '{path}' already exists, do you want to overwrite? Y/N");

                int key = Console.Read();
                if (key == 'Y' || key == 'y')
                {
                    File.Delete(path);
                }

                else
                {
                    throw new FileException($"output file {path} not deleted at request of user");
                }


            }
        }

        public static void PrintWarning(string msg)
        {
            Console.WriteLine("[warning] " + msg);
        }

        // prints message/warning based on if a search query yielded any results
        public static void ResultsFound(int found, string searchFor)
        {
            if (found == 0)
            {
                PrintWarning($"search query: '{searchFor}' not found");
            }
            else
            {
                Console.WriteLine($"search query: '{searchFor}' yielded {found} results");
            }
        }

    }
}
