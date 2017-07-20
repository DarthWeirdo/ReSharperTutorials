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
    public enum PathType
    {
        BaseSolutionFolder,
        BaseContentFolder,
        BaseSolutionFile,
        BaseContentFile,
        WorkCopySolutionFolder,
        WorkCopyContentFolder,
        WorkCopySolutionFile,
        WorkCopyContentFile,
        WorkCopyImagesFolder,
    }

    [ShellComponent]
    public class GlobalSettings
    {
        private readonly string _commonTutorialPath;
        private readonly string _commonWorkCopyPath;
        public readonly Dictionary<int, string> AvailableTutorials;
        public readonly Lifetime Lifetime;
        public TutorialWindowManager TutorialWindowManager = null;

        public const string PluginName = "JetBrains.ReSharperTutorials.0.9.13";
        
        public const string NextStepShortcutAction = "Edit.InsertTab";

        public GlobalSettings(Lifetime lifetime)
        {
            Lifetime = lifetime;

            _commonTutorialPath = SolutionCopyHelper.GetTutorialsPath();
            _commonWorkCopyPath = SolutionCopyHelper.GetWorkingCopyPath();

            if (_commonTutorialPath == null || _commonWorkCopyPath == null)
                throw new DirectoryNotFoundException(
                    "Unable to find the folder with sample solutions. Please reinstall the plugin");

            AvailableTutorials = new Dictionary<int, string>
            {
                {1, GetPath(1, PathType.WorkCopySolutionFile)},
                {3, GetPath(3, PathType.WorkCopySolutionFile)},
                {4, GetPath(4, PathType.WorkCopySolutionFile)},
            };
        }

        public static GlobalSettings Instance => Shell.Instance.GetComponent<GlobalSettings>();

        public string GetGlobalImgPath()
        {
            return _commonWorkCopyPath + "\\Content\\img";
        }

        public string GetPath(int tutorialId, PathType pType)
        {
            switch (tutorialId)
            {
                case 0:
                    break;

                case 1:
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

                case 2:
                    break;
                case 3:
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
                case 4:
                    switch (pType)
                    {
                        case PathType.BaseSolutionFolder:
                            return _commonTutorialPath + "\\Tutorial4_WhatsNewReSharper2017.1";
                        case PathType.BaseContentFolder:
                            return _commonTutorialPath + "\\Content\\Tutorial4";
                        case PathType.BaseSolutionFile:
                            return _commonTutorialPath +
                                   "\\Tutorial4_WhatsNewReSharper2017.1\\Tutorial4_WhatsNewReSharper2017.1.sln";
                        case PathType.BaseContentFile:
                            return _commonTutorialPath + "\\Content\\Tutorial4\\Tutorial4Content.xml";
                        case PathType.WorkCopySolutionFolder:
                            return _commonWorkCopyPath + "\\Tutorial4_WhatsNewReSharper2017.1";
                        case PathType.WorkCopyContentFolder:
                            return _commonWorkCopyPath + "\\Content\\Tutorial4";
                        case PathType.WorkCopySolutionFile:
                            return _commonWorkCopyPath +
                                   "\\Tutorial4_WhatsNewReSharper2017.1\\Tutorial4_WhatsNewReSharper2017.1.sln";
                        case PathType.WorkCopyContentFile:
                            return _commonWorkCopyPath + "\\Content\\Tutorial4\\Tutorial4Content.xml";
                        default:
                            throw new ArgumentOutOfRangeException(nameof(pType), pType, null);
                    }
                case 5:
                    break;
            }
            return null;
        }
    }
}