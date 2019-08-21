using System;

namespace lug.Exceptions
{
    class InvalidSubjectTokenException : Exception
    {
        public InvalidSubjectTokenException()
        {
          
        }
    
        public InvalidSubjectTokenException(string name)
            : base(name)
        { 
        }

    }
}