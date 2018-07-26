using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;

namespace SkimThrough
{
    /// <summary>
    /// TextAdornment1 places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class TextAdornment1
    {
        /// <summary>
        /// The layer of the adornment.
        /// </summary>
        private readonly IAdornmentLayer layer;

        /// <summary>
        /// Text view where the adornment is created.
        /// </summary>
        private readonly IWpfTextView view;

        /// <summary>
        /// Adornment brush.
        /// </summary>
        private readonly Brush brush;

        /// <summary>
        /// Adornment pen.
        /// </summary>
        private readonly Pen pen;
        
        private static bool go = false;
        //private readonly ITextStructureNavigatorSelectorService navigatorService;
        private readonly IViewTagAggregatorFactoryService tagAggregatorService;
        private SkimNavigationService skimNavigationService ;
        

        /// <summary>
        /// Initializes a new instance of the <see cref="TextAdornment1"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public TextAdornment1(IWpfTextView view,/* ITextStructureNavigatorSelectorService navigatorService,*/ IViewTagAggregatorFactoryService tagAggregatorService)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            //this.navigatorService = navigatorService;

            this.layer = view.GetAdornmentLayer("TextAdornment1");

            this.view = view;
            //this.view.LayoutChanged += this.OnLayoutChanged;
            this.view.Caret.PositionChanged += this.OnPositionChanged;

            this.tagAggregatorService = tagAggregatorService;

            // Create the pen and brush to color the box behind the a's
            this.brush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
            this.brush.Freeze();

            //skimNavigationService = new SkimNavigationService(view, view.TextDataModel.DocumentBuffer, tagAggregatorService.CreateTagAggregator<IStructureTag>(view));

            var penBrush = new SolidColorBrush(Colors.Red);
            penBrush.Freeze();
            this.pen = new Pen(penBrush, 0.5);
            this.pen.Freeze();
        }

        /// <summary>
        /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
        /// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
        /// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            foreach (ITextViewLine line in e.NewOrReformattedLines)
            {
                //this.CreateVisuals(line);
            }
        }

        internal void OnPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            if (go)
            {
                return;
            }
            go = true;

            
            var tagaggregator = tagAggregatorService.CreateTagAggregator<IStructureTag>(e.TextView);
            var tags = tagaggregator.GetTags(new SnapshotSpan(e.TextView.TextDataModel.DocumentBuffer.CurrentSnapshot, new Span(0, e.TextView.TextDataModel.DocumentBuffer.CurrentSnapshot.Length)));

            if (skimNavigationService == null)
                skimNavigationService = new SkimNavigationService(e.TextView, e.TextView.TextDataModel.DocumentBuffer, tagaggregator);

            int move = e.NewPosition.BufferPosition.Position - e.OldPosition.BufferPosition.Position;

            var caret = this.view.Caret;
            var test = new SnapshotSpan(e.NewPosition.BufferPosition.Snapshot, new Span(e.NewPosition.BufferPosition.Position, 1));
            //test = navigatorService.GetTextStructureNavigator(e.TextView.TextBuffer).GetExtentOfWord(e.NewPosition.BufferPosition).Span;
            //var span = new CustomNavigator(e.TextView.TextBuffer).GetSpanOfFirstChild(test);

            //SnapshotSpan span;
            //if (foward)
            //{
            //    span = navigatorService.GetTextStructureNavigator(e.TextView.TextBuffer).GetSpanOfNextSibling(test);
            //    if (span.Start < e.NewPosition.BufferPosition.Position)
            //        span = navigatorService.GetTextStructureNavigator(e.TextView.TextBuffer).GetSpanOfFirstChild(test);

            //}
            //else
            //{
            //    span = navigatorService.GetTextStructureNavigator(e.TextView.TextBuffer).GetSpanOfPreviousSibling(test);
            //}

            
            //var distincType = tags.Select(x => x.Tag.GetType().ToString()).Distinct();
            
            //var tree = SyntaxTreeBuilder.BuildTree(e.TextView.TextDataModel.DocumentBuffer, tags);
            //var node = tree?.GetChildAt(0)?.GetChildAt(0)?.GetFirstChildAt(test);
            //bool included = tree?.GetChildAt(0)?.GetChildAt(0)?.IsSpanInBound(test) ?? false;

            

            if (move == 1)
            {
                skimNavigationService.AltRight();
            }
            else if (move == -1)
            {
                skimNavigationService.AltLeft();

            }
            else if (move < -1)
            {
                skimNavigationService.Up();

            }
            else if (move > 1)
            {
                skimNavigationService.Down();

            }

            go = false;
        }

        /// <summary>
        /// Adds the scarlet box behind the 'a' characters within the given line
        /// </summary>
        /// <param name="line">Line to add the adornments</param>
        private void CreateVisuals(ITextSnapshotLine line)
        {
            IWpfTextViewLineCollection textViewLines = this.view.TextViewLines;

            // Loop through each character, and place a box around any 'a'
            for (int charIndex = line.Start; charIndex < line.End; charIndex++)
            {
                if (this.view.TextSnapshot[charIndex] == 'a' || true)
                {
                    SnapshotSpan span = new SnapshotSpan(this.view.TextSnapshot, Span.FromBounds(charIndex, charIndex + 1));
                    Geometry geometry = textViewLines.GetMarkerGeometry(span);
                    if (geometry != null)
                    {
                        var drawing = new GeometryDrawing(this.brush, this.pen, geometry);
                        drawing.Freeze();

                        var drawingImage = new DrawingImage(drawing);
                        drawingImage.Freeze();

                        var image = new Image
                        {
                            Source = drawingImage,
                        };

                        // Align the image with the top of the bounds of the text geometry
                        Canvas.SetLeft(image, geometry.Bounds.Left);
                        Canvas.SetTop(image, geometry.Bounds.Top);

                        this.layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
                    }
                }
            }
        }
    }
}
