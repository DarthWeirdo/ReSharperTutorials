using System;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Tasks;
using JetBrains.ReSharper.Psi;
using JetBrains.Threading;

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

    [ShellComponent]
    public class SolutionStateTracker : ISolutionStateTracker
    {
        public SolutionStateTracker([NotNull] Lifetime lifetime)
        {
            AfterSolutionContainerCreated = new Signal<ISolution>(lifetime, "SolutionStateTracker.AfterSolutionContainerCreated");
            AfterSolutionOpened = new Signal<ISolution>(lifetime, "SolutionStateTracker.AfterSolutionOpened");
            BeforeSolutionClosed = new Signal<ISolution>(lifetime, "SolutionStateTracker.BeforeSolutionClosed");
            AfterPsiLoaded = new Signal<ISolution>(lifetime, "SolutionStateTracker.AfterPsiLoaded");            
            AgreeToRunTutorial = new Signal<bool>(lifetime, "SolutionStateTracker.AgreeToRunTutorial");            
        }

        public ISolution Solution { get; private set; }
        public ISignal<ISolution> AfterSolutionContainerCreated { get; private set; }
        public ISignal<ISolution> AfterSolutionOpened { get; private set; }
        public ISignal<ISolution> BeforeSolutionClosed { get; private set; }
        public ISignal<ISolution> AfterPsiLoaded { get; private set; }
        public ISignal<bool> AgreeToRunTutorial { get; }

        private void HandleSolutionContainerCreated(ISolution solution)
        {
            AfterSolutionContainerCreated.Fire(solution);
        }

        private void HandleSolutionOpened(ISolution solution)
        {
            Solution = solution;
            AfterSolutionOpened.Fire(solution);            
        }

        private void HandlePsiLoaded(ISolution solution)
        {
            Solution = solution;
            AfterPsiLoaded.Fire(solution);
        }
        

        private void HandleSolutionClosed()
        {
            if (Solution == null)
                // this means that the solution is closed before it has been completely loaded
                return;

            BeforeSolutionClosed.Fire(Solution);
            Solution = null;
        }

        public void NotifyAgreeToRunTutorial()
        {
            AgreeToRunTutorial.Fire(true);
        }
        

        [SolutionComponent]
        private class SolutionStateNotifier
        {            
            public SolutionStateNotifier([NotNull] Lifetime lifetime,
                                         [NotNull] ISolution solution,
                                         [NotNull] IShellLocks shellLocks,
                                         [NotNull] ISolutionLoadTasksScheduler scheduler,
                                         [NotNull] SolutionStateTracker solutionStateTracker,
                                         [NotNull] IPsiServices psiServices)
            {
                if (lifetime == null)
                    throw new ArgumentNullException("lifetime");
                if (solution == null)
                    throw new ArgumentNullException("solution");
                if (shellLocks == null)
                    throw new ArgumentNullException("shellLocks");
                if (scheduler == null)
                    throw new ArgumentNullException("scheduler");
                if (solutionStateTracker == null)
                    throw new ArgumentNullException("solutionStateTracker");                

                scheduler.EnqueueTask(new SolutionLoadTask("SolutionStateTracker",
                    SolutionLoadTaskKinds.SolutionContainer, () => solutionStateTracker.HandleSolutionContainerCreated(solution)));

                scheduler.EnqueueTask(new SolutionLoadTask("SolutionStateTracker",
                    SolutionLoadTaskKinds.Done, () => solutionStateTracker.HandleSolutionOpened(solution)));                

                scheduler.EnqueueTask(new SolutionLoadTask("SolutionStateTracker", 
                    SolutionLoadTaskKinds.AfterDone, () => psiServices.CachesState.IsIdle.WhenTrueOnce(lifetime, () => 
                    solutionStateTracker.HandlePsiLoaded(solution))));                

                lifetime.AddAction(solutionStateTracker.HandleSolutionClosed);                
            }            
        }
    }
}