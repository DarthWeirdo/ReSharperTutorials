using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.platforms;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Paths;
using JetBrains.ReSharper.Psi.Tree;
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
                return file.GetPsiFiles<CSharpLanguage>().SafeOfType<ICSharpFile>().SingleOrDefault();        
        }

        [CanBeNull]
        public static IDeclaration GetDeclaration(ITreeNode node)
        {
            while (null != node)
            {
                var declaration = node as IDeclaration;
                if (null != declaration)
                    return declaration;
                node = node.Parent;
            }
            return null;
        }

        [CanBeNull]
        public static IDeclaredElement GetDeclaredElement(ITreeNode node)
        {
            var declaration = GetDeclaration(node);            
            return declaration?.DeclaredElement;
        }

        #region High-level R# navigation (not used)

        public static void NavigateToMethod(ICSharpFile file, string typeName, string methodName, IShellLocks shellLocks, Lifetime lifetime)
        {
            var treeNodeList = file.EnumerateTo(file.LastChild);            

            var methods = (from treeNode in treeNodeList
                let element = GetDeclaredElement(treeNode)
                let typeElement = element as ITypeElement
                where typeElement != null
                where typeElement.GetFullClrName() == typeName
                select typeElement.GetAllMethods()).FirstOrDefault();

            if (methods == null) return;

            var targetMethod = methods.FirstOrDefault(method => method.ShortName == methodName);            

            shellLocks.ReentrancyGuard.ExecuteOrQueue("Navigate", () =>
            {
                targetMethod.Navigate(true);                
            });            
        }        

        public static void NavigateToType(ICSharpFile file, string typeName, IShellLocks shellLocks, Lifetime lifetime)
        {
            var treeNodeList = file.EnumerateTo(file.LastChild);

            var targetType = (from treeNode in treeNodeList
                let element = GetDeclaredElement(treeNode)
                let typeElement = element as ITypeElement
                where typeElement != null                           
                where typeElement.GetFullClrName() == typeName
                select typeElement).FirstOrDefault();

           shellLocks.ReentrancyGuard.ExecuteOrQueue("Navigate", () =>
           {
               targetType.Navigate(true);
           });
        }

        public static void NavigateToText(ICSharpFile file, string typeName, string methodName, string text, int textOcc, IShellLocks shellLocks, Lifetime lifetime)
        {
            var treeNodeList = file.EnumerateTo(file.LastChild);

            var methods = (from treeNode in treeNodeList
                           let element = GetDeclaredElement(treeNode)
                           let typeElement = element as ITypeElement
                           where typeElement != null
                           where typeElement.GetFullClrName() == typeName
                           select typeElement.GetAllMethods()).FirstOrDefault();

            if (methods == null) return;

            var targetMethod = methods.FirstOrDefault(method => method.ShortName == methodName);

            shellLocks.ReentrancyGuard.ExecuteOrQueue("Navigate", () => { targetMethod.Navigate(true);});
            shellLocks.ReentrancyGuard.ExecuteOrQueue("FindText", () =>
            {
                VsIntegration.NavigateToTextInCurrentDocument(text, textOcc);
            });            
        }
        #endregion

        [CanBeNull]
        public static ITreeNode GetTypeNodeByFullClrName(ICSharpFile file, string name)
        {
            var treeNodeList = file.EnumerateTo(file.LastChild);

            var resultList = (from node in treeNodeList
                              let element = GetDeclaredElement(node)
                              let typeElement = element as ITypeElement
                              where typeElement != null && typeElement.GetFullClrName() == name
                              select node).ToList();

            return resultList.FirstOrDefault();
        }



        [CanBeNull]
        public static ITreeNode GetMethodNodeByFullClrName(ICSharpFile file, string typeName, string methodName)
        {
            var typeNode = GetTypeNodeByFullClrName(file, typeName);
            if (typeNode == null) return null;

            var treeNodeList = typeNode.EnumerateTo(typeNode.NextSibling);

            return (from treeNode in treeNodeList
                    where treeNode is IMethodDeclaration
                    let method = (IMethodDeclaration)treeNode
                    where method.DeclaredName == methodName
                    select treeNode).FirstOrDefault();
        }


        [CanBeNull]
        public static ITreeNode GetTreeNodeForStep(ICSharpFile file, string typeName, string methodName, string text, int textOccurrence)
        {
            IEnumerable<ITreeNode> treeNodeList;
            string navText;
            var occIndex = 0;   // TODO: provide occurence # for methods as well (for overloads)

            if (text != null)
            {
                navText = text;
                if (textOccurrence > 0) occIndex = textOccurrence - 1;
            }
            else if (methodName != null)
            {
                navText = methodName;
                occIndex = 0;
            }
            else            
                navText = GetShortNameFromFqn(typeName);
                            
            if (typeName == null && methodName == null)
                treeNodeList = file.EnumerateTo(file.LastChild);
            else if (typeName != null && methodName != null)
            {
                var node = GetMethodNodeByFullClrName(file, typeName, methodName);
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

            var result = from treeNode in treeNodeList
                         where treeNode.GetText() == navText
                         select treeNode;

            return result.AsArray().Length > 0 ? result.AsArray()[occIndex] : null;
            
        }


        private static string GetShortNameFromFqn(string fqn)
        {
            var pos = fqn.LastIndexOf(".", StringComparison.Ordinal) + 1;
            return pos > 0 ? fqn.Substring(pos) : fqn;
        }


    }
}
