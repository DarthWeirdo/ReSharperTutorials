using System;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.TextControl;
using JetBrains.UI.Application;

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

        public ISignal<bool> OnCheckPass { get; private set; }

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

            EventHandler caretMoved =
                (sender, args) =>
                {
                    _shellLocks.QueueReadLock(_lifetime, "StepNavigationChecker.CheckOnCaretChange", CheckCode);
                };

            _lifetime.AddBracket(
                () => textControlManager.Legacy.CaretMoved += caretMoved,
                () => textControlManager.Legacy.CaretMoved -= caretMoved);

            OnCheckPass = new Signal<bool>(lifetime, "StepNavigationChecker.AfterNavigationDone");
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
                OnCheckPass.Fire(true);
        }
    }
}