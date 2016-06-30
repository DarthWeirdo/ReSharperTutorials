using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace Tutorial1_EssentialShortcuts
{
    public class badlyNamedClass
    {
        private SomeClass _someField;
    }

    public class ContextAction
    {        
        public static string FormatString(string arg)
        {            
            return "Hello" + arg + "World";
        }

        public SomeClass ReturnSomeClass()
        {
            return new SomeClass();
        }

    }
    
    public class GoToAction
    {
    }

    public class SomeClass
    {        
    }
}
