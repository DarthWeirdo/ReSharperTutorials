using JetBrains.CommonControls.Browser;

namespace ReSharperTutorials.TutorialUI
{
    public interface IHtmlCommunication
    {
        HtmlMediator HtmlMediator { get; }
        HtmlViewControl HtmlViewControl { get; }
        void RunTutorial(string htmlTutorialId);
    }
}