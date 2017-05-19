using System.Linq;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharperTutorials.CodeNavigator;

namespace ReSharperTutorials.Checker
{
    /// <summary>
    /// Custom Step Checks. Must contain 'public bool' methods that return true if the check passes. 
    /// Each method MUST be marked with RunCheckAttribute. This attribute defines HOW a check will be TRIGGERED: 
    /// OnEvent.AfterAction if it should be triggered by the action specified in the step (NOT WORKING CURRENTLY); 
    /// OnEvent.PsiChange if it should be triggered by any Psi tree change; 
    /// OnEvent.CaretMove if it should be triggered by any caret position change.
    /// </summary>
    class Tutorial1Checks : ICustomCheck
    {
        public ISolution Solution { get; set; }
        public IEditorManager EditorManager { get; set; }
        public ITextControlManager TextControlManager { get; set; }
        public DocumentManager DocumentManager { get; set; }

        public Tutorial1Checks(ISolution solution, IEditorManager editorManager, DocumentManager documentManager,
            ITextControlManager textControlManager)
        {
            Solution = solution;
            EditorManager = editorManager;
            DocumentManager = documentManager;
            TextControlManager = textControlManager;
        }

        /// <summary>
        /// Example of a PSI check
        /// </summary>
        /// <returns>Returns true if BadlyNamedClass is found</returns>
        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step4()
        {
            return TypicalChecks.TypeDeclarationExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "Tutorial1_EssentialShortcuts.BadlyNamedClass");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step5()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs", "string.Format");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step6()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs", "public class Renamed");           
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step9()
        {
            return TypicalChecks.TypeDeclarationExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "Tutorial1_EssentialShortcuts.MyCircle");
        }

        [RunCheck(OnEvent.CaretMove)]
        public bool CheckTutorial1Step10()
        {
            return TypicalChecks.TypeDeclarationExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "Tutorial1_EssentialShortcuts.MyCircle");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step11()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "public MyCircle()");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step12()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "public TYPE Type");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step13()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "public int Type");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step14()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "public int I");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step15()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "public int Radius");
        }

        [RunCheck(OnEvent.CaretMove)]
        public bool CheckTutorial1Step16()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "public int Radius");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step17()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "public TYPE Type");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step18()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "public CenterCoordinates Type");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step19()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "public CenterCoordinates CenterCoordinates");
        }

        [RunCheck(OnEvent.CaretMove)]
        public bool CheckTutorial1Step20()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "public CenterCoordinates CenterCoordinates");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step23()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "protected bool Equals(MyCircle");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step26()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "MyCircle.cs",
                "class MyCircle");
        }

        [RunCheck(OnEvent.CaretMove)]
        public bool CheckTutorial1Step2832()
        {
            var node = TypicalChecks.GetTreeNodeUnderCaret(DocumentManager, TextControlManager);
            var parentNode = node?.Parent as ITypeDeclaration;
            return parentNode != null && parentNode.DeclaredName == "CenterCoordinates";
        }

        [RunCheck(OnEvent.CaretMove)]
        public bool CheckTutorial1Step29()
        {
            var node = TypicalChecks.GetTreeNodeUnderCaret(DocumentManager, TextControlManager);
            var parentNode = node?.Parent as IMultipleFieldDeclaration;
            var decl = parentNode?.Declarators.FirstOrDefault();
            return parentNode != null && decl?.DeclaredName == "_coordinates";
        }

        [RunCheck(OnEvent.CaretMove)]
        public bool CheckTutorial1Step30()
        {
            var textControl = TextControlManager.CurrentFrameTextControl.Value;
            var range = textControl.Document.DocumentRange;
            var text = textControl.Document.GetText(range);
            return text.Contains("class MyCircle");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step34()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs",
                "ReturnString(int intArg");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step35()
        {
            var nodes = PsiNavigationHelper.GetTypeElementsByClrName(Solution,
                "Tutorial1_EssentialShortcuts.ContextAction");

            var node = (from element in nodes
                select element).FirstOrDefault();

            return node?.Methods.AsArray()[2].ShortName == "ReturnCoordinates";
        }
    }
}