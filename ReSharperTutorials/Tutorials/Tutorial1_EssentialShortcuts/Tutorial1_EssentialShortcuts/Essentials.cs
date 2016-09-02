using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace Tutorial1_EssentialShortcuts
{
    public class badlyNamedClass
    {
        private Coordinates _coordinates;
    }

    public class ContextAction
    {        
        public static string FormatString(string arg)
        {            
            return "Hello" + arg + "World";
        }

        public Coordinates ReturnCoordinates()
        {
            return new Coordinates();
        }

        public string ReturnString(string stringArg, int intArg)
        {
            return (intArg.ToString() + stringArg);
        }

    }
    
    public class FindAction
    {
    }

    public class Coordinates
    {
        public int X;
        public int Y;
    }
}
