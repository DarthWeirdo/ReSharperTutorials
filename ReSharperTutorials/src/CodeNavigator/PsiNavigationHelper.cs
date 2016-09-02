using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.platforms;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Paths;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharperTutorials.Utils;
using PlatformID = JetBrains.Application.platforms.PlatformID;

namespace ReSharperTutorials.CodeNavigator
{
    public static class PsiNavigationHelper
    {        
        public static IEnumerable<ProjectFileTextRange> GetProjectFileTextRangesByDeclaredElement(ISolution solution,
          IDeclaredElement declaredElement)
        {
            IEnumerable<IDeclaration> declarations = declaredElement.GetDeclarations();
            var navigationRanges =
              declarations.Select(declaration => declaration.GetNavigationRange()).Where(range => range.IsValid());
            var psiModule = declarations.Select(decl => decl.GetPsiModule()).FirstOrDefault();
            var documentManager = solution.GetComponent<DocumentManager>();

            return navigationRanges.Select(range =>
            {
                var projectFile = documentManager.TryGetProjectFile(range.Document);

                return projectFile == null
                  ? ProjectFileTextRange.Invalid
                  : new ProjectFileTextRange(projectFile, range.TextRange,
                    psiModule != null ? psiModule.TargetFrameworkId : null);
            }).Where(range => range.IsValid);
        }

        public static IEnumerable<ITypeElement> GetTypeElementsByClrName(ISolution solution, string clrName)
        {            
            IPsiServices psiServices = solution.GetComponent<IPsiServices>();
            psiServices.Files.CommitAllDocuments();

            ISymbolCache symbolCache = psiServices.Symbols;
            ISymbolScope symbolScope = symbolCache.GetSymbolScope(LibrarySymbolScope.FULL, true);

            IEnumerable<ITypeElement> validTypeElements = symbolScope.GetTypeElementsByCLRName(clrName)
              .Where(element => element.IsValid());

            return SkipDefaultProfileIfRuntimeExist(validTypeElements);
        }

        private static IEnumerable<ITypeElement> SkipDefaultProfileIfRuntimeExist(IEnumerable<ITypeElement> validTypeElements)
        {
            return validTypeElements              
              .GroupBy(typeElement => typeElement.GetPlatformId(), DefaultPlatformUtil.IgnoreRuntimeAndDefaultProfilesComparer)
              .Select(TypeFromRuntimeProfilePlatformIfExist);
        }

        private static ITypeElement TypeFromRuntimeProfilePlatformIfExist(IGrouping<PlatformID, ITypeElement> @group)
        {            
            return @group.OrderByDescending(typeElement => typeElement.GetPlatformId(), DefaultPlatformUtil.DefaultPlatformIDComparer).First();
        }

        [CanBeNull]
        public static IProject GetProjectByName(ISolution solution, string projectName)
        {
            var projects = solution.GetTopLevelProjects();
            return projects.FirstOrDefault(project => project.Name == projectName);
        }

        [CanBeNull]
        public static IProject GetOpenedProject(ISolution solution)
        {
            var projects = solution.GetTopLevelProjects();            
            return projects.FirstOrDefault(project => project.IsOpened);            
        }

        [CanBeNull]
        public static ICSharpFile GetCSharpFile(IProject project, string filename)
        {            
                IPsiSourceFile file = project.GetPsiSourceFileInProject(FileSystemPath.Parse(filename));
                return file?.GetPsiFiles<CSharpLanguage>().SafeOfType<ICSharpFile>().SingleOrDefault();        
        }


        [CanBeNull]
        public static ITreeNode GetTypeNodeByFullClrName(ICSharpFile file, string name)
        {
            
            var namespaceName = GetLongNameFromFqn(name);
            var shortName = GetShortNameFromFqn(name);

            var namespaceDecls = file.NamespaceDeclarationsEnumerable;
            var namespaceDecl = (from decl in namespaceDecls
                                 where decl.DeclaredName == namespaceName
                                 select decl).FirstOrDefault();

            if (namespaceDecl == null) return null;                    
            var typeDecls = namespaceDecl.TypeDeclarationsEnumerable;
            var resultList = (from node in typeDecls
                              where node.DeclaredName == shortName
                              select node).ToList();

            return resultList.FirstOrDefault();           
        }



