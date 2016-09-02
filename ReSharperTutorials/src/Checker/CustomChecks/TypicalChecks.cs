using System.Text;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReSharperTutorials.CodeNavigator;
using JetBrains.Annotations;
using JetBrains.DocumentManagers;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.IDE;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.TextControl;

namespace ReSharperTutorials.Checker
{
    static class TypicalChecks
    {
        /// <summary>
        /// Converts the entire IFile to string and checks whether it contains $text$        
        /// </summary>     
        public static bool StringExists(ISolution solution, string projectName, string fileName, string text)
        {            
            return ConvertFileToString(solution, projectName, fileName).Contains(text);
        }

        private static string ConvertFileToString(ISolution solution, string projectName, string fileName)
        {
            var result = new StringBuilder();

            solution.Locks.TryExecuteWithReadLock(() =>            
            {
                var project = PsiNavigationHelper.GetProjectByName(solution, projectName);
                var file = PsiNavigationHelper.GetCSharpFile(project, fileName);
                
                if (file == null) return;
                var treeNodeList = file.EnumerateTo(file.LastChild);

                foreach (var node in treeNodeList)
                {
                    result.AppendSlice(node.GetText());
                }
            });

            return result.ToString();
        }

        /// <summary>
        /// Finds type declaration in scope specified in the current step
        /// </summary>
        /// <returns>Returns true if $typeName$ is found</returns>
        public static bool TypeDeclarationExists(ISolution solution, string projectName, string fileName, string typeName)
        {
            ITreeNode node = null;

            solution.Locks.TryExecuteWithReadLock(() =>
            {
                var project = PsiNavigationHelper.GetProjectByName(solution, projectName);                
                var file = PsiNavigationHelper.GetCSharpFile(project, fileName);
                node = PsiNavigationHelper.GetTypeNodeByFullClrName(file, typeName);
            });

            return node != null;
        }

        [CanBeNull]
        public static ITreeNode GetTypeNode(ISolution solution, string projectName, string fileName, string typeName)
        {
            ITreeNode node = null;

            solution.Locks.TryExecuteWithReadLock(() =>
            {
                var project = PsiNavigationHelper.GetProjectByName(solution, projectName);
                var file = PsiNavigationHelper.GetCSharpFile(project, fileName);
                node = PsiNavigationHelper.GetTypeNodeByFullClrName(file, typeName);
            });

            return node;
        }

        /// <summary>
        /// Finds method declaration in scope specified in the current step
        /// </summary>
        /// <returns>Returns true if $typeName$ is found</returns>
        public static bool MethodDeclarationExists(ISolution solution, string projectName, string fileName, string typeName, string methodName, int methodOccurrence)
        {
            ITreeNode node = null;

            solution.Locks.TryExecuteWithReadLock(() =>
            {
                var project = PsiNavigationHelper.GetProjectByName(solution, projectName);
                var file = PsiNavigationHelper.GetCSharpFile(project, fileName);
                node = PsiNavigationHelper.GetMethodNodeByFullClrName(file, typeName, methodName, methodOccurrence);
            });

            return node != null;
        }


        /// <summary>
        /// Get ITreeNode by method's FQN
        /// </summary>
        /// <param name="solution">Solution</param>
        /// <param name="projectName">Project name</param>
        /// <param name="fileName">File name</param>
        /// <param name="typeName">Type short name</param>
        /// <param name="methodName">Method short name</param>
        /// <param name="methodOccurrence">Method occurence (for overloads)</param>
        /// <returns></returns>
        [CanBeNull]
        public static ITreeNode GetMethodNode(ISolution solution, string projectName, string fileName, string typeName, string methodName, int methodOccurrence)
        {
            ITreeNode node = null;

            solution.Locks.TryExecuteWithReadLock(() =>
            {
                var project = PsiNavigationHelper.GetProjectByName(solution, projectName);
                var file = PsiNavigationHelper.GetCSharpFile(project, fileName);
                node = PsiNavigationHelper.GetMethodNodeByFullClrName(file, typeName, methodName, methodOccurrence);
            });

            return node;
        }

        /// <summary>
        /// Finds text of a tree node in scope specified in the current step
        /// </summary>        
        /// <returns>Returns true if specific $occurrence$ of $text$ is found</returns>        
        public static bool TreeNodeWithTextExists(ISolution solution, string projectName, string fileName, string typeName, string methodName, int methodOccurrence, string text, int occurrence)
        {
            ITreeNode node = null;

            solution.Locks.TryExecuteWithReadLock(() =>
            {
                var project = PsiNavigationHelper.GetProjectByName(solution, projectName);
                var file = PsiNavigationHelper.GetCSharpFile(project, fileName);
                node = PsiNavigationHelper.GetTreeNodeForStep(file, typeName, methodName, methodOccurrence, text, occurrence);
            });

            return node != null;
        }


        [CanBeNull]
        public static ITreeNode GetTreeNodeUnderCaret(DocumentManager documentManager, ITextControlManager textControlManager)
        {            
            var textControl = textControlManager.LastFocusedTextControl.Value;
            if (textControl == null)
                return null;

            var projectFile = documentManager.TryGetProjectFile(textControl.Document);
            if (projectFile == null)
                return null;            

//            var range = textControl.Selection.HasSelection()
//                ? textControl.Selection.OneDocRangeWithCaret()
//                : new TextRange(textControl.Caret.Offset());

            var range = new TextRange(textControl.Caret.Offset());

            var psiSourceFile = projectFile.ToSourceFile().NotNull("File is null");

            var documentRange = range.CreateDocumentRange(projectFile);
            var file = psiSourceFile.GetPsiFile(psiSourceFile.PrimaryPsiLanguage, documentRange);

            var element = file?.FindNodeAt(documentRange);
            return element;
        }        
    }
}
