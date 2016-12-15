using System;
// ReSharper disable UnusedMember.Global
namespace ReSharper20163
{
    public class IntroducePropertyForLazilyInitialisedField
    {
        private readonly Lazy<string> _foo = new Lazy<string>(() => "Hello world");
    }
}