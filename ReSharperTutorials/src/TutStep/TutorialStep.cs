using System;
using JetBrains.DataFlow;
using ReSharperTutorials.Checker;

namespace ReSharperTutorials.TutStep
{
    public delegate void StepIsDoneHandler(object sender, EventArgs e);

    public class TutorialStep
    {
        public int Id { get; }
        public bool StrikeOnDone { get; }
        public string Text { get; set; }
        public NavNode NavNode { get; set; }
        public Check Check { get; set; }

        /// <summary>
        /// If GoToNextStep is specified as Manual or not specified, 
        /// a user can proceed to the next step ONLY by clicking the Next button. 
        /// </summary>
        public GoToNextStep GoToNextStep { get; }

        private bool _isActionDone;
        private bool _isCheckDone;
        public event StepIsDoneHandler StepIsDone;

        /// <summary>
        /// Lifetime created for the duration of performing checks
        /// </summary>
        private LifetimeDefinition _processingLifetime;


        public bool IsActionDone
        {
            get { return _isActionDone; }
            set
            {
                if (value == _isActionDone) return;
                _isActionDone = value;

                if (Check.Method == null || IsCheckDone)
                    OnStepIsDone();
            }
        }

        public bool IsCheckDone
        {
            get { return _isCheckDone; }
            set
            {
                if (value == _isCheckDone) return;
                _isCheckDone = value;

                if (Check == null)
                {
                    OnStepIsDone();
                    return;
                }

                if (Check.Actions != null && IsActionDone)
                    OnStepIsDone();
                else if (Check.Actions == null)
                    OnStepIsDone();
            }
        }


        public TutorialStep(int li, NavNode navNode, Check check, string text, string goToNextStep, bool strkieOnDone)
        {
            Id = li;
            NavNode = navNode;
            Text = text;
            Check = check;
            StrikeOnDone = strkieOnDone;
            _processingLifetime = null;

            GoToNextStep = check != null ? GoToNextStep.Auto : GoToNextStep.Manual;
        }


        protected virtual void OnStepIsDone()
        {
            _processingLifetime.Terminate();
            StepIsDone?.Invoke(this, EventArgs.Empty);
        }


        public void PerformChecks(TutorialStepPresenter stepPresenter)
        {
            _processingLifetime = Lifetimes.Define(stepPresenter.Lifetime);

            if (GoToNextStep == GoToNextStep.Auto)
            {
                var checker = new MainChecker(_processingLifetime.Lifetime, this, stepPresenter.Solution,
                    stepPresenter.PsiFiles,
                    stepPresenter.ChangeManager, stepPresenter.TextControlManager, stepPresenter.ShellLocks,
                    stepPresenter.EditorManager,
                    stepPresenter.DocumentManager, stepPresenter.ActionManager, stepPresenter.Environment);

                checker.PerformStepChecks();
            }
            else if (GoToNextStep == GoToNextStep.Manual)
            {
                var nextStepPressChecker =
                    new NextStepPressChecker(_processingLifetime.Lifetime, this, "ReSharper_AltEnter");
            }
        }
    }
}