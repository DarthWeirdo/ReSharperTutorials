using System;

namespace Tutorial1_EssentialShortcuts
{
    class BadlyFormattedCode
    {
        public int SomeInt { get;
        set; }


        private string _someString;

        public BadlyFormattedCode(string someString)
        {
            if (someString == "") {    throw new Exception("String is empty");}
            else                                                                               
        _someString = someString;

        }
    }
}
