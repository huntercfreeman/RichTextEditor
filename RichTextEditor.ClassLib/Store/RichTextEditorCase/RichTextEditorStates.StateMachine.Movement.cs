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
                {
                    var currentToken = focusedRichTextEditorRecord
                        .GetCurrentTextTokenAs<TextTokenBase>();

                    if (currentToken.IndexInPlainText == 0)
                    {
                        return SetPreviousTokenAsCurrent(focusedRichTextEditorRecord);
                    }
                    else
                    {
                        var replacementCurrentToken = currentToken with
                        {
                            IndexInPlainText = currentToken.IndexInPlainText - 1
                        };

                        focusedRichTextEditorRecord = ReplaceCurrentTokenWith(focusedRichTextEditorRecord, replacementCurrentToken);
                    }
                        
                    break;
                }
                case KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN_KEY:
                {
                    break;
                }
                case KeyboardKeyFacts.MovementKeys.ARROW_UP_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_UP_KEY:
                {
                    break;
                }
                case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_RIGHT_KEY:
                {
                    var currentToken = focusedRichTextEditorRecord
                        .GetCurrentTextTokenAs<TextTokenBase>();

                    if (currentToken.IndexInPlainText == currentToken.PlainText.Length - 1)
                    {
                        return SetNextTokenAsCurrent(focusedRichTextEditorRecord);
                    }
                    else
                    {
                        var replacementCurrentToken = currentToken with
                        {
                            IndexInPlainText = currentToken.IndexInPlainText + 1
                        };

                        focusedRichTextEditorRecord = ReplaceCurrentTokenWith(focusedRichTextEditorRecord, replacementCurrentToken);
                    }
                    
                    break;
                }
            }

            return focusedRichTextEditorRecord;
        }
    }
}
