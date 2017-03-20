using System;

namespace ReSharper20171
{
    class UnformattedCode
    {
        public int SomeInt {
          get;
          set; }




        private string _someString;

        public UnformattedCode(string someString)
        {
            if (someString == "")
            { throw new Exception("String is empty");
            }
            else
             _someString = someString;
        }

    }
}
