using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace ReSharperTutorials.CodeNavigator
{
    public static class TreeNodeExtensions
    {
        [NotNull]
        public static IEnumerable<ITreeNode> ChildrenInSubtrees([NotNull] this ITreeNode node)
        {            
            if (node.FirstChild == null) yield break;
            for (var child = node.FirstChild; child != null; child = child.NextSibling)
            {
                yield return child;

                foreach (var children in child.ChildrenInSubtrees())
                {
                    yield return children;
                }                
            }
        }
    }
}