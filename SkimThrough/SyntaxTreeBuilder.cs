using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkimThrough
{
    [Export(typeof(SyntaxTreeBuilder))]
    class SyntaxTreeBuilder
    {

        public static SyntaxTreeNode BuildTree(ITextBuffer textBuffer, IEnumerable<IMappingTagSpan<IStructureTag>> tags)
        {
            SyntaxTreeNode root = new SyntaxTreeNode(null, null);

            var depths = GetDepthList(textBuffer, tags);

            if (depths.Count != tags.Count())
                throw new Exception("whoops");

            SyntaxTreeNode activeNode = root;
            int lastIndex = -1;
            for (int i = 0; i < tags.Count(); i++)
            {
                SyntaxTreeNode parentNode = activeNode;
                int N = lastIndex - depths[i] + 1;
                lastIndex = depths[i];

                for (int j = 0; j < N; j++)
                {
                    parentNode = parentNode.GetParent();
                }

                SyntaxTreeNode node = new SyntaxTreeNode(parentNode, tags.ElementAt(i));
                activeNode = node;
            }

            return root;
        }

        private static List<int> GetDepthList(ITextBuffer textBuffer, IEnumerable<IMappingTagSpan<IStructureTag>> tags)
        {
            List<int> depthList = new List<int>();
            IEnumerable<NormalizedSnapshotSpanCollection> spans = tags.Select(x => x?.Span?.GetSpans(textBuffer)).Where(x => x != null);

            foreach (var tag in tags)
            {
                int depth = tags.Where(x => x.Span.Start.GetPoint(textBuffer, PositionAffinity.Predecessor) < tag.Span.Start.GetPoint(textBuffer, PositionAffinity.Predecessor) && x.Span.GetSpans(textBuffer).OverlapsWith(tag.Span.GetSpans(textBuffer))).Count();
                depthList.Add(depth);
            }

            return depthList;
        }
    }
}
