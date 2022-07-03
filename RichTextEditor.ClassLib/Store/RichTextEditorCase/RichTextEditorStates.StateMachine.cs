using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RichTextEditor.ClassLib.Keyboard;

namespace RichTextEditor.ClassLib.Store.RichTextEditorCase;

public partial record RichTextEditorStates
{
    private class StateMachine
    {
        public static RichTextEditorRecord HandleKeyDownEvent(RichTextEditorRecord focusedRichTextEditorRecord, 
            KeyDownEventRecord keyDownEventRecord)
        {
            if (KeyboardKeyFacts.IsWhitespaceKey(keyDownEventRecord))
            {

            }
            else if (KeyboardKeyFacts.IsMovementKey(keyDownEventRecord))
            {
                
            }
            else if (KeyboardKeyFacts.IsMetaKey(keyDownEventRecord)) 
            {
                
            }
            else
            {
                return focusedRichTextEditorRecord with
                {
                    Content = new StringBuilder(
                        focusedRichTextEditorRecord.Content.ToString() + keyDownEventRecord.Key)
                };
            }

            return focusedRichTextEditorRecord;   
        }
    }
}
