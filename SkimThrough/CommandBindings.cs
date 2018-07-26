using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Editor.Commanding;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkimThrough
{
    internal sealed class CommandBindings
    {
        public const string SkimCommandSet = "d0951c77-c2bf-4ed4-9512-981db162a8e0";
        public const uint SkimDownCommandId = 0x0100;
        public const uint SkimUpCommandId = 4129;

        [Export]
        [CommandBinding(SkimCommandSet, SkimDownCommandId, typeof(SkimDownCommandArgs))]
        internal CommandBindingDefinition skimCommandBindings;
    }
}
