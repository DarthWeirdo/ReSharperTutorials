namespace ReSharper20171
{

    abstract class Base
    {
        public abstract (string name, string surname) GetPerson();                
    }

    class Derived : Base
    {
        public override (string name, string surname) GetPerson() => ("John", "Doe");
    }
}
