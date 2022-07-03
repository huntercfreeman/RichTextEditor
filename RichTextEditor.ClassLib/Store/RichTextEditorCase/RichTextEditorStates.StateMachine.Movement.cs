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
        public static IRichTextEditor HandleMovement(RichTextEditorRecord focusedRichTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            switch (keyDownEventRecord.Key)
            {
                case KeyboardKeyFacts.MovementKeys.ARROW_LEFT_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_LEFT_KEY:
                    var currentToken = focusedRichTextEditorRecord
                        .GetCurrentTextTokenAs<TextTokenBase>();

                    var replacementCurrentToken = currentToken with
                        {
                            IndexInPlainText = currentToken.IndexInPlainText == 0
                                ? null
                                : currentToken.IndexInPlainText - 1
                        };

                    focusedRichTextEditorRecord = ReplaceCurrentTokenWith(focusedRichTextEditorRecord, replacementCurrentToken);

                    if (replacementCurrentToken.IndexInPlainText is null) 
                        focusedRichTextEditorRecord = SetPreviousTokenAsCurrent(focusedRichTextEditorRecord);
                    break;
                case KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN_KEY:
                    break;
                case KeyboardKeyFacts.MovementKeys.ARROW_UP_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_UP_KEY:
                    break;
                case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_RIGHT_KEY:
                    break;
            }

            return focusedRichTextEditorRecord;
        }
    }
}
