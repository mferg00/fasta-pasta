using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search16s
{

    /* 
     * ArgException
     * this will be thrown if any of the cmd line arguments provided are invalid
     */
    [Serializable]
    class ArgException : Exception
    {
        public ArgException() { }

        public ArgException(string message)
            : base(String.Format("[arg error] {0}", message))
        {

        }

    }

    /* 
     * FileException
     * thrown if any of the files provided have formatting issues or don't exist
     */
    [Serializable]
    class FileException : Exception
    {
        public FileException() { }

        public FileException(string message)
            : base(String.Format("[file error] {0}", message))
        {

        }

    }

    /* 
     * ArgException
     * this will be thrown if any of the provided search queries are invalid
     */
    [Serializable]
    class ParseException : Exception
    {
        public ParseException() { }

        public ParseException(string message)
            : base(String.Format("[parse error] invalid search query '{0}'", message))
        {

        }

    }
}
