using System;

namespace lug.Exceptions
{
    class InvalidTokenException : Exception
    {
        public InvalidTokenException()
        {
          
        }
    
        public InvalidTokenException(string name)
            : base(name)
        { 
        }

    }
}