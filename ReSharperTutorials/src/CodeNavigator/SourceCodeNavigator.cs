using System;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Rider.Model.Refactorings;
using JetBrains.TextControl;
using JetBrains.UI.Application;
using JetBrains.Util;

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

            if (step.NavNode.RunMethod != null)
            {
                RunCustomNavigation(step.NavNode.RunMethod);
                return;
            }

            if (step.NavNode.TypeName == null && step.NavNode.MethodName == null && step.NavNode.TextToFind == null)
                return;

            _shellLocks.ExecuteOrQueueReadLock(_lifetime, "Navigate", () =>
            {                
                _psiFiles.CommitAllDocumentsAsync(() =>
                {
                    var project = PsiNavigationHelper.GetProjectByName(_solution, step.NavNode.ProjectName);

                    var file = PsiNavigationHelper.GetCSharpFile(project, step.NavNode.FileName);

                    var node = PsiNavigationHelper.GetTreeNodeForStep(file, step.NavNode.TypeName, step.NavNode.MethodName,
                        step.NavNode.MethodNameOccurrence, step.NavNode.TextToFind, step.NavNode.TextToFindOccurrence);

                    if (node == null)
                    {
                        MessageBox.ShowExclamation("OOPS! Someone has changed the code in an unexpected way. " +
                                                   "That wasn't you, right?;) Anyway, now it would be better to restart tutorial.");
                    }
                    else
                    {
                        PsiNavigationHelper.NavigateToNode(_documentManager, _editorManager, node, true);
                    }
                });                                
            });
        }


        private void RunCustomNavigation(string methodFqn)
        {            
            var typeName = PsiNavigationHelper.GetLongNameFromFqn(methodFqn);
            var methodName = PsiNavigationHelper.GetShortNameFromFqn(methodFqn);
            var customType = Type.GetType(typeName);
            if (customType == null)
                throw new ApplicationException("Unknown custom navigation class. Try reinstalling the plugin.");                        
            var navMethod = customType.GetMethod(methodName);
            if (navMethod == null)
                throw new ApplicationException("Unknown custom navigation method. Try reinstalling the plugin."); 
            var parameterArray = new object[] { _solution, _editorManager, _documentManager };
            var customInst = Activator.CreateInstance(customType, parameterArray);

            _shellLocks.ExecuteOrQueueReadLock(_lifetime, "Navigate", () =>
            {
                _psiFiles.CommitAllDocumentsAsync(() => { navMethod.Invoke(customInst, null); });
            });

            //_shellLocks.ExecuteWithWriteLockWhenAvailable(_lifetime, "Navigate", () =>
            //{                                
            //    _psiFiles.CommitAllDocumentsAsync(() => { navMethod.Invoke(customInst, null); });                
            //});
        }
    }
}
