using JetBrains.UI.ActionsRevised;
using JetBrains.UI.MenuGroups;

namespace ReSharperTutorials.Runner
{
    [ActionGroup("ActionGroupOpenTutorial", ActionGroupInsertStyles.Submenu, Text = "Tutorials", Id = 200)]
    public class ActionGroupOpenTutorial : IAction, IInsertLast<MainMenuFeaturesGroup>
    {
        public ActionGroupOpenTutorial(ActionOpenTutorial1 action, ActionShowMainTutorialWindow action2)
        {            
        }        
    }
}