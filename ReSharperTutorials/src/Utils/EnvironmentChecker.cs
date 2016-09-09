using System;

namespace ReSharperTutorials.Utils
{
    /// <summary>
    /// Checks whether user's Visual Studio is able to run tutorials
    /// </summary>
    internal static class EnvironmentChecker
    {

        public static void RunAllChecks()
        {
            if (!ShortcutsAssigned())            
                throw new NoShortCutsAssignedException();            
        }

        private static bool ShortcutsAssigned()
        {
            return VsIntegration.GetActionShortcut("ReSharper_AltEnter") != "Undefined";
        }
    }

    public class NoShortCutsAssignedException : Exception
    {
    }
}
