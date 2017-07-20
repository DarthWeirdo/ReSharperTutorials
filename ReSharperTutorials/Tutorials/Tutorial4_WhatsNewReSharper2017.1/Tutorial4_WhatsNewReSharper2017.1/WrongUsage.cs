namespace ReSharper20171
{
    class WrongUsage
    {
        public WrongUsage()
        {
            var someClass = new SomeClass();
            someClass.DoSomething();
        }
    }

    class RightUsage
    {
        public RightUsage()
        {
            var someClass = new SomeClass();            
            someClass.DoSomething((IFoo) new object());
        }
    }

    class SomeClass
    {
        public void DoSomething(IFoo foo)
        {            
        }
    }    

}
