using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SkimThrough
{
    public class KeyProcessorImplementation : KeyProcessor
    {
        private IWpfTextView wpfTextView;

        public KeyProcessorImplementation(IWpfTextView wpfTextView)
        {
            this.wpfTextView = wpfTextView;
        }

        public override void KeyDown(KeyEventArgs args)
        {
            Debug.WriteLine($"key down {args.Key}, {args.SystemKey}, {args.ImeProcessedKey}");
        }

        public override bool IsInterestedInHandledEvents => true;

        public override void PreviewKeyDown(KeyEventArgs args)
        {
            if (args.Key == Key.System)
                return;
            Debug.WriteLine($"preview key down {args.Key}, {args.SystemKey}, {args.ImeProcessedKey}");
            args.Handled = true;
        }
    }

    [Export(typeof(IKeyProcessorProvider))]
    [Name("The processor")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [ContentType("text")]
    [Order(Before = "default")]
    public class KeyProcessorImplementationProvider : IKeyProcessorProvider
    {
        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return new KeyProcessorImplementation(wpfTextView);
        }
    }
}
