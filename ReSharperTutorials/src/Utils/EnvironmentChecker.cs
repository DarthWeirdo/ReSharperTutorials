using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.ActionManagement;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;
using ReSharperTutorials.Runner;

namespace ReSharperTutorials.Utils
{
    /// <summary>
    /// Checks whether user's Visual Studio is able to run tutorials
    /// </summary>
    internal static class EnvironmentChecker
    {
        

        public static void RunAllChecks(int tutorialId)
        {
            var actionManager = Shell.Instance.GetComponent<IActionManager>();

            if (ShortcutSchemeNotSelected(actionManager))
                throw new NoShortcutSchemeSelectedException();

            var undefShortcuts = ShortcutsUndefined(tutorialId, actionManager);
            if (undefShortcuts.Count > 0)
            {
                var e = new NoShortcutsAssignedException();
                e.Data.Add("Shortcuts", undefShortcuts);
                throw e;
            }
        }

        private static List<string> ShortcutsUndefined(int tutorialId, IActionManager actionManager)
        {
            var tutPath = GlobalSettings.Instance.GetPath(tutorialId, PathType.WorkCopyContentFile);            
            var actionConverter = new ActionToShortcutConverter(actionManager);

            try
            {               
                var text = System.IO.File.ReadAllText(tutPath);                
                var undefShortcuts = actionConverter.GetUndefinedShortcutsList(text);
                return undefShortcuts;
            }
            catch (Exception e)
            {                
                MessageBox.ShowError(
                    "Tutorial content files are not found. Please reinstall the plugin.",
                    "ReSharper Tutorials");
                throw;
            }
        }

        private static bool ShortcutSchemeNotSelected(IActionManager actionManager)
        {
            var currentScheme = actionManager.Shortcuts.CurrentScheme;
            return currentScheme == ShortcutScheme.None;
        }
    }

    public class NoShortcutsAssignedException : Exception
    {
    }

    public class NoShortcutSchemeSelectedException : Exception
    {
    }
}