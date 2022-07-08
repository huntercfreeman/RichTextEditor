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
            var currentToken = focusedRichTextEditorRecord
                    .GetCurrentTextTokenAs<TextTokenBase>();

            if (currentToken.IndexInPlainText!.Value != currentToken.PlainText.Length - 1)
            {
                return SplitCurrentToken(
                    focusedRichTextEditorRecord, new WhitespaceTextToken(keyDownEventRecord)
                );
            }
            else
            {
                if (KeyboardKeyFacts.WhitespaceKeys.ENTER_CODE == keyDownEventRecord.Code)
                {
                    return InsertNewLine(focusedRichTextEditorRecord);
                }

                return InsertNewCurrentTokenAfterCurrentPosition(focusedRichTextEditorRecord,
                    new WhitespaceTextToken(keyDownEventRecord));
            }
        }
    }
}
