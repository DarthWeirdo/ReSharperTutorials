using System;
using System.Reflection;
using System.Text;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.UI.Application;
using JetBrains.Util;
using ReSharperTutorials.CodeNavigator;

namespace ReSharperTutorials.Checker
{
    internal class MainChecker
    {
        private readonly Lifetime _lifetime;
        private readonly ISolution _solution;
        private readonly IPsiFiles _psiFiles;
        private readonly TextControlManager _textControlManager;
        private readonly IShellLocks _shellLocks;
        private readonly IEditorManager _editorManager;
        private readonly DocumentManager _documentManager;
        private readonly IUIApplication _environment;
        private readonly IActionManager _actionManager;
        private int _psiTimestamp;
        
        /// <summary>
        /// Use to identify whether a user applied an action specified in the step
        /// </summary>
        private readonly StepActionChecker _stepActionChecker;

        /// <summary>
        /// Use to perform checks that use PSI tree
        /// </summary>
        private readonly StepPsiChecker _stepPsiChecker;

        /// <summary>
        /// Use to identify whether a user navigated to the right part of code
        /// </summary>
        private readonly StepNavigationChecker _stepNavigationChecker;

        private readonly TutStep.TutorialStep _currentStep;
                

        public MainChecker(Lifetime lifetime, TutStep.TutorialStep step, ISolution solution, IPsiFiles psiFiles,
                                  TextControlManager textControlManager, IShellLocks shellLocks,
                                  IEditorManager editorManager, DocumentManager documentManager, IActionManager actionManager,  
                                  IUIApplication environment)
        {
            _lifetime = lifetime;
            _currentStep = step;
            _solution = solution;
            _psiFiles = psiFiles;
            _textControlManager = textControlManager;
            _shellLocks = shellLocks;
            _documentManager = documentManager;
            _environment = environment;
            _editorManager = editorManager;
            _actionManager = actionManager;

            _stepActionChecker = new StepActionChecker(lifetime, shellLocks, psiFiles, actionManager);
            _stepPsiChecker = new StepPsiChecker(lifetime, solution, psiFiles, textControlManager, shellLocks, editorManager, 
                documentManager, environment);
            _stepNavigationChecker = new StepNavigationChecker(lifetime, solution, psiFiles, textControlManager, shellLocks, editorManager, documentManager, environment);
        }


        public void PerformStepChecks()
        {            
            var attr = new RunCheckAttribute(OnEvent.None);

            if (_currentStep.Check != null)
            {
                var mInfo = typeof (MainChecker).GetMethod(_currentStep.Check);
                if (mInfo != null)
                {
                    var checkMethod = (Func<bool>) Delegate.CreateDelegate(typeof (Func<bool>), this, mInfo);
                    attr = (RunCheckAttribute) mInfo.GetCustomAttribute(typeof (RunCheckAttribute));

                    switch (attr.OnEvent)
                    {
                        case OnEvent.PsiChange:
                            _stepPsiChecker.Check = checkMethod;
                            _stepPsiChecker.AfterPsiChangesDone.Advise(_lifetime,
                                () => { _currentStep.IsCheckDone = true; });
                            break;
                        case OnEvent.CaretMove:
                            _stepNavigationChecker.Check = checkMethod;
                            _stepNavigationChecker.AfterNavigationDone.Advise(_lifetime,
                                () => { _currentStep.IsCheckDone = true; });                            
                            break;
                        case OnEvent.AfterAction:
                            _stepActionChecker.StepActionName = _currentStep.Action;
                            _stepActionChecker.Check = checkMethod;
                            _stepActionChecker.AfterActionApplied.Advise(_lifetime,
                                () =>
                                {
                                    _currentStep.IsActionDone = true;
                                    _currentStep.IsCheckDone = true;                                    
                                });
                            break;
                        case OnEvent.None:
                            throw new Exception(
                                $"Unable to run the check: method {_currentStep.Check} is not associated with any event");       
                        default:
                            throw new ArgumentOutOfRangeException(
                                $"Unable to run the check: method {_currentStep.Check} must be marked with the RunCheckAttribute");
                    }
                }
                else
                    throw new Exception($"Unable to find the checker method {_currentStep.Check}. Please reinstall the plugin.");
            }

            if (_currentStep.Action == null || attr.OnEvent == OnEvent.AfterAction) return;
            _stepActionChecker.StepActionName = _currentStep.Action;
            _stepActionChecker.AfterActionApplied.Advise(_lifetime, 
                () => { _currentStep.IsActionDone = true; });
        }
       

