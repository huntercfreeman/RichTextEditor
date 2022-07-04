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
        public static RichTextEditorRecord HandleWhitespace(RichTextEditorRecord focusedRichTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            if (KeyboardKeyFacts.WhitespaceKeys.ENTER_CODE == keyDownEventRecord.Code)
            {
                return InsertNewLine(focusedRichTextEditorRecord);
            }
            else
            {
                var replacementCurrentToken = focusedRichTextEditorRecord
                    .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

                focusedRichTextEditorRecord = ReplaceCurrentTokenWith(focusedRichTextEditorRecord, replacementCurrentToken);

                var whitespaceTextToken = new WhitespaceTextToken(keyDownEventRecord);

                return InsertNewCurrentTokenAfterCurrentPosition(focusedRichTextEditorRecord,
                    whitespaceTextToken);
            }
        }
    }
}
