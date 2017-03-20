using System;

namespace ReSharper20171
{
    public class Person
    {
        private string _lastName;
        private string _firstName;

        public Person(string firstName, string lastName)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (lastName == null)
            {
                throw new ArgumentNullException(nameof(lastName));
            }
            _lastName = lastName;
            _firstName = firstName;
        }
    }


    public class Address
    {        
        private string _city;

        public string City
        {
            get { return _city; }
            set { _city = value ?? ArgumentNullException }
        }
    }

}