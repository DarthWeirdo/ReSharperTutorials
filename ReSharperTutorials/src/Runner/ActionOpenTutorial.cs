using System;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.DataFlow;
using JetBrains.UI.ActionsRevised;
using JetBrains.Util;
using ReSharperTutorials.Utils;

namespace ReSharperTutorials.Runner
{    
    public abstract class ActionOpenTutorial : IExecutableAction
    {

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            OpenTutorial(context, nextExecute);
        }

        protected abstract void OpenTutorial(IDataContext context, DelegateExecute nextExecute);

        public void OpenOrRestart(IDataContext context, TutorialId id)
        {
            var globalOptions = context.GetComponent<GlobalSettings>();
            var titleString = TutorialXmlReader.ReadIntro(globalOptions.GetPath(id, PathType.WorkCopyContentFile));
            var step = TutorialXmlReader.ReadCurrentStep(globalOptions.GetPath(id, PathType.WorkCopyContentFile));
            var firstTime = step == 1;

//            var titleWnd = new TitleWindow(titleString, firstTime);

            var result =
                MessageBox.ShowYesNo(
                    "This will close your current solution and open the tutorial solution. Run the tutorial?",
                    "ReSharper Tutorials");
//            if (titleWnd.ShowDialog() != true) return;
//            if (titleWnd.Restart)
            if (result)
            {
//                var locks = context.GetComponent<IShellLocks>();
//
//                if (VsIntegration.IsAnySolutionOpened())
//                {
//                    var lt = Lifetimes.Define();                    
//                    var stateTracker = context.GetComponent<SolutionStateTracker>();
//                    stateTracker.BeforeSolutionClosed.Advise(lt.Lifetime, () =>
//                    {
//                        SolutionCopyHelper.CopySolution(globalOptions.GetPath(id, PathType.BaseSolutionFolder),
//                            globalOptions.GetPath(id, PathType.WorkCopySolutionFolder));
//                        VsIntegration.OpenVsSolution(globalOptions.GetPath(id, PathType.WorkCopySolutionFile));
//                        lt.Terminate();
//                    });
//
//                    VsIntegration.CloseVsSolution();
//                }
//                else
//                {
//                    locks.ExecuteWithReadLock(() =>
//                    {
                        VsIntegration.CloseVsSolution();
                        SolutionCopyHelper.CopySolution(globalOptions.GetPath(id, PathType.BaseSolutionFolder),
                            globalOptions.GetPath(id, PathType.WorkCopySolutionFolder));
                        VsIntegration.OpenVsSolution(globalOptions.GetPath(id, PathType.WorkCopySolutionFile));
//                    });
                }
                
//                VsIntegration.CloseVsSolution();

//                SolutionCopyHelper.CopySolution(globalOptions.GetPath(id, PathType.BaseSolutionFolder),
//                    globalOptions.GetPath(id, PathType.WorkCopySolutionFolder));

                // now we always run from the beginning 
//                GC.Collect();                
//                TutorialXmlReader.WriteCurrentStep(globalOptions.GetPath(id, PathType.WorkCopyContentFile), "1");

//                VsIntegration.OpenVsSolution(globalOptions.GetPath(id, PathType.WorkCopySolutionFile));
            }
//            else
//                VsIntegration.OpenVsSolution(globalOptions.GetPath(id, PathType.WorkCopySolutionFile));
        }

//    }

    [Action("ActionOpenTutorial1", "Start Tutorial 1 - Essential Shortcuts", Id = 100)]
    public class ActionOpenTutorial1 : ActionOpenTutorial
    {
        protected override void OpenTutorial(IDataContext context, DelegateExecute nextExecute)
        {
            OpenOrRestart(context, TutorialId.Tutorial1);
        }
    }
}