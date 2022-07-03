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
        public static IRichTextEditor HandleKeyDownEvent(RichTextEditorRecord focusedRichTextEditorRecord, 
            KeyDownEventRecord keyDownEventRecord)
        {
            if (KeyboardKeyFacts.IsWhitespaceKey(keyDownEventRecord))
            {
                return HandleWhitespace(focusedRichTextEditorRecord, keyDownEventRecord);
            }
            else if (KeyboardKeyFacts.IsMovementKey(keyDownEventRecord))
            {
                return HandleMovement(focusedRichTextEditorRecord, keyDownEventRecord);
            }
            else if (KeyboardKeyFacts.IsMetaKey(keyDownEventRecord)) 
            {
                return HandleMetaKey(focusedRichTextEditorRecord, keyDownEventRecord);
            }
            else
            {
                return HandleDefault(focusedRichTextEditorRecord, keyDownEventRecord);
            }
        }

        private static RichTextEditorRecord InsertNewCurrentTokenAfterCurrentPosition(RichTextEditorRecord focusedRichTextEditorRecord,
            ITextToken textToken)
        {
            var nextMap = new Dictionary<TextTokenKey, ITextToken>(
                focusedRichTextEditorRecord.CurrentRichTextEditorRow.Map
            );

            nextMap[textToken.Key] = textToken;
            
            var nextList = new List<TextTokenKey>(
                focusedRichTextEditorRecord.CurrentRichTextEditorRow.Array
            );

            nextList.Add(textToken.Key);
            
            var nextRowInstance = focusedRichTextEditorRecord.GetCurrentRichTextEditorRowAs<RichTextEditorRow>() with
            {
                Map = nextMap.ToImmutableDictionary(),
                Array = nextList.ToImmutableArray()
            };
            
            var nextRowMap = new Dictionary<RichTextEditorRowKey, IRichTextEditorRow>(
                focusedRichTextEditorRecord.Map
            );

            nextRowMap[nextRowInstance.Key] = nextRowInstance;

            return focusedRichTextEditorRecord with
            {
                Map = nextRowMap.ToImmutableDictionary(),
                CurrentTokenIndex = focusedRichTextEditorRecord.CurrentTokenIndex + 1
            };
        }
        
        private static RichTextEditorRecord ReplaceCurrentTokenWith(RichTextEditorRecord focusedRichTextEditorRecord,
            ITextToken textToken)
        {
            var nextMap = new Dictionary<TextTokenKey, ITextToken>(
                focusedRichTextEditorRecord.CurrentRichTextEditorRow.Map
            );

            nextMap[textToken.Key] = textToken;

            var nextRowInstance = focusedRichTextEditorRecord.GetCurrentRichTextEditorRowAs<RichTextEditorRow>() with
            {
                Map = nextMap.ToImmutableDictionary()
            };
            
            var nextRowMap = new Dictionary<RichTextEditorRowKey, IRichTextEditorRow>(
                focusedRichTextEditorRecord.Map
            );

            nextRowMap[nextRowInstance.Key] = nextRowInstance;

            return focusedRichTextEditorRecord with
            {
                Map = nextRowMap.ToImmutableDictionary()
            };
        }

        private static RichTextEditorRecord InsertNewLine(RichTextEditorRecord focusedRichTextEditorRecord)
        {
            var replacementCurrentToken = focusedRichTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedRichTextEditorRecord = ReplaceCurrentTokenWith(focusedRichTextEditorRecord, replacementCurrentToken);

            var constructedRowInstance = new RichTextEditorRow();
            
            var nextRowMap = new Dictionary<RichTextEditorRowKey, IRichTextEditorRow>(
                focusedRichTextEditorRecord.Map
            );

            nextRowMap.Add(constructedRowInstance.Key, constructedRowInstance);

            var nextRowList = new List<RichTextEditorRowKey>(
                focusedRichTextEditorRecord.Array
            );

            nextRowList.Add(constructedRowInstance.Key);

            return focusedRichTextEditorRecord with
            {
                Map = nextRowMap.ToImmutableDictionary(),
                Array = nextRowList.ToImmutableArray(),
                CurrentTokenIndex = 0,
                CurrentRowIndex = focusedRichTextEditorRecord.CurrentRowIndex + 1
            };
        }
        
        private static (int rowIndex, int tokenIndex, TextTokenBase token) GetPreviousTokenTuple(RichTextEditorRecord focusedRichTextEditorRecord)
        {
            if (focusedRichTextEditorRecord.CurrentTokenIndex == 0)
            {
                if (focusedRichTextEditorRecord.CurrentRowIndex > 0) 
                {
                    var rowIndex = focusedRichTextEditorRecord.CurrentRowIndex - 1;

                    var rowKey = focusedRichTextEditorRecord.Array[rowIndex];

                    var row = focusedRichTextEditorRecord.Map[rowKey];

                    var tokenIndex = row.Array.Length - 1;

                    var tokenKey = row.Array[tokenIndex];
                    
                    var token = row.Map[tokenKey];

                    return (
                        rowIndex, 
                        tokenIndex, 
                        token 
                            as TextTokenBase
                            ?? throw new ApplicationException($"Expected {nameof(TextTokenBase)}")
                    );
                }

                return (
                    focusedRichTextEditorRecord.CurrentRowIndex, 
                    focusedRichTextEditorRecord.CurrentTokenIndex, 
                    focusedRichTextEditorRecord.GetCurrentTextTokenAs<TextTokenBase>()
                );
            }
            else
            {
                var row = focusedRichTextEditorRecord.GetCurrentRichTextEditorRowAs<RichTextEditorRow>();

                var tokenIndex = focusedRichTextEditorRecord.CurrentTokenIndex - 1;

                var tokenKey = row.Array[tokenIndex];
                
                var token = row.Map[tokenKey];

                return (
                    focusedRichTextEditorRecord.CurrentRowIndex, 
                    tokenIndex, 
                    token 
                        as TextTokenBase
                        ?? throw new ApplicationException($"Expected {nameof(TextTokenBase)}")
                );
            }
        }
        
        private static RichTextEditorRecord SetPreviousTokenAsCurrent(RichTextEditorRecord focusedRichTextEditorRecord)
        {
            var replacementCurrentToken = focusedRichTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedRichTextEditorRecord = ReplaceCurrentTokenWith(focusedRichTextEditorRecord, replacementCurrentToken);

            var previousTokenTuple = GetPreviousTokenTuple(focusedRichTextEditorRecord);

            if (previousTokenTuple.rowIndex == focusedRichTextEditorRecord.CurrentRowIndex)
            {
                if (previousTokenTuple.token.Key == focusedRichTextEditorRecord.CurrentTextTokenKey)
                {
                    // No tokens previous to me
                    replacementCurrentToken = focusedRichTextEditorRecord
                        .GetCurrentTextTokenAs<TextTokenBase>() with
                        {
                            IndexInPlainText = 0
                        };

                    return ReplaceCurrentTokenWith(focusedRichTextEditorRecord, replacementCurrentToken);
                }

                // There is a token previous to me on my current row
                var currentRow = focusedRichTextEditorRecord
                    .GetCurrentRichTextEditorRowAs<RichTextEditorRow>() with
                    {
                        
                    };

                var replacementRowDictionary = new Dictionary<TextTokenKey, ITextToken>(currentRow.Map);

                replacementRowDictionary[previousTokenTuple.token.Key] = previousTokenTuple.token with
                {
                    IndexInPlainText = previousTokenTuple.token.PlainText.Length - 1
                };

                var nextRowMap = new Dictionary<RichTextEditorRowKey, IRichTextEditorRow>(
                    focusedRichTextEditorRecord.Map
                );

                nextRowMap[focusedRichTextEditorRecord.CurrentRichTextEditorRowKey] = focusedRichTextEditorRecord
                    .GetCurrentRichTextEditorRowAs<RichTextEditorRow>() with
                    {
                        Map = replacementRowDictionary.ToImmutableDictionary()
                    };

                return focusedRichTextEditorRecord with
                {
                    CurrentTokenIndex = previousTokenTuple.tokenIndex
                };
            }
            else
            {
                // There was a previous token HOWEVER, it was located on previous row
                var previousRowKey = focusedRichTextEditorRecord.Array[previousTokenTuple.rowIndex];

                var previousRow = focusedRichTextEditorRecord.Map[previousRowKey];

                var replacementRowDictionary = new Dictionary<TextTokenKey, ITextToken>(previousRow.Map);

                replacementRowDictionary[previousTokenTuple.token.Key] = previousTokenTuple.token with
                {
                    IndexInPlainText = previousTokenTuple.token.PlainText.Length - 1
                };

                var nextRowMap = new Dictionary<RichTextEditorRowKey, IRichTextEditorRow>(
                    focusedRichTextEditorRecord.Map
                );

                nextRowMap[focusedRichTextEditorRecord.CurrentRichTextEditorRowKey] = focusedRichTextEditorRecord
                    .GetCurrentRichTextEditorRowAs<RichTextEditorRow>() with
                    {
                        Map = replacementRowDictionary.ToImmutableDictionary()
                    };

                return focusedRichTextEditorRecord with
                {
                    CurrentTokenIndex = previousTokenTuple.tokenIndex
                };
            }
        }
    }
}