        [CanBeNull]
        public static ITreeNode GetMethodNodeByFullClrName(ICSharpFile file, string typeName, string methodName, int methodOccurrence)
        {
            var typeNode = GetTypeNodeByFullClrName(file, typeName);
            var typeDecl = typeNode as IClassDeclaration;
            if (typeDecl == null) return null;
            var methodDecls = typeDecl.MethodDeclarationsEnumerable;

            var resultList = (from decl in methodDecls
                              where decl.DeclaredName == methodName
                              select decl).AsArray();

            //            var treeNodeList = typeNode.EnumerateTo(typeNode.NextSibling);
            //
            //            var resultList = (from treeNode in treeNodeList
            //                               where treeNode is IMethodDeclaration
            //                               let method = (IMethodDeclaration)treeNode
            //                               where method.DeclaredName == methodName
            //                               select treeNode).AsArray();

            if (methodOccurrence > 0) methodOccurrence = methodOccurrence - 1;
            return resultList.Length > 0 ? resultList[methodOccurrence] : null;
        }


        [CanBeNull]
        public static ITreeNode GetTreeNodeForStep(ICSharpFile file, string typeName, string methodName, int methodOccurrence, string text, int textOccurrence)
        {
            IEnumerable<ITreeNode> treeNodeList;
            string navText;
            var tOccIndex = 0;   
            var mOccIndex = 0;

            if (text != null)
            {
                navText = text;
                if (textOccurrence > 0) tOccIndex = textOccurrence - 1;
            }
            else if (methodName != null)
            {
                navText = methodName;                
            }
            else            
                navText = GetShortNameFromFqn(typeName);
                            
            if (typeName == null && methodName == null)
                treeNodeList = file.EnumerateTo(file.LastChild);
            else if (typeName != null && methodName != null)
            {
                //if (methodOccurrence > 0) mOccIndex = methodOccurrence - 1;
                var node = GetMethodNodeByFullClrName(file, typeName, methodName, methodOccurrence);
                if (node == null) return null;
                treeNodeList = node.EnumerateTo(node.NextSibling);
            }
            else if (typeName != null)
            {
                var node = GetTypeNodeByFullClrName(file, typeName);
                if (node == null) return null;
                treeNodeList = node.EnumerateTo(node.NextSibling);
            }
            else
                return null;                

            var result = (from treeNode in treeNodeList
                         where treeNode.GetText() == navText
                         select treeNode).AsArray();

            return result.Length > 0 ? result[tOccIndex] : null;
            
        }


        public static string GetShortNameFromFqn(string fqn)
        {
            var pos = fqn.LastIndexOf(".", StringComparison.Ordinal) + 1;
            return pos > 0 ? fqn.Substring(pos) : fqn;
        }


        public static string GetLongNameFromFqn(string fqn)
        {
            var pos = fqn.LastIndexOf(".", StringComparison.Ordinal) + 1;
            return pos > 0 ? fqn.Substring(0, pos - 1) : fqn;
        }


        public static IPsiModule GetPsiModuleByName(ISolution solution, IProject project, string name)
        {
            var modules = solution.PsiModules().GetPsiModules(project);
            return modules.FirstOrDefault(psiModule => psiModule.DisplayName == name);
        }


        public static void NavigateToNode(DocumentManager documentManager, IEditorManager editorManager, ITreeNode treeNode, bool activate)
        {            
            if (treeNode == null) return;

            var range = treeNode.GetDocumentRange();
            if (!range.IsValid()) return;                

            var projectFile = documentManager.TryGetProjectFile(range.Document);
            if (projectFile == null) return;

            var textControl = editorManager.OpenProjectFile(projectFile, activate);

            textControl?.Caret.MoveTo(range.TextRange.EndOffset, CaretVisualPlacement.DontScrollIfVisible);

//            textControl.Caret.MoveTo(range.TextRange.StartOffset, CaretVisualPlacement.DontScrollIfVisible);
//            textControl.Selection.SetRange(range.TextRange);
        }
    }
}
