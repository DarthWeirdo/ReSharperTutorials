using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;

namespace ReSharperTutorials.Runner
{
    public interface ISolutionStateTracker
    {
        [CanBeNull]
        ISolution Solution { get; }

        ISignal<ISolution> AfterSolutionContainerCreated { get; }
        ISignal<ISolution> AfterSolutionOpened { get; }
        ISignal<ISolution> BeforeSolutionClosed { get; }
        ISignal<ISolution> AfterPsiLoaded { get; }
        ISignal<bool> AgreeToRunTutorial { get; }
    }
}