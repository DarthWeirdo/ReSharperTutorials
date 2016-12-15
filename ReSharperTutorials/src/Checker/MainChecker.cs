using System;
using System.Reflection;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.Application.changes;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.TextControl;
using JetBrains.UI.Application;
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
            ChangeManager changeManager, TextControlManager textControlManager, IShellLocks shellLocks,
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

            if (step.Check.Actions != null)
                _stepActionChecker = new StepActionChecker(lifetime, shellLocks, psiFiles, actionManager);

            if (step.Check.Method == null) return;
            _stepPsiChecker = new StepPsiChecker(lifetime, solution, psiFiles, changeManager, textControlManager,
                shellLocks, editorManager,
                documentManager, environment);
            _stepNavigationChecker = new StepNavigationChecker(lifetime, solution, psiFiles, textControlManager,
                shellLocks, editorManager, documentManager, environment);
        }


        public void PerformStepChecks()
        {
            var attr = new RunCheckAttribute(OnEvent.None);

            if (_currentStep.Check.Method != null)
            {
                var typeName = PsiNavigationHelper.GetLongNameFromFqn(_currentStep.Check.Method);
                var methodName = PsiNavigationHelper.GetShortNameFromFqn(_currentStep.Check.Method);
                var customType = Type.GetType(typeName);
                if (customType == null)
                    throw new ApplicationException("Unknown custom check class. Try reinstalling the plugin.");

                var mInfo = customType.GetMethod(methodName);
                if (mInfo != null)
                {
                    var parameterArray = new object[] {_solution, _editorManager, _documentManager, _textControlManager};
                    var customInst = Activator.CreateInstance(customType, parameterArray);
                    var checkMethod = (Func<bool>) Delegate.CreateDelegate(typeof(Func<bool>), customInst, mInfo);
                    attr = (RunCheckAttribute) mInfo.GetCustomAttribute(typeof(RunCheckAttribute));

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
                            _stepActionChecker.StepActionNames = _currentStep.Check.Actions;
                            _stepActionChecker.Check = checkMethod;
                            _stepActionChecker.AfterActionApplied.Advise(_lifetime,
                                () =>
                                {
                                    _currentStep.IsActionDone = true;
                                    _currentStep.IsCheckDone = true;
                                });
                            break;
                        case OnEvent.None:
                            throw new ApplicationException(
                                $"Unable to run the check: method {_currentStep.Check} is not associated with any event");
                        default:
                            throw new ApplicationException(
                                $"Unable to run the check: method {_currentStep.Check} must be marked with the RunCheckAttribute");
                    }
                }
                else
                    throw new ApplicationException(
                        $"Unable to find the checker method {_currentStep.Check}. Please reinstall the plugin.");
            }

            if (_currentStep.Check.Actions == null || attr.OnEvent == OnEvent.AfterAction) return;
            _stepActionChecker.StepActionNames = _currentStep.Check.Actions;
            _stepActionChecker.AfterActionApplied.Advise(_lifetime,
                () => { _currentStep.IsActionDone = true; });
        }
    }
}