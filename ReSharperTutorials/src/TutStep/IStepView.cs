using System;

namespace ReSharperTutorials.TutStep
{
    public interface IStepView
    {
        string StepText { get; set; }

        int StepCount { set; }

        event EventHandler NextStep;

        void UpdateProgress();
    }
    
}
