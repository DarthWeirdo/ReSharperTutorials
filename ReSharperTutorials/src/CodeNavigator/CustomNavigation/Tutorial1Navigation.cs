﻿using System;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;

namespace ReSharperTutorials.CodeNavigator
{
    /// <summary>
    /// Custom navigation class MUST be ICustomNavigation and have ISolution, IEditorManager, 
    /// DocumentManager in constructor. 
    /// </summary>
    class Tutorial1Navigation : ICustomNavigation
    {
        public ISolution Solution { get; set; }
        public IEditorManager EditorManager { get; set; }
        public DocumentManager DocumentManager { get; set; }

        public Tutorial1Navigation(ISolution solution, IEditorManager editorManager, DocumentManager documentManager)
        {
            Solution = solution;
            EditorManager = editorManager;
            DocumentManager = documentManager;
        }

        public void NavigateStep3()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial1_EssentialShortcuts");
            var file = PsiNavigationHelper.GetCSharpFile(project, "Essentials.cs");
            var node = file?.FirstChild;
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.StartOffset, CaretVisualPlacement.DontScrollIfVisible);
        }

        public void NavigateStep9()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial1_EssentialShortcuts");
            var file = PsiNavigationHelper.GetCSharpFile(project, "Essentials.cs");
            var node = PsiNavigationHelper.GetTreeNodeForStep(file, "Tutorial1_EssentialShortcuts.CenterCoordinates",
                null, 0, null, 0);
            var classDecl = (IClassDeclaration) node?.Parent;
            node = classDecl?.Body.LastChild;
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var text = $"{Environment.NewLine}\t{Environment.NewLine}\t";
            document.InsertText(range.TextRange.EndOffset, text);

            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.EndOffset + text.Length, CaretVisualPlacement.DontScrollIfVisible);
        }

        public void NavigateStep12()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial1_EssentialShortcuts");
            var file = PsiNavigationHelper.GetCSharpFile(project, "Essentials.cs");
            var node = PsiNavigationHelper.GetTreeNodeForStep(file, "Tutorial1_EssentialShortcuts.MyCircle", null, 0,
                null, 0);
            var classDecl = (IClassDeclaration) node?.Parent;
            node = classDecl?.Body.FirstChild;
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var text = $"{Environment.NewLine}\t\t";
            document.InsertText(range.TextRange.EndOffset, text);

            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.EndOffset + text.Length, CaretVisualPlacement.DontScrollIfVisible);
        }

        public void NavigateStep17()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial1_EssentialShortcuts");
            var file = PsiNavigationHelper.GetCSharpFile(project, "Essentials.cs");
            var node = PsiNavigationHelper.GetTreeNodeForStep(file, "Tutorial1_EssentialShortcuts.MyCircle", null, 0,
                null, 0);
            var classDecl = (IClassDeclaration) node?.Parent;
            node = classDecl?.Body.FirstChild;
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var text = $"{Environment.NewLine}\t\t";
            document.InsertText(range.TextRange.EndOffset, text);

            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.EndOffset + text.Length, CaretVisualPlacement.DontScrollIfVisible);
        }


        public void NavigateStep23()
        {
            var project = PsiNavigationHelper.GetProjectByName(Solution, "Tutorial1_EssentialShortcuts");
            var file = PsiNavigationHelper.GetCSharpFile(project, "Essentials.cs");
            var node = PsiNavigationHelper.GetTreeNodeForStep(file, "Tutorial1_EssentialShortcuts.MyCircle", null, 0,
                null, 0);
            var classDecl = (IClassDeclaration) node?.Parent;
            node = classDecl?.Body.Constructors[0]?.LastChild;
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;
            var text = $"{Environment.NewLine}\t\t";
            document.InsertText(range.TextRange.EndOffset, text);

            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.EndOffset + text.Length, CaretVisualPlacement.DontScrollIfVisible);
        }

        #region Don't forget

        //            var module = GetPsiModuleByName(Solution, project, "Tutorial1_EssentialShortcuts");
        //
        //            var factory = CSharpElementFactory.GetInstance(module);
        //
        //            var st = factory.CreateEmptyBlock();
        //
        //            CSharpGeneratorContext context = CSharpGeneratorContext.CreateContext("myContext", Solution, DocumentManager.);

        // ------------

        //            node = classDecl?.NextSibling?.NextSibling;
        //            range = node.GetDocumentRange();
        //            if (!range.IsValid()) return;
        //            var projectFile = DocumentManager.TryGetProjectFile(document);
        //            if (projectFile == null) return;
        //            var textControl = EditorManager.OpenProjectFile(projectFile, true);
        //            textControl?.Caret.MoveTo(range.TextRange.EndOffset, CaretVisualPlacement.DontScrollIfVisible);

        #endregion
    }
}