using System;

namespace ReSharper20171
{
    public class LocalFunctions
    {
        public int Factorial(int number)
        {
            Func<int, int> factorial = null;
            factorial = x =>
            {
                if (x == 0) return 1;
                return x * factorial(x - 1);
            };

            return factorial(number);
        }        
    }
}