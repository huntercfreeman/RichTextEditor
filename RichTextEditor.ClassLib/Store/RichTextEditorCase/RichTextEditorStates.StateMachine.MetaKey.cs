using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RichTextEditor.ClassLib.Keyboard;

namespace RichTextEditor.ClassLib.Store.RichTextEditorCase;

public partial record RichTextEditorStates
{
    private partial class StateMachine
    {
        public static IRichTextEditor HandleMetaKey(RichTextEditorRecord focusedRichTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            return focusedRichTextEditorRecord;
        }
    }
}
