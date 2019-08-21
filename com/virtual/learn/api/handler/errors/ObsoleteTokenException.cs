using System;

namespace lug.Exceptions
{
    class ObsoleteTokenException : Exception
    {
        public ObsoleteTokenException()
        {
          
        }
    
        public ObsoleteTokenException(string name)
            : base(name)
        { 
        }

    }
}