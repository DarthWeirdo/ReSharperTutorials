using System;
using System.Diagnostics;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.TextControl.Coords;
using ReSharperTutorials.Runner;
using ReSharperTutorials.Utils;

namespace ReSharperTutorials.CodeNavigator
{
    class Tutorial4Navigation : ICustomNavigation
    {
        public ISolution Solution { get; set; }
        public IEditorManager EditorManager { get; set; }
        public DocumentManager DocumentManager { get; set; }

        public Tutorial4Navigation(ISolution solution, IEditorManager editorManager, DocumentManager documentManager)
        {
            Solution = solution;
            EditorManager = editorManager;
            DocumentManager = documentManager;
        }

        public void NavigateStep9()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial4_WhatsNewReSharper2017.1");
            var file = PsiNavigationHelper.GetCSharpFile(project, "LocalFunctions.cs");
            var node = PsiNavigationHelper.GetTreeNodeForStep(file, "ReSharper20171.LocalFunctions",
                "Factorial", 1, null, 0);

            var methodDecl = (IMethodDeclaration) node?.Parent;
            node = methodDecl?.Body.Statements[0];
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.StartOffset + 5, CaretVisualPlacement.Generic);
        }

        public void NavigateStep13()
        {
            var dte = VsIntegration.GetCurrentVsInstance();
            var path = GlobalSettings.Instance.GetPath(4, PathType.WorkCopySolutionFolder) +
                    "\\Tutorial4_WhatsNewReSharper2017.1\\.editorconfig";
            dte.ExecuteCommand("File.OpenFile", path);
        }

        public void NavigateStep11_14_15()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial4_WhatsNewReSharper2017.1");
            var file = PsiNavigationHelper.GetCSharpFile(project, "CodeFormatting.cs");

            Debug.Assert(file != null, "file != null");
            var namespaceDecl =
                file.NamespaceDeclarations.FirstOrDefault(
                    namespaceDeclaration => namespaceDeclaration.ShortName == "ReSharper20171");

            Debug.Assert(namespaceDecl != null, "namespaceDecl != null");
            var typeDecl =
                namespaceDecl.TypeDeclarations.FirstOrDefault(
                    declaration => declaration.CLRName == "ReSharper20171.CodeFormatting");

            var range = typeDecl.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;

            Debug.Assert(typeDecl != null, "typeDecl != null");
            Debug.Assert(typeDecl.FirstChild != null, "typeDecl.FirstChild != null");
            var startPos = typeDecl.FirstChild.GetNavigationRange().StartOffset.Offset;
            Debug.Assert(typeDecl.LastChild != null, "typeDecl.LastChild != null");
            var endPos = typeDecl.LastChild.GetNavigationRange().EndOffset.Offset;

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