using System.Collections.Generic;

namespace ReSharperTutorials.TutStep
{
    public class Check
    {
        public readonly string[] Actions;
        public readonly string Method;


        public Check(string[] actions, string method)
        {
            Actions = actions;
            Method = method;
        }
    }
}
