using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;

namespace ReSharperTutorials.Checker
{
    /// <summary>
    /// Custom Step Checks. Must contain 'public bool' methods that return true if the check passes. 
    /// The method MUST be marked with RunCheckAttribute. This attribute defines HOW a check will be TRIGGERED: 
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

        public Tutorial1Checks(ISolution solution, IEditorManager editorManager, DocumentManager documentManager, ITextControlManager textControlManager)
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
        public bool CheckTutorial1Step2()
        {
            return TypicalChecks.TypeDeclarationExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs", "Tutorial1_EssentialShortcuts.BadlyNamedClass");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step3()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs", "string.Format");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step4()
        {
            return TypicalChecks.TypeDeclarationExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs", "Tutorial1_EssentialShortcuts.Renamed");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step8()
        {
            return TypicalChecks.TypeDeclarationExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs", "Tutorial1_EssentialShortcuts.MyNewClass");
        }

        [RunCheck(OnEvent.CaretMove)]
        public bool CheckTutorial1Step9()
        {
            return TypicalChecks.TypeDeclarationExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs", "Tutorial1_EssentialShortcuts.MyNewClass");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step10()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs", "public MyNewClass()");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step11()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs", "public SomeClass SomeClass");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step12()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "MyNewClass.cs", "class MyNewClass");
        }

        [RunCheck(OnEvent.CaretMove)]
        public bool CheckTutorial1Step1519()
        {
            var node = TypicalChecks.GetTreeNodeUnderCaret(DocumentManager, TextControlManager);
            var parentNode = node?.Parent as ITypeDeclaration;
            return parentNode != null && parentNode.DeclaredName == "SomeClass";
        }

        [RunCheck(OnEvent.CaretMove)]
        public bool CheckTutorial1Step16()
        {
            var node = TypicalChecks.GetTreeNodeUnderCaret(DocumentManager, TextControlManager);
            var parentNode = node?.Parent as IMultipleFieldDeclaration;
            var decl = parentNode?.Declarators.FirstOrDefault();
            return parentNode != null && decl?.DeclaredName == "_someField";
        }

        [RunCheck(OnEvent.CaretMove)]
        public bool CheckTutorial1Step17()
        {            
            var node = TypicalChecks.GetTreeNodeUnderCaret(DocumentManager, TextControlManager);
            var parentNode = node?.Parent as ITypeDeclaration;
            return parentNode != null && parentNode.DeclaredName == "MyNewClass";
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step21()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial1_EssentialShortcuts", "Essentials.cs", "ReturnString(int intArg");
        }

    }
}
