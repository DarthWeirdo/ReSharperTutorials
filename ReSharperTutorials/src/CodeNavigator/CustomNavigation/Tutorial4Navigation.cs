using System;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;

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
           
            var methodDecl = (IMethodDeclaration)node?.Parent;
            node = methodDecl?.Body.Statements[0];            
            var range = node.GetDocumentRange();
            if (!range.IsValid()) return;
            var document = range.Document;                        
            var projectFile = DocumentManager.TryGetProjectFile(document);
            if (projectFile == null) return;
            var textControl = EditorManager.OpenProjectFile(projectFile, true);
            textControl?.Caret.MoveTo(range.TextRange.StartOffset + 5, CaretVisualPlacement.Generic);
        }

    }
}
