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
        public static RichTextEditorRecord HandleMovement(RichTextEditorRecord focusedRichTextEditorRecord,
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
                    if (focusedRichTextEditorRecord.CurrentRowIndex >= 
                        focusedRichTextEditorRecord.Array.Length - 1)
                    {
                        return focusedRichTextEditorRecord;
                    }

                    var inclusiveStartingColumnIndexOfCurrentToken = 
                        CalculateCurrentTokenColumnIndexRespectiveToRow(focusedRichTextEditorRecord);

                    var currentColumnIndexWithIndexInPlainTextAccountedFor = inclusiveStartingColumnIndexOfCurrentToken +
                        focusedRichTextEditorRecord.CurrentTextToken.IndexInPlainText!.Value;

                    var rowBelowKey = focusedRichTextEditorRecord.Array[focusedRichTextEditorRecord.CurrentRowIndex + 1];

                    var rowBelow = focusedRichTextEditorRecord.Map[rowBelowKey];

                    var tokenInRowBelowTuple = CalculateTokenAtColumnIndexRespectiveToRow(
                        focusedRichTextEditorRecord,
                        rowBelow 
                            as RichTextEditorRow 
                            ?? throw new ApplicationException($"Expected type {nameof(RichTextEditorRow)}"),
                        currentColumnIndexWithIndexInPlainTextAccountedFor);

                    while (focusedRichTextEditorRecord.CurrentTextToken.Key !=
                           tokenInRowBelowTuple.token.Key)
                    {
                        focusedRichTextEditorRecord = HandleMovement(focusedRichTextEditorRecord, new KeyDownEventRecord(
                            KeyboardKeyFacts.MovementKeys.ARROW_RIGHT_KEY,
                            KeyboardKeyFacts.MovementKeys.ARROW_RIGHT_KEY,
                            false,
                            keyDownEventRecord.ShiftWasPressed,
                            false
                        ));
                    }

                    if (currentColumnIndexWithIndexInPlainTextAccountedFor <
                        tokenInRowBelowTuple.exclusiveEndingColumnIndex)
                    {
                        var replacementCurrentToken = focusedRichTextEditorRecord
                            .GetCurrentTextTokenAs<TextTokenBase>() with
                            {
                                IndexInPlainText = currentColumnIndexWithIndexInPlainTextAccountedFor -
                                    tokenInRowBelowTuple.inclusiveStartingColumnIndex
                            };

                        focusedRichTextEditorRecord = ReplaceCurrentTokenWith(focusedRichTextEditorRecord, replacementCurrentToken);
                    }
                    else
                    {
                        var replacementCurrentToken = focusedRichTextEditorRecord
                            .GetCurrentTextTokenAs<TextTokenBase>() with
                            {
                                IndexInPlainText = focusedRichTextEditorRecord.CurrentTextToken.PlainText.Length - 1
                            };
                        focusedRichTextEditorRecord = ReplaceCurrentTokenWith(focusedRichTextEditorRecord, replacementCurrentToken);
                    }
                    
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
