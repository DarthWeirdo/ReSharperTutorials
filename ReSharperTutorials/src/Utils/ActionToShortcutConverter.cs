using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.ActionManagement;
using JetBrains.UI.ActionsRevised.Loader;

namespace ReSharperTutorials.Utils
{
    internal class ActionToShortcutConverter
    {
        private readonly IEnumerable<IActionDefWithId> _actionDefs;
        private readonly ShortcutScheme _currentScheme;

        public ActionToShortcutConverter(IActionManager actionManager)
        {                        
            _actionDefs = actionManager.Defs.GetAllActionDefs();
            _currentScheme = actionManager.Shortcuts.CurrentScheme;            
        }


        public string SubstituteShortcutsViaVs(string text)
        {
            var keySequence = "";

            var result = Regex.Replace(text, @"<shortcut>(.*?)</shortcut>", txt =>
            {
                var action = txt.Groups[1].Value;

                // VS doesn't support shortcut sequences - GotoText must be treated individually
                if (action == "ReSharper.ReSharper_GotoText")
                {
                    action = "ReSharper.ReSharper_GotoType";
                    if (_currentScheme == ShortcutScheme.Idea)
                        keySequence = ",N,N";
                    if (_currentScheme == ShortcutScheme.VS)
                        keySequence = ",T,T";
                }

                return $"<span class=\"shortcut\">{VsIntegration.GetActionShortcut(action)}{keySequence}</span>";
            });
            return result;
        }


        public string SubstituteShortcuts(string text)
        {
            var result = Regex.Replace(text, @"<shortcut>(.*?)</shortcut>", txt =>
            {
                var action = txt.Groups[1].Value;
                return $"<span class=\"shortcut\">{GetActionShortcut(action)}</span>";
            });
            return result;
        }


        private static string GetActionId(string actionName)
        {
            var index = actionName.LastIndexOf("_", StringComparison.Ordinal) + 1;
            return actionName.Substring(index);
        }


        private string GetActionShortcut(string actionName)
        {                                                                        
            foreach (var def in _actionDefs)
            {
                if (def.ActionId != GetActionId(actionName)) continue;

                if (_currentScheme == ShortcutScheme.Idea)
                {
                    if (def.IdeaShortcuts.Length > 0)                    
                        return def.IdeaShortcuts[0];
                    if (def.VsShortcuts.Length > 0)
                        return def.VsShortcuts[0];
                }

                if (_currentScheme == ShortcutScheme.VS)
                {
                    if (def.VsShortcuts.Length > 0)
                        return def.VsShortcuts[0];
                    if (def.IdeaShortcuts.Length > 0)
                        return def.IdeaShortcuts[0];                    
                }
            }
            return "Undefined";
        }
    }
}
