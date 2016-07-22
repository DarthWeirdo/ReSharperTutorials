using System;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.changes;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.UI.Application;
using JetBrains.Util;

namespace ReSharperTutorials.Checker
{
    internal class StepNavigationChecker
    {
        private readonly Lifetime _lifetime;
        private readonly ISolution _solution;
        private readonly IPsiFiles _psiFiles;
        private readonly TextControlManager _textControlManager;
        private readonly IShellLocks _shellLocks;
        private readonly IEditorManager _editorManager;
        private readonly DocumentManager _documentManager;
        private readonly IUIApplication _environment;
        private int _psiTimestamp;

        /// <summary>
        /// This method must check whether a user navigated to the right node. It corresponds
        /// to a method of the MainChecker class and is run on every caret move.
        /// </summary>
        public Func<bool> Check;

        public ISignal<bool> AfterNavigationDone { get; private set; }

        public StepNavigationChecker(Lifetime lifetime, ISolution solution, IPsiFiles psiFiles,
            TextControlManager textControlManager, IShellLocks shellLocks,
            IEditorManager editorManager, DocumentManager documentManager, IUIApplication environment)
        {
            _lifetime = lifetime;
            _solution = solution;
            _psiFiles = psiFiles;
            _textControlManager = textControlManager;
            _shellLocks = shellLocks;
            _documentManager = documentManager;
            _environment = environment;
            _editorManager = editorManager;                        

            EventHandler caretMoved = (sender, args) =>
            {
                _shellLocks.QueueReadLock(_lifetime, "StepNavigationChecker.CheckOnCaretChange", CheckCode);
            };

            _lifetime.AddBracket(
                () => textControlManager.Legacy.CaretMoved += caretMoved,
                () => textControlManager.Legacy.CaretMoved -= caretMoved);

            AfterNavigationDone = new Signal<bool>(lifetime, "StepNavigationChecker.AfterNavigationDone");
        }

        public bool IsUpToDate()
        {
            // PsiTimestamp is a time stamp for PsiFiles - use it to check the current PSI is up to date
            return _psiTimestamp == Solution.GetPsiServices().Files.PsiTimestamp;
        }

        public ISolution Solution
        {
            get { return _solution; }
        }


        private void CheckCode()
        {
            if (!_psiFiles.AllDocumentsAreCommitted) return;                
            
            if (Check == null) return;
            if (Check())
                AfterNavigationDone.Fire(true);
        }


        [CanBeNull]
        public ITreeNode GetTreeNodeUnderCaret()
        {
//            if (!_solution.GetPsiServices().Files.AllDocumentsAreCommitted) return null;

            var projectFile = GetProjectFile(_textControlManager.LastFocusedTextControl.Value);
            if (projectFile == null)
                return null;

            var textControl = _textControlManager.LastFocusedTextControl.Value;
            if (textControl == null)
                return null;

            var range = textControl.Selection.HasSelection()
                ? textControl.Selection.RandomRange()
                : new TextRange(textControl.Caret.Offset());

            var psiSourceFile = projectFile.ToSourceFile().NotNull("File is null");

            var documentRange = range.CreateDocumentRange(projectFile);            
            var file = psiSourceFile.GetPsiFile(psiSourceFile.PrimaryPsiLanguage, documentRange);

            var element = file?.FindNodeAt(documentRange);
            return element;
        }

        [CanBeNull]
        private IProjectFile GetProjectFile([CanBeNull] ITextControl textControl)
        {
            return textControl == null ? null : _documentManager.TryGetProjectFile(textControl.Document);
        }
    }
}
