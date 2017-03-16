using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Resources.Shell;
using ReSharperTutorials.TutorialUI;
using ReSharperTutorials.Utils;

namespace ReSharperTutorials.Runner
{
    public enum TutorialId
    {
        None,
        Tutorial1,
        Tutorial2,
        Tutorial3,
        Tutorial4,
        Tutorial5
    }

    public enum PathType
    {
        BaseSolutionFolder,
        BaseContentFolder,
        BaseSolutionFile,
        BaseContentFile,
        WorkCopySolutionFolder,
        WorkCopyContentFolder,
        WorkCopySolutionFile,
        WorkCopyContentFile
    }

    [ShellComponent]
    public class GlobalSettings
    {
        private readonly string _commonTutorialPath;
        private readonly string _commonWorkCopyPath;
        public readonly Dictionary<TutorialId, string> AvailableTutorials;
        public readonly Lifetime Lifetime;
        public TutorialWindowManager TutorialWindowManager = null;
        public const string PluginName = "JetBrains.ReSharperTutorials.0.9.3";
        //public const string NextStepShortcutAction = "ReSharper_AltEnter";
        public const string NextStepShortcutAction = "Edit.InsertTab";

        public GlobalSettings(Lifetime lifetime)
        {
            Lifetime = lifetime;

            _commonTutorialPath = SolutionCopyHelper.GetTutorialsPath();
            _commonWorkCopyPath = SolutionCopyHelper.GetWorkingCopyPath();

            if (_commonTutorialPath == null || _commonWorkCopyPath == null)
                throw new DirectoryNotFoundException(
                    "Unable to find the folder with sample solutions. Please reinstall the plugin");

            AvailableTutorials = new Dictionary<TutorialId, string>
            {
                {TutorialId.Tutorial1, GetPath(TutorialId.Tutorial1, PathType.WorkCopySolutionFile)},
                {TutorialId.Tutorial3, GetPath(TutorialId.Tutorial3, PathType.WorkCopySolutionFile)}
            };
        }

        public static GlobalSettings Instance => Shell.Instance.GetComponent<GlobalSettings>();

        public string GetGlobalImgPath()
        {
            return _commonWorkCopyPath + "\\Content\\img";
        }

        public string GetPath(TutorialId tutorialId, PathType pType)
        {
            switch (tutorialId)
            {
                case TutorialId.None:
                    break;

                case TutorialId.Tutorial1:
                    switch (pType)
                    {
                        case PathType.BaseSolutionFolder:
                            return _commonTutorialPath + "\\Tutorial1_EssentialShortcuts";
                        case PathType.BaseContentFolder:
                            return _commonTutorialPath + "\\Content\\Tutorial1";
                        case PathType.BaseSolutionFile:
                            return _commonTutorialPath +
                                   "\\Tutorial1_EssentialShortcuts\\Tutorial1_EssentialShortcuts.sln";
                        case PathType.BaseContentFile:
                            return _commonTutorialPath + "\\Content\\Tutorial1\\Tutorial1Content.xml";
                        case PathType.WorkCopySolutionFolder:
                            return _commonWorkCopyPath + "\\Tutorial1_EssentialShortcuts";
                        case PathType.WorkCopyContentFolder:
                            return _commonWorkCopyPath + "\\Content\\Tutorial1";
                        case PathType.WorkCopySolutionFile:
                            return _commonWorkCopyPath +
                                   "\\Tutorial1_EssentialShortcuts\\Tutorial1_EssentialShortcuts.sln";
                        case PathType.WorkCopyContentFile:
                            return _commonWorkCopyPath + "\\Content\\Tutorial1\\Tutorial1Content.xml";
                        default:
                            throw new ArgumentOutOfRangeException(nameof(pType), pType, null);
                    }

                case TutorialId.Tutorial2:
                    break;
                case TutorialId.Tutorial3:
                    switch (pType)
                    {
                        case PathType.BaseSolutionFolder:
                            return _commonTutorialPath + "\\Tutorial3_WhatsNewReSharper2016.3";
                        case PathType.BaseContentFolder:
                            return _commonTutorialPath + "\\Content\\Tutorial3";
                        case PathType.BaseSolutionFile:
                            return _commonTutorialPath +
                                   "\\Tutorial3_WhatsNewReSharper2016.3\\Tutorial3_WhatsNewReSharper2016.3.sln";
                        case PathType.BaseContentFile:
                            return _commonTutorialPath + "\\Content\\Tutorial3\\Tutorial3Content.xml";
                        case PathType.WorkCopySolutionFolder:
                            return _commonWorkCopyPath + "\\Tutorial3_WhatsNewReSharper2016.3";
                        case PathType.WorkCopyContentFolder:
                            return _commonWorkCopyPath + "\\Content\\Tutorial3";
                        case PathType.WorkCopySolutionFile:
                            return _commonWorkCopyPath +
                                   "\\Tutorial3_WhatsNewReSharper2016.3\\Tutorial3_WhatsNewReSharper2016.3.sln";
                        case PathType.WorkCopyContentFile:
                            return _commonWorkCopyPath + "\\Content\\Tutorial3\\Tutorial3Content.xml";
                        default:
                            throw new ArgumentOutOfRangeException(nameof(pType), pType, null);
                    }
                case TutorialId.Tutorial4:
                    break;
                case TutorialId.Tutorial5:
                    break;
            }
            return null;
        }
    }
}