namespace ReSharper20171
{
    class Usages
    {
        private IFoo _foo;

        public Usages(object o)
        {
            var foo = o as IFoo;
            if (foo != null)
                _foo = foo;
        }
    }

    interface IFoo
    {
        
    }
    
    class Boo
    {
        public IFoo Foo { get; set; }
    }    
}
