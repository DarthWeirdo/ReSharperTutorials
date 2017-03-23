using System.IO;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.TextControl;
using ReSharperTutorials.Runner;

namespace ReSharperTutorials.Checker
{
    class Tutorial4Checks : ICustomCheck
    {
        public ISolution Solution { get; set; }
        public IEditorManager EditorManager { get; set; }
        public ITextControlManager TextControlManager { get; set; }
        public DocumentManager DocumentManager { get; set; }

        public Tutorial4Checks(ISolution solution, IEditorManager editorManager, DocumentManager documentManager,
            ITextControlManager textControlManager)
        {
            Solution = solution;
            EditorManager = editorManager;
            DocumentManager = documentManager;
            TextControlManager = textControlManager;
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep3()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial4_WhatsNewReSharper2017.1", "Exceptions.cs",
                "firstName ?? throw new");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep5()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial4_WhatsNewReSharper2017.1", "Exceptions.cs",
                "value ?? throw new");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep7()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial4_WhatsNewReSharper2017.1", "LocalFunctions.cs",
                "int Func(int x)");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep9()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial4_WhatsNewReSharper2017.1", "LocalFunctions.cs",
                "private int");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep11()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial4_WhatsNewReSharper2017.1", "CodeFormatting.cs",
                "foo){");
        }

        //[RunCheck(OnEvent.OnTimer)]
        //public bool CheckStep12()
        //{
        //    var di = new DirectoryInfo(
        //        GlobalSettings.Instance.GetPath(TutorialId.Tutorial4, PathType.WorkCopySolutionFolder) +
        //        "\\Tutorial4_WhatsNewReSharper2017.1");
        //    FileInfo[] settingsFiles = di.GetFiles("*.sln.DotSetting.User");
        //    return settingsFiles.Length > 0;
        //}

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep15()
        {
            return !TypicalChecks.StringExists(Solution, "Tutorial4_WhatsNewReSharper2017.1", "CodeFormatting.cs",
                "foo){");
        }
    }
}