﻿using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;

namespace ReSharperTutorials.Checker
{
    class Tutorial3Checks: ICustomCheck
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
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "CodeGenerationIDisposable.cs",
                "Dispose(true)");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep5()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "CodeGenerationIComparable.cs",
                "other._sizeCm");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep6()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "CodeGenerationIComparable.cs",
                "other._name");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep7()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "CodeGenerationIComparable.cs",
                "RelationalComparer();");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep9()
        {
            return TypicalChecks.StringExistsInCssFile(Solution, "Tutorial3_WhatsNewReSharper2016.3", "LanguageInjections.cs", 1, "red");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep10()
        {
            return TypicalChecks.StringExistsInCssFile(Solution, "Tutorial3_WhatsNewReSharper2016.3", "LanguageInjections.cs", 1, "rgb");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep11()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "LanguageInjections.cs",
                "javascript");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep12()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "LanguageInjections.cs",
                "postfix=;}");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep14()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "CSharp7.cs",
                "0b11101110");
        }

        [RunCheck(OnEvent.PsiChange)]
        public bool CheckStep15()
        {
            return TypicalChecks.StringExists(Solution, "Tutorial3_WhatsNewReSharper2016.3", "CSharp7.cs",
                "0b111011_");
        }

        [RunCheck(OnEvent.CaretMove)]
        public bool CheckStep17()
        {
            var node = TypicalChecks.GetTreeNodeUnderCaret(DocumentManager, TextControlManager);
            var parentNode = node?.Parent?.Parent as IPropertyDeclaration;            
            return parentNode != null && parentNode.DeclaredName == "Third";
        }
    }
}
