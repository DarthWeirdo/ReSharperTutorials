using ReSharperTutorials.Utils;

namespace ReSharperTutorials.Runner
{
    public static class TutorialSolutionOpener
    {
        public static void OpenTutorialSolution(SolutionStateTracker solutionStateTracker, int id)
        {
            var globalSettings = GlobalSettings.Instance;            
            VsIntegration.CloseVsSolution();
            solutionStateTracker.NotifyAgreeToRunTutorial();
            SolutionCopyHelper.CleanUpDirectory(globalSettings.GetPath(id, PathType.WorkCopySolutionFolder));
            SolutionCopyHelper.CopySolution(globalSettings.GetPath(id, PathType.BaseSolutionFolder),
                globalSettings.GetPath(id, PathType.WorkCopySolutionFolder));
            VsIntegration.OpenVsSolution(globalSettings.GetPath(id, PathType.WorkCopySolutionFile));
        }
    }
}