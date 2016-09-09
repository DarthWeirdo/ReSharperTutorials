using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace Tutorial1_EssentialShortcuts
{
    public class badlyNamedClass
    {
        private CenterCoordinates _coordinates;
    }

    public class ContextAction
    {        
        public static string FormatString(string arg)
        {            
            return "Hello" + arg + "World";
        }

        public CenterCoordinates ReturnCoordinates()
        {
            return new CenterCoordinates();
        }

        public string ReturnString(string stringArg, int intArg)
        {
            return (intArg.ToString() + stringArg);
        }

    }
    
    public class FindAction
    {
    }

    public class CenterCoordinates
    {
        public int X;
        public int Y;
    }
}
