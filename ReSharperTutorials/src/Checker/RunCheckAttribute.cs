using System;

namespace ReSharperTutorials.Checker
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class RunCheckAttribute : Attribute
    {
        public readonly OnEvent OnEvent;

        public RunCheckAttribute(OnEvent onEvent)
        {
            OnEvent = onEvent;
        }
    }
}