// ReSharper disable UnusedMember.Global
namespace ReSharper20163
{
    public class TransformParameters
    {        
        public bool DoSomething(int param1, string param2, int param3, out bool param4)
        {
            param4 = true;

            return false;
        }
    }
}