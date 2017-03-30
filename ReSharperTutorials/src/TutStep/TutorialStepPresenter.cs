using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.Application.changes;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.TextControl;
using JetBrains.UI.ActionsRevised.Shortcuts;
using JetBrains.UI.Application;
using ReSharperTutorials.CodeNavigator;
using ReSharperTutorials.Utils;

namespace ReSharperTutorials.TutStep
{
    public class TutorialStepPresenter
    {
        private readonly IStepView _stepView;
        private readonly SourceCodeNavigator _codeNavigator;
        private readonly string _contentPath;
        private readonly Dictionary<int, TutorialStep> _steps;
        private int _currentStepId;
        private readonly Lifetime _lifetime;
        public readonly ISolution Solution;
        public readonly IPsiFiles PsiFiles;
        public readonly ChangeManager ChangeManager;
        public readonly TextControlManager TextControlManager;
        public readonly IShellLocks ShellLocks;
        public readonly IEditorManager EditorManager;
        public readonly DocumentManager DocumentManager;
        public readonly IUIApplication Environment;
        public readonly IActionManager ActionManager;
        public readonly string Title;
        public TutorialStep CurrentStep { get; set; }
        public bool IsLastStep => _currentStepId == _steps.Count;
        /// <summary>
        /// Lifetime created for the duration of performing checks in current step
        /// </summary>
        private LifetimeDefinition _checksLifetime;


        public TutorialStepPresenter(IStepView view, string contentPath, Lifetime lifetime, ISolution solution,
            IPsiFiles psiFiles,
            ChangeManager changeManager, TextControlManager textControlManager, IShellLocks shellLocks,
            IEditorManager editorManager,
            DocumentManager documentManager, IUIApplication environment, IActionManager actionManager,
            IPsiServices psiServices, IActionShortcuts shortcutManager)
        {            
            _stepView = view;
            _lifetime = lifetime;
            Solution = solution;
            PsiFiles = psiFiles;
            ChangeManager = changeManager;
            TextControlManager = textControlManager;
            ShellLocks = shellLocks;
            EditorManager = editorManager;
            DocumentManager = documentManager;
            Environment = environment;
            ActionManager = actionManager;
            _contentPath = contentPath;
            _codeNavigator = new SourceCodeNavigator(lifetime, solution, psiFiles, textControlManager, shellLocks,
                editorManager,
                documentManager, environment);
            _steps = new Dictionary<int, TutorialStep>();

            var tutorialXmlReader = new TutorialXmlReader(actionManager);
            _steps = tutorialXmlReader.ReadTutorialSteps(contentPath);
            Title = TutorialXmlReader.ReadTitle(contentPath);

            //var converter = new ActionToShortcutConverter(actionManager);
            //foreach (var step in _steps.Values)
            //    step.Text = converter.SubstituteShortcutsViaVs(step.Text);
            //step.Text = converter.SubstituteShortcuts(step.Text);

            // always start from the beginning
            // _currentStepId = TutorialXmlReader.ReadCurrentStep(contentPath);
            _currentStepId = 1;
            CurrentStep = _steps[_currentStepId];
            _stepView.StepCount = _steps.Count;

            lifetime.AddBracket(                
                () => { _stepView.NextStep += StepOnStepIsDone; },
                () => { _stepView.NextStep -= StepOnStepIsDone; });

            ProcessCurrentStep();
        }

        public void Close(object sender, RoutedEventArgs args)
        {
            VsIntegration.CloseVsSolution(true);
        }

        public void RunStepNavigation()
        {
            _codeNavigator.Navigate(CurrentStep);
        }

        private void GoToNextStep(object sender, EventArgs args)
        {
            if (_currentStepId == _steps.Count) return;

            _currentStepId++;
            CurrentStep = _steps[_currentStepId];
            ProcessCurrentStep();
        }

        private void ProcessCurrentStep()
        {
            ShowText(CurrentStep);
            _codeNavigator.Navigate(CurrentStep);
            _stepView.UpdateProgress();

            CurrentStep.StepIsDone += StepOnStepIsDone;
            _checksLifetime = Lifetimes.Define(_lifetime);
            CurrentStep.PerformChecks(_checksLifetime.Lifetime, this);
        }


        private void StepOnStepIsDone(object sender, EventArgs eventArgs)
        {
            CurrentStep.StepIsDone -= StepOnStepIsDone;            
            _checksLifetime.Terminate();
            GoToNextStep(this, null);
        }


        private void ShowText(TutorialStep step)
        {
            var result = $"<div id =\"currentStep\" class =\"currentStep\">{step.Text}</div>";

            if (step.Id > 1)
            {
                var prevStep = _steps[step.Id - 1];

                if (prevStep.StrikeOnDone)
                    result =
                        $"<div id=\"prevStep\" class=\"prevStep\">{prevStep.Text}</div> <div id=\"currentStep\" class =\"currentStep\">{step.Text}</div>";
            }

            _stepView.StepText = result;
        }
    }
}