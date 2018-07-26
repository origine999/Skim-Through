using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkimThrough
{
    [Export(typeof(SkimNavigationService))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class SkimNavigationService
    {
        [Import(typeof(IViewTagAggregatorFactoryService))]
        internal IViewTagAggregatorFactoryService TagAggregatorService { get; set; }
        
        private ITagAggregator<IStructureTag> TagAggregator;
        private SyntaxTreeNode Root;
        private SyntaxTreeNode ActiveNode;
        private ITextView TextView;
        private ITextBuffer TextBuffer;

        public SkimNavigationService(ITextView textView, ITextBuffer textBuffer, ITagAggregator<IStructureTag> tagAggregator)
        {
            TextView = textView;
            TextBuffer = textBuffer;
            TagAggregator = tagAggregator;//TagAggregatorService.CreateTagAggregator<IStructureTag>(TextView);
            var tags = TagAggregator.GetTags(new SnapshotSpan(textBuffer.CurrentSnapshot, new Span(0, textBuffer.CurrentSnapshot.Length)));
            
            ActiveNode = Root = SyntaxTreeBuilder.BuildTree(textBuffer, tags);
        }

        public void Up()
        {
            int newLineIndex = (TextView.Caret.Position.BufferPosition.GetContainingLine().LineNumber + 1);
            if (newLineIndex <= 0)
                    return;
            var line = TextBuffer.CurrentSnapshot.GetLineFromLineNumber(newLineIndex);
            TextView.Caret.MoveTo(line.Start);

            SnapshotSpan snapshotSpan = new SnapshotSpan(TextView.Caret.Position.BufferPosition.Snapshot, new Span(TextView.Caret.Position.BufferPosition.GetContainingLine().Start.Position, TextView.Caret.Position.BufferPosition.GetContainingLine().LengthIncludingLineBreak));
            while (ActiveNode.IsSpanInBound(snapshotSpan))
            {
                newLineIndex = TextView.Caret.Position.BufferPosition.GetContainingLine().LineNumber - 1;

                if (newLineIndex < 0)
                    return;

                line = TextBuffer.CurrentSnapshot.GetLineFromLineNumber(newLineIndex);
                var temp = TextView.Caret.MoveTo(line.Start);
                snapshotSpan = new SnapshotSpan(TextView.Caret.Position.BufferPosition.Snapshot, new Span(TextView.Caret.Position.BufferPosition.GetContainingLine().Start.Position, TextView.Caret.Position.BufferPosition.GetContainingLine().LengthIncludingLineBreak));

                if (!ActiveNode.IsSpanOverlapChildren(snapshotSpan))
                    return;

            }
        }

        public void Down()
        {        
            int newLineIndex = (TextView.Caret.Position.BufferPosition.GetContainingLine().LineNumber - 1);
            if (newLineIndex >= TextBuffer.CurrentSnapshot.LineCount)
                    return;
            var line = TextBuffer.CurrentSnapshot.GetLineFromLineNumber(newLineIndex);
            TextView.Caret.MoveTo(line.Start);

            SnapshotSpan snapshotSpan = new SnapshotSpan(TextView.Caret.Position.BufferPosition.Snapshot, new Span(TextView.Caret.Position.BufferPosition.GetContainingLine().Start.Position, TextView.Caret.Position.BufferPosition.GetContainingLine().LengthIncludingLineBreak));
            while (ActiveNode.IsSpanInBound(snapshotSpan))
            {
                newLineIndex = TextView.Caret.Position.BufferPosition.GetContainingLine().LineNumber + 1;

                if (newLineIndex >= TextBuffer.CurrentSnapshot.LineCount)
                    return;

                line = TextBuffer.CurrentSnapshot.GetLineFromLineNumber(newLineIndex);
                var temp = TextView.Caret.MoveTo(line.Start);
                snapshotSpan = new SnapshotSpan(TextView.Caret.Position.BufferPosition.Snapshot, new Span(TextView.Caret.Position.BufferPosition.GetContainingLine().Start.Position, TextView.Caret.Position.BufferPosition.GetContainingLine().LengthIncludingLineBreak));

                if (!ActiveNode.IsSpanOverlapChildren(snapshotSpan))
                    return;

            }
        }

        public void AltLeft()
        {
            SyntaxTreeNode node = ActiveNode.GetParent();

            if (node == null)
                return;

            ActiveNode = node;
            TextView.Caret.MoveTo(ActiveNode.GetHeaderPosition(TextBuffer));
        }

        public void AltRight()
        {
            SyntaxTreeNode node = ActiveNode.GetFirstChildAt(new SnapshotSpan(TextView.Caret.Position.BufferPosition.Snapshot, new Span(TextView.Caret.Position.BufferPosition.Position-1, 1)));

            if (node == null)
                return;

            ActiveNode = node;
            TextView.Caret.MoveTo(ActiveNode.GetHeaderPosition(TextBuffer));
        }
    }
}
