using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Editor.Commanding;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkimThrough
{
    [Export(typeof(ICommandHandler))]
    [Name("Skim Through")]
    [ContentType("any")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class SkimCommandHandler : ICommandHandler<SkimDownCommandArgs>
    {
        public SkimCommandHandler()
        {
            Debug.WriteLine("Handler constructor called !");
        }

        public string DisplayName => "Skim Down";

        public bool ExecuteCommand(SkimDownCommandArgs args, CommandExecutionContext executionContext)
        {
            Debug.WriteLine("Omg, it worked !");
            return false;
        }

        public CommandState GetCommandState(SkimDownCommandArgs args)
        {
            return CommandState.Available;
        }
    }
}
