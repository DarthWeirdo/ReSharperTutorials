using System;
using JetBrains.DataFlow;
using ReSharperTutorials.Checker;
using ReSharperTutorials.Runner;

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
        /// a user can proceed to the Next Step (Alt+Enter) ONLY by clicking the Next button or pressing Alt+Enter. 
        /// </summary>
        public GoToNextStep GoToNextStep { get; }

        private bool _isActionDone;
        private bool _isCheckDone;
        public event StepIsDoneHandler StepIsDone;

        /// <summary>
        /// Shows whether a user performed the action required by the step 
        /// </summary>
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

        /// <summary>
        /// Shows whether the check required by the step passes or not
        /// </summary>
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

        /// <summary>
        /// Go to next step avoiding all checks
        /// </summary>
        public void ForceStepDone()
        {
            OnStepIsDone();
        }

        public TutorialStep(int li, NavNode navNode, Check check, string text, string goToNextStep, bool strkieOnDone)
        {
            Id = li;
            NavNode = navNode;
            Text = text;
            Check = check;
            StrikeOnDone = strkieOnDone;

            GoToNextStep = check != null ? GoToNextStep.Auto : GoToNextStep.Manual;
        }


        protected virtual void OnStepIsDone()
        {
            StepIsDone?.Invoke(this, EventArgs.Empty);
        }


        public void PerformChecks(Lifetime checksLifetime, TutorialStepPresenter stepPresenter)
        {
            switch (GoToNextStep)
            {
                case GoToNextStep.Auto:
                    var checker = new MainChecker(checksLifetime, this, stepPresenter.Solution,
                        stepPresenter.PsiFiles, stepPresenter.TextControlManager, stepPresenter.ShellLocks,
                        stepPresenter.EditorManager,
                        stepPresenter.DocumentManager);
                    checker.PerformStepChecks();
                    break;
                case GoToNextStep.Manual:
                    var nextStepPressChecker =
                        new NextStepShortcutChecker(checksLifetime, this, GlobalSettings.NextStepShortcutAction);
                    break;
            }
        }
    }
}