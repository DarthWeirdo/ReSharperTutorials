// ReSharper disable UnusedMember.Global
// ReSharper disable NotAccessedField.Local
namespace ReSharper20163
{
    public class IntroduceFromUnusedParameters
    {
        private readonly int _used;
        
        public IntroduceFromUnusedParameters(int unused1, int unused2, int used, int unused3)
        {
            _used = used;
        }
    }
}