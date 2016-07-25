using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.UI.Application;

namespace ReSharperTutorials.CodeNavigator
{
    public class SourceCodeNavigator
    {
        private readonly Lifetime _lifetime;
        private readonly ISolution _solution;
        private readonly IPsiFiles _psiFiles;
        private readonly TextControlManager _textControlManager;
        private readonly IShellLocks _shellLocks;
        private readonly IEditorManager _editorManager;
        private readonly DocumentManager _documentManager;
        private readonly IUIApplication _environment;
        private int _psiTimestamp;

        public SourceCodeNavigator(Lifetime lifetime, ISolution solution, IPsiFiles psiFiles,
                                  TextControlManager textControlManager, IShellLocks shellLocks,
                                  IEditorManager editorManager, DocumentManager documentManager, IUIApplication environment)
        {            
            _lifetime = lifetime;
            _solution = solution;
            _psiFiles = psiFiles;
            _textControlManager = textControlManager;
            _shellLocks = shellLocks;
            _documentManager = documentManager;
            _environment = environment;
            _editorManager = editorManager;                                                                          
        }


        public void Navigate(TutStep.TutorialStep step)
        {
            if (step.NavNode == null) 
                return;
            if (step.NavNode.TypeName == null && step.NavNode.MethodName == null && step.NavNode.TextToFind == null)
                return;

            _shellLocks.ExecuteOrQueueReadLock(_lifetime, "Navigate", () =>
            {                
                _psiFiles.CommitAllDocumentsAsync(() =>
                {
                    var project = PsiNavigationHelper.GetProjectByName(Solution, step.NavNode.ProjectName);

                    var file = PsiNavigationHelper.GetCSharpFile(project, step.NavNode.FileName);

                    var node = PsiNavigationHelper.GetTreeNodeForStep(file, step.NavNode.TypeName, step.NavNode.MethodName,
                        step.NavNode.MethodNameOccurrence, step.NavNode.TextToFind, step.NavNode.TextToFindOccurrence);

                    NavigateToNode(node, true);
                });                                
            });
        }
    
        
        private void NavigateToNode(ITreeNode treeNode, bool activate)
        {
            //            if (!IsUpToDate()) return;
            if (treeNode == null) return;

            var range = treeNode.GetDocumentRange();
            if (!range.IsValid()) return;                

            var projectFile = _documentManager.TryGetProjectFile(range.Document);
            if (projectFile == null) return;

            var textControl = _editorManager.OpenProjectFile(projectFile, activate);

            textControl.Caret.MoveTo(range.TextRange.StartOffset, CaretVisualPlacement.DontScrollIfVisible);

            textControl.Selection.SetRange(range.TextRange);
        }



        public bool IsUpToDate()
        {
            // PsiTimestamp is a time stamp for PsiFiles - use it to check the current PSI is up to date
            return _psiTimestamp == Solution.GetPsiServices().Files.PsiTimestamp;
        }

        public ISolution Solution
        {
            get { return _solution; }
        }
    }
}
