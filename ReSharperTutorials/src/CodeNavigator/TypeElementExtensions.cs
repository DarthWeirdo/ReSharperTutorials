using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using PlatformID = JetBrains.Application.platforms.PlatformID;

namespace ReSharperTutorials.CodeNavigator
{

    public static class TypeElementExtensions
    {
        [CanBeNull]
        public static PlatformID GetPlatformId([NotNull] this ITypeElement typeElement)
        {
            if (typeElement == null) throw new ArgumentNullException("typeElement");
            IModule containingProjectModule = typeElement.Module.ContainingProjectModule;
            return containingProjectModule == null ? null : containingProjectModule.PlatformID;
        }

        public static string GetFullClrName([NotNull] this ITypeElement typeElement)
        {
            if (typeElement == null) throw new ArgumentNullException("typeElement");
            return typeElement.GetClrName().FullName;
        }

        [NotNull]
        public static IEnumerable<IMethod> GetAllMethods([NotNull] this ITypeElement typeElement)
        {
            if (typeElement == null) throw new ArgumentNullException("typeElement");
            return typeElement.GetMembers().OfType<IMethod>();
        }


        public static void NavigateToFirstMember([NotNull] this ITypeElement typeElement)
        {
            if (typeElement == null) throw new ArgumentNullException("typeElement");
            var member = typeElement.GetMembers().FirstOrDefault();
            member.Navigate(true);
        }

    }
}