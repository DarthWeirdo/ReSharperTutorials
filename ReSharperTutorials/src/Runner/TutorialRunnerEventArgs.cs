using System.Windows;

namespace ReSharperTutorials.Runner
{
    internal class TutorialRunnerEventArgs : RoutedEventArgs
    {
        public readonly bool SolutionSaved;

        public TutorialRunnerEventArgs(bool solutionSaved)
        {
            SolutionSaved = solutionSaved;
        }
    }
}