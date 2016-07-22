using System;
using System.Collections.Generic;
using System.Windows;
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
        public readonly Lifetime Lifetime;
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


        public TutorialStepPresenter(IStepView view, string contentPath, Lifetime lifetime, ISolution solution, IPsiFiles psiFiles,
            ChangeManager changeManager, TextControlManager textControlManager, IShellLocks shellLocks, IEditorManager editorManager,
            DocumentManager documentManager, IUIApplication environment, IActionManager actionManager,
            IPsiServices psiServices, IActionShortcuts shortcutManager)
        {
            _stepView = view;
            Lifetime = lifetime;
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
            _codeNavigator = new SourceCodeNavigator(lifetime, solution, psiFiles, textControlManager, shellLocks, editorManager,
                documentManager, environment);                        
            _steps = new Dictionary<int, TutorialStep>();
            _steps = TutorialXmlReader.ReadTutorialSteps(contentPath);
            Title = TutorialXmlReader.ReadTitle(contentPath);

            var converter = new ActionToShortcutConverter(actionManager);
            foreach (var step in _steps.Values)            
                step.Text = converter.SubstituteShortcutsViaVs(step.Text);                        

            // always start from the beginning
            // _currentStepId = TutorialXmlReader.ReadCurrentStep(contentPath);
            _currentStepId = 1; 
            CurrentStep = _steps[_currentStepId];
            _stepView.StepCount = _steps.Count;            

            lifetime.AddBracket(
                () => { _stepView.NextStep += GoNext; }, 
                () => { _stepView.NextStep -= GoNext; });            

            ProcessStep();

        }

        public void Close(object sender, RoutedEventArgs args)
        {
            VsIntegration.CloseVsSolution(true);         
        }        

        private void GoNext(object sender, EventArgs args)
        {
            if (_currentStepId == _steps.Count) return;

            _currentStepId++;
            CurrentStep = _steps[_currentStepId];            
            ProcessStep();
        }

        private void ProcessStep()
        {
            ShowText(CurrentStep);
            _codeNavigator.Navigate(CurrentStep);
            _stepView.UpdateProgress();

            if (CurrentStep.GoToNextStep != GoToNextStep.Auto) return;
            CurrentStep.StepIsDone += StepOnStepIsDone;
            CurrentStep.PerformChecks(this);
        }


        private void StepOnStepIsDone(object sender, EventArgs eventArgs)
        {
            CurrentStep.StepIsDone -= StepOnStepIsDone;
            GoNext(this, null);            
        }


        private void ShowText(TutorialStep step)
        {
            var result = $"<div id =\"currentStep\">{step.Text}</div>";

            if (step.Id > 1)
            {
                var prevStep = _steps[step.Id - 1];
                if ((prevStep.StrikeOnDone && step.GoToNextStep == GoToNextStep.Auto) || (prevStep.StrikeOnDone && step.Id == _steps.Count))    
                    result = $"<div id=\"prevStep\">{prevStep.Text}</div> <div id=\"currentStep\">{step.Text}</div>";                
            }

            _stepView.StepText = result;
        }        

    }
}
