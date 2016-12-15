using System;
using System.Diagnostics;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.TextControl.Coords;

namespace ReSharperTutorials.CodeNavigator
{
    class Tutorial3Navigation : ICustomNavigation
    {
        public ISolution Solution { get; set; }
        public IEditorManager EditorManager { get; set; }
        public DocumentManager DocumentManager { get; set; }

        public Tutorial3Navigation(ISolution solution, IEditorManager editorManager, DocumentManager documentManager)
        {
            Solution = solution;
            EditorManager = editorManager;
            DocumentManager = documentManager;
        }

        public void NavigateStep5()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial3_WhatsNewReSharper2016.3");
            var file = PsiNavigationHelper.GetCSharpFile(project, "CodeGeneration.cs");
            var node = PsiNavigationHelper.GetTreeNodeForStep(file, "ReSharper20163.CodeGenerationIComparable+ShoeSize",
                null, 0, null, 0);
            var classDecl = (IClassDeclaration) node?.Parent;
            node = classDecl?.Body.LastChild?.PrevSibling?.PrevSibling?.PrevSibling;
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var text = $"{Environment.NewLine}\t\t\t";

            document.InsertText(range.TextRange.EndOffset, text);
            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.EndOffset + text.Length, CaretVisualPlacement.Generic);
        }

        public void NavigateStep6_7()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial3_WhatsNewReSharper2016.3");
            var file = PsiNavigationHelper.GetCSharpFile(project, "CodeGeneration.cs");
            var node = PsiNavigationHelper.GetTreeNodeForStep(file, "ReSharper20163.CodeGenerationIComparable+Account",
                null, 0, null, 0);
            var classDecl = (IClassDeclaration) node?.Parent;
            node = classDecl?.Body.LastChild?.PrevSibling?.PrevSibling?.PrevSibling;
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var text = $"{Environment.NewLine}\t\t\t";
            document.InsertText(range.TextRange.EndOffset, text);

            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.EndOffset + text.Length, CaretVisualPlacement.Generic);
        }

        public void NavigateStep9()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial3_WhatsNewReSharper2016.3");
            var file = PsiNavigationHelper.GetCSharpFile(project, "CodeGeneration.cs");
            var node = PsiNavigationHelper.GetTreeNodeForStep(file, "ReSharper20163.GenerateConstructorCheckForNull",
                null, 0, null, 0);
            var classDecl = (IClassDeclaration) node?.Parent;
            node = classDecl?.Body.LastChild?.PrevSibling?.PrevSibling?.PrevSibling;
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var text = $"{Environment.NewLine}\t\t";
            document.InsertText(range.TextRange.EndOffset, text);

            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.EndOffset + text.Length, CaretVisualPlacement.Generic);
        }

        public void NavigateStep17()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial3_WhatsNewReSharper2016.3");
            var file = PsiNavigationHelper.GetCSharpFile(project, "LanguageInjections.cs");
            var node = PsiNavigationHelper.GetTreeNodeForStep(file, "ReSharper20163.LanguageInjections", null, 0,
                "Style", 1);
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.EndOffset + 5, CaretVisualPlacement.DontScrollIfVisible);
        }

        public void NavigateStep18()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial3_WhatsNewReSharper2016.3");
            var file = PsiNavigationHelper.GetCssFile(project, "LanguageInjections.cs", 1);
            var node = PsiNavigationHelper.GetAnyTreeNodeForStep(file, "red", 1);

            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.EndOffset, CaretVisualPlacement.DontScrollIfVisible);
        }

        public void NavigateStep19()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial3_WhatsNewReSharper2016.3");
            var file = PsiNavigationHelper.GetCSharpFile(project, "LanguageInjections.cs");
            var node = PsiNavigationHelper.GetTreeNodeForStep(file, "ReSharper20163.LanguageInjections", null, 0,
                "private", 4);
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.StartOffset - 9, CaretVisualPlacement.DontScrollIfVisible);
        }

        public void NavigateStep20()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial3_WhatsNewReSharper2016.3");
            var file = PsiNavigationHelper.GetCSharpFile(project, "LanguageInjections.cs");
            var node = PsiNavigationHelper.GetTreeNodeForStep(file, "ReSharper20163.LanguageInjections", null, 0,
                "private", 6);
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.StartOffset - 9, CaretVisualPlacement.DontScrollIfVisible);
        }

        public void NavigateStep25()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial3_WhatsNewReSharper2016.3");
            var file = PsiNavigationHelper.GetCSharpFile(project, "MatchSimilarConstructs.cs");
            var node = PsiNavigationHelper.GetTreeNodeForStep(file, "ReSharper20163.MatchSimilarConstructs", null, 0,
                "Second", 1);
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;

            var startPos = range.TextRange.StartOffset - 5;
            var endPos = range.TextRange.EndOffset;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            if (textControl == null) return;
            textControl.Caret.MoveTo(range.TextRange.StartOffset - 5, CaretVisualPlacement.DontScrollIfVisible);
            var selection = new[]
            {
                new TextControlPosRange(textControl.Coords.FromDocOffset(startPos),
                    textControl.Coords.FromDocOffset(endPos))
            };
            textControl.Selection.SetRanges(selection);
        }

        public void NavigateStep27()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial3_WhatsNewReSharper2016.3");
            var file = PsiNavigationHelper.GetCSharpFile(project, "InterpolatedStringsImprovement.cs");
            var node = PsiNavigationHelper.GetTreeNodeForStep(file, "ReSharper20163.InterpolatedStringsImprovement",
                null, 0, "=", 1);
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.EndOffset + 1, CaretVisualPlacement.DontScrollIfVisible);
        }

        public void NavigateStep29()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial3_WhatsNewReSharper2016.3");
            var file = PsiNavigationHelper.GetCSharpFile(project, "JoinLines.cs");

            Debug.Assert(file != null, "file != null");
            var namespaceDecl =
                file.NamespaceDeclarations.FirstOrDefault(
                    namespaceDeclaration => namespaceDeclaration.ShortName == "ReSharper20163");

            Debug.Assert(namespaceDecl != null, "namespaceDecl != null");
            var typeDecl =
                namespaceDecl.TypeDeclarations.FirstOrDefault(
                    declaration => declaration.CLRName == "ReSharper20163.JoinLines");

            var range = typeDecl.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;

            Debug.Assert(typeDecl != null, "typeDecl != null");
            var methodDecl = (IPropertyDeclaration) typeDecl.MemberDeclarations.FirstOrDefault(memberDeclaration =>
                memberDeclaration.DeclaredName == "MyProperty");

            Debug.Assert(methodDecl != null, "methodDecl != null");
            Debug.Assert(methodDecl.FirstChild != null, "methodDecl.FirstChild != null");
            var startPos = methodDecl.FirstChild.GetNavigationRange().StartOffset.Offset;
            Debug.Assert(methodDecl.LastChild != null, "methodDecl.LastChild != null");
            var endPos = methodDecl.LastChild.GetNavigationRange().EndOffset.Offset;

            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            if (textControl == null) return;
            textControl.Caret.MoveTo(range.TextRange.EndOffset, CaretVisualPlacement.DontScrollIfVisible);
            var selection = new[]
            {
                new TextControlPosRange(textControl.Coords.FromDocOffset(startPos),
                    textControl.Coords.FromDocOffset(endPos))
            };
            textControl.Selection.SetRanges(selection);
        }
    }
}