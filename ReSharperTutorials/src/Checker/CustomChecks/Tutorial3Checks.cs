using System.Linq;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using ReSharperTutorials.CodeNavigator;

namespace ReSharperTutorials.Checker
{
    class Tutorial3Checks : ICustomCheck
    {
        public ISolution Solution { get; set; }
        public IEditorManager EditorManager { get; set; }
        public ITextControlManager TextControlManager { get; set; }
        public DocumentManager DocumentManager { get; set; }

        public Tutorial3Checks(ISolution solution, IEditorManager editorManager, DocumentManager documentManager,
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
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "CodeGeneration.cs",
                "Dispose(true)");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep5()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "CodeGeneration.cs",
                "other._sizeCm");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep6()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "CodeGeneration.cs",
                "other._name");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep7()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "CodeGeneration.cs",
                "RelationalComparer();");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep9()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "CodeGeneration.cs",
                "ArgumentNullException(");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep11()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3",
                "IntroduceFromUnusedParameters.cs",
                "Unused1 = unused1;");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep13()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3",
                "IntroducePropertyForLazilyInitialisedField.cs",
                "_foo.Value");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep15()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "TransformParameters.cs",
                "ReturnValue");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep17()
        {
            return TypicalChecks.StringExistsInCssFile(Solution, "Tutorial3_WhatsNewReSharper2016.3",
                "LanguageInjections.cs", 1, "red");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep18()
        {
            return TypicalChecks.StringExistsInCssFile(Solution, "Tutorial3_WhatsNewReSharper2016.3",
                "LanguageInjections.cs", 1, "rgb");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep19()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "LanguageInjections.cs",
                "javascript");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep20()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "LanguageInjections.cs",
                "postfix=;}");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep22()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "CSharp7.cs",
                "0b11101110");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep23()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "CSharp7.cs",
                "0b111011_");
        }

        [RunCheck(OnEvent.CaretMove)]
        public bool CheckStep25()
        {
            var node = TypicalChecks.GetTreeNodeUnderCaret(DocumentManager, TextControlManager);
            var parentNode = node?.Parent?.Parent as IPropertyDeclaration;
            return parentNode != null && parentNode.DeclaredName == "Third";
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep27()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3",
                "InterpolatedStringsImprovement.cs",
                "$\"\"");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep29()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial3_WhatsNewReSharper2016.3");
            var file = PsiNavigationHelper.GetCSharpFile(project, "JoinLines.cs");
            var namespaceDecl = file?.NamespaceDeclarations.FirstOrDefault(
                namespaceDeclaration => namespaceDeclaration.ShortName == "ReSharper20163");
            var typeDecl =
                namespaceDecl?.TypeDeclarations.FirstOrDefault(
                    declaration => declaration.CLRName == "ReSharper20163.JoinLines");
            var methodDecl = (IPropertyDeclaration) typeDecl?.MemberDeclarations.FirstOrDefault(memberDeclaration =>
                memberDeclaration.DeclaredName == "MyProperty");

            var newLines = methodDecl?.ChildrenInSubtrees().Where(node => node.GetText() == "\r\n").ToList();
            return newLines?.Count == 0;
        }
    }
}