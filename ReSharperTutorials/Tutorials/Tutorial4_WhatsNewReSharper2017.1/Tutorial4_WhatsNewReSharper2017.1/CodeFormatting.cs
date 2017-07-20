using System;

namespace ReSharper20171
{
    class CodeFormatting
    {
        public string Foo { get; set; }


        public CodeFormatting(string foo)
        {
            if (foo == null)
            {
                throw new ArgumentNullException();
            }
            Foo = foo;
        }
    }
}