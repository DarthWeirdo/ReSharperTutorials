﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE;
using JetBrains.Annotations;
using Process = System.Diagnostics.Process;

namespace ReSharperTutorials.Utils
{    

    public static class VsCommunication
    {

        public static bool FindTextInCurrentDocument(string text)
        {
            var vsInstance = GetCurrentVsInstance();
            var selection = vsInstance?.ActiveDocument.Selection as TextSelection;            
            return selection != null && selection.FindText(text);            
        }

        public static void NavigateToTextInCurrentDocument(string text, int occurrence)
        {
            var vsInstance = GetCurrentVsInstance();
            var selection = vsInstance.ActiveDocument.Selection as TextSelection;
            if (occurrence == 0) occurrence = 1;
            for (int i = 1; i <= occurrence; i++)            
                selection?.FindText(text);                        
            selection?.MoveToPoint(selection.BottomPoint);
        }

        public static void DropSelection()
        {
            var vsInstance = GetCurrentVsInstance();
            var selection = vsInstance.ActiveDocument.Selection as TextSelection;
            selection?.MoveToPoint(selection.BottomPoint);
        }

        public static void SetActiveDocument(string doc)
        {
            var vsInstance = GetCurrentVsInstance();
            
            foreach (Document document in vsInstance.Documents)
            {
                if (document.Name != doc) continue;
                document.Activate();
                return;
            }            
        }


        public static bool IsSolutionSaved()
        {
            var vsInstance = GetCurrentVsInstance();
            var solution = vsInstance?.Solution;
            return solution != null && solution.Saved;
        }


        public static void OpenVsSolution(string path)
        {                       
            var vsInstance = GetCurrentVsInstance();
            var solution = vsInstance?.Solution;
            solution?.Open(path);

//            vsInstance.ExecuteCommand("File.OpenProject", path);            
        }

        public static void SaveVsSolution()
        {
            var vsInstance = GetCurrentVsInstance();
            var solution = vsInstance?.Solution;
            if (solution == null) return;            
         
            if (!solution.Saved)
                solution.SaveAs(solution.FullName);

            for (int i = 1; i <= solution.Projects.Count; i++)
            {
                var project = solution.Projects.Item(i);
                if (!project.Saved)
                    project.Save();

                for (int j = 1; j <= project.ProjectItems.Count; j++)
                {
                    var item = project.ProjectItems.Item(j);
                    if (!item.Saved)
                        item.Save();
                }
            }

//            vsInstance.ExecuteCommand("File.SaveAll");
        }

        public static void CloseVsSolution(bool saveFirst)
        {
            var vsInstance = GetCurrentVsInstance();
            var solution = vsInstance?.Solution;

            //            solution?.Close(saveFirst);
            if (solution == null) return;
            if (saveFirst)
                SaveVsSolution();
            
            vsInstance.ExecuteCommand("File.CloseSolution");
        }
        

        private static IEnumerable<DTE> EnumVsInstances()
        {            
            IRunningObjectTable rot;
            int retVal = GetRunningObjectTable(0, out rot);
            if (retVal != 0) yield break;
            IEnumMoniker enumMoniker;
            rot.EnumRunning(out enumMoniker);

            var fetched = IntPtr.Zero;
            var moniker = new IMoniker[1];
            while (enumMoniker.Next(1, moniker, fetched) == 0)
            {
                IBindCtx bindCtx;
                CreateBindCtx(0, out bindCtx);
                string displayName;
                moniker[0].GetDisplayName(bindCtx, null, out displayName);                    
                var isVisualStudio = displayName.StartsWith("!VisualStudio");
                if (!isVisualStudio) continue;
                object obj;
                rot.GetObject(moniker[0], out obj);
                var dte = obj as DTE;
                yield return dte;
            }
        }

        [CanBeNull]
        public static DTE GetCurrentVsInstance()
        {            
            IRunningObjectTable rot;
            GetRunningObjectTable(0, out rot);
            IEnumMoniker enumMoniker;
            rot.EnumRunning(out enumMoniker);
            enumMoniker.Reset();
            var fetched = IntPtr.Zero;
            var moniker = new IMoniker[1];
            while (enumMoniker.Next(1, moniker, fetched) == 0)
            {
                IBindCtx bindCtx;
                CreateBindCtx(0, out bindCtx);
                string displayName;
                moniker[0].GetDisplayName(bindCtx, null, out displayName);
                var isCurrentVsInstance = displayName.StartsWith("!VisualStudio") && displayName.Contains(Process.GetCurrentProcess().Id.ToString());
                if (!isCurrentVsInstance) continue;
                object obj;
                rot.GetObject(moniker[0], out obj);
                return (DTE)obj;
            }
            return null;

        }

        [NotNull]
        public static string GetCurrentSolutionPath()
        {
            var dte = GetCurrentVsInstance();
            var solutionPath = Path.GetFullPath(dte.Solution.FullName);
            return solutionPath;
        }

        [DllImport("ole32.dll")]
        private static extern void CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

    }
}