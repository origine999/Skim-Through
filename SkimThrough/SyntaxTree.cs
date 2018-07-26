using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SkimThrough
{
    class SyntaxTreeNode
    {
        private List<SyntaxTreeNode> ChildNode;        
        private readonly SyntaxTreeNode Parent;
        private IMappingTagSpan<IStructureTag> TagSpan;

        public SyntaxTreeNode(SyntaxTreeNode parent, IMappingTagSpan<IStructureTag> tagSpan)
        {
            ChildNode = new List<SyntaxTreeNode>();
            TagSpan = tagSpan;
            Parent = parent;
            parent?.AddChild(this);
        }

        public SyntaxTreeNode GetParent()
        {
            return Parent;
        }

        private void AddChild(SyntaxTreeNode child)
        {
            ChildNode.Add(child);
        }

        public SnapshotPoint GetHeaderPosition(ITextBuffer textBuffer)
        {
            if (TagSpan == null)
                return new SnapshotPoint();

            return TagSpan.Span.Start.GetPoint(textBuffer, PositionAffinity.Predecessor).Value;
        }

        public SyntaxTreeNode GetChildAt(int i)
        {
            return ChildNode.ElementAtOrDefault(i);
        }

        public bool IsSpanInBound(SnapshotSpan span)
        {
            if (TagSpan == null)
                return Parent == null;

            return TagSpan.Tag.HeaderSpan.Value.IntersectsWith(span) || TagSpan.Tag.OutliningSpan.Value.IntersectsWith(span);
        }

        public bool IsSpanOverlapChildren(SnapshotSpan span)
        {
            return ChildNode.Where(x => x.TagSpan.Tag.OutliningSpan.Value.OverlapsWith(span)).Count() != 0;
        }

        public SyntaxTreeNode GetFirstChildAt(SnapshotSpan span)
        {
            return ChildNode.Where(x => x.TagSpan.Tag.HeaderSpan.Value.OverlapsWith(span)).FirstOrDefault();
        }
    }
}