        #region Typical Checks

        /// <summary>
        /// Converts the entire IFile to string and checks whether it contains $text$
        /// </summary>                
        private bool StringExists(string text, string fileName = null)
        {
            if (fileName == null)
                fileName = _currentStep.FileName;
            return ConvertFileToString(fileName).Contains(text);
        }
        
        private string ConvertFileToString(string fileName)
        {
            var result = new StringBuilder();

            _shellLocks.TryExecuteWithReadLock(() =>
            {
                var project = PsiNavigationHelper.GetProjectByName(_solution, _currentStep.ProjectName);
                var file = PsiNavigationHelper.GetCSharpFile(project, fileName);
                
                if (file == null) return;
                var treeNodeList = file.EnumerateTo(file.LastChild);

                foreach (var node in treeNodeList)
                {
                    result.AppendSlice(node.GetText());
                }
            });

            return result.ToString();
        }
       
        
        /// <summary>
        /// Finds type declaration in scope specified in the current step
        /// </summary>
        /// <returns>Returns true if $typeName$ is found</returns>
        private bool TypeDeclarationExists(string typeName, string fileName = null)
        {
            ITreeNode node = null;
            if (fileName == null)            
                fileName = _currentStep.FileName;
                        
            _shellLocks.TryExecuteWithReadLock(() =>
            {
                var project = PsiNavigationHelper.GetProjectByName(_solution, _currentStep.ProjectName);                
                var file = PsiNavigationHelper.GetCSharpFile(project, fileName);
                node = PsiNavigationHelper.GetTypeNodeByFullClrName(file, typeName);
            });

            return node != null;
        }


        /// <summary>
        /// Finds method declaration in scope specified in the current step
        /// </summary>
        /// <returns>Returns true if $typeName$ is found</returns>
        private bool MethodDeclarationExists(string typeName, string methodName, string fileName = null)
        {
            ITreeNode node = null;
            if (fileName == null)
                fileName = _currentStep.FileName;

            _shellLocks.TryExecuteWithReadLock(() =>
            {
                var project = PsiNavigationHelper.GetProjectByName(_solution, _currentStep.ProjectName);
                var file = PsiNavigationHelper.GetCSharpFile(project, fileName);
                node = PsiNavigationHelper.GetMethodNodeByFullClrName(file, typeName, methodName);
            });

            return node != null;
        }


        /// <summary>
        /// Finds text of a tree node in scope specified in the current step
        /// </summary>        
        /// <returns>Returns true if specific $occurrence$ of $text$ is found</returns>        
        private bool TreeNodeWithTextExists(string text, int occurrence, string fileName = null)
        {
            ITreeNode node = null;
            if (fileName == null)
                fileName = _currentStep.FileName;

            _shellLocks.TryExecuteWithReadLock(() =>
            {
                var project = PsiNavigationHelper.GetProjectByName(_solution, _currentStep.ProjectName);
                var file = PsiNavigationHelper.GetCSharpFile(project, fileName);
                node = PsiNavigationHelper.GetTreeNodeForStep(file, _currentStep.TypeName, _currentStep.MethodName, text, occurrence);
            });

            return node != null;
        }

        #endregion

        #region Custom Step Checks. This must be a 'public bool' method that returns true if the check passes. The method MUST be marked with RunCheckAttribute. This attribute defines HOW a check will be TRIGGERED: OnEvent.AfterAction if it should be triggered by the action specified in the step (NOT WORKING CURRENTLY); OnEvent.PsiChange if it should be triggered by any Psi tree change; OnEvent.CaretMove if it should be triggered by any caret position change.

        /// <summary>
        /// Example of a PSI check
        /// </summary>
        /// <returns>Returns true if BadlyNamedClass is found</returns>
        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step2()
        {
            return TypeDeclarationExists("Tutorial1_EssentialShortcuts.BadlyNamedClass");
        }


        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step3()
        {            
            return StringExists("string.Format");
        }


        [RunCheck(OnEvent.PsiChange)]
        public bool CheckTutorial1Step4()
        {
            return TypeDeclarationExists("Tutorial1_EssentialShortcuts.Renamed");
        }


        [RunCheck(OnEvent.CaretMove)]
        public bool CheckTutorial1Step5()
        {
            var node = _stepNavigationChecker.GetTreeNodeUnderCaret();
            var parentNode = node?.Parent as ITypeDeclaration;
            
            return parentNode != null && parentNode.DeclaredName == "SomeClass";
        }

        #endregion
    }
}
