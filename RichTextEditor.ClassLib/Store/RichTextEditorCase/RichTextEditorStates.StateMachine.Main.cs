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
        public static RichTextEditorRecord HandleKeyDownEvent(RichTextEditorRecord focusedRichTextEditorRecord, 
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
                return HandleDefaultInsert(focusedRichTextEditorRecord, keyDownEventRecord);
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
        
        private static RichTextEditorRecord RemoveCurrentToken(RichTextEditorRecord focusedRichTextEditorRecord)
        {
            if (focusedRichTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.StartOfRow)
            {
                if (focusedRichTextEditorRecord.CurrentRowIndex == 0)
                {
                    return focusedRichTextEditorRecord;
                }
                
                if (focusedRichTextEditorRecord.CurrentRichTextEditorRow.Array.Length == 1)
                {
                    return RemoveCurrentRow(focusedRichTextEditorRecord);
                }

                // TODO: Move current row to end of previous row
                return focusedRichTextEditorRecord;
            }

            var toBeRemovedTokenKey = focusedRichTextEditorRecord.CurrentTextTokenKey;
            var toBeChangedRow = focusedRichTextEditorRecord.GetCurrentRichTextEditorRowAs<RichTextEditorRow>();

            focusedRichTextEditorRecord = SetPreviousTokenAsCurrent(focusedRichTextEditorRecord);

            var nextMap = new Dictionary<TextTokenKey, ITextToken>(
                toBeChangedRow.Map
            );

            nextMap.Remove(toBeRemovedTokenKey);
            
            var nextList = new List<TextTokenKey>(
                toBeChangedRow.Array
            );

            nextList.Remove(toBeRemovedTokenKey);
            
            var nextRowInstance = toBeChangedRow with
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
            };
        }

        private static RichTextEditorRecord RemoveCurrentRow(RichTextEditorRecord focusedRichTextEditorRecord)
        {
            var toBeDeletedRow = focusedRichTextEditorRecord.CurrentRichTextEditorRow;

            focusedRichTextEditorRecord = SetPreviousTokenAsCurrent(focusedRichTextEditorRecord);
            
            var nextRowMap = new Dictionary<RichTextEditorRowKey, IRichTextEditorRow>(
                focusedRichTextEditorRecord.Map
            );

            nextRowMap.Remove(toBeDeletedRow.Key);
            
            var nextRowList = new List<RichTextEditorRowKey>(
                focusedRichTextEditorRecord.Array
            );

            nextRowList.Remove(toBeDeletedRow.Key);

            return focusedRichTextEditorRecord with
            {
                Map = nextRowMap.ToImmutableDictionary(),
                Array = nextRowList.ToImmutableArray()
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

            nextRowList.Insert(focusedRichTextEditorRecord.CurrentRowIndex + 1, constructedRowInstance.Key);

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
        
        private static (int rowIndex, int tokenIndex, TextTokenBase token) GetNextTokenTuple(RichTextEditorRecord focusedRichTextEditorRecord)
        {
            var currentRow = focusedRichTextEditorRecord.GetCurrentRichTextEditorRowAs<RichTextEditorRow>();

            if (focusedRichTextEditorRecord.CurrentTokenIndex == currentRow.Array.Length - 1)
            {
                if (focusedRichTextEditorRecord.CurrentRowIndex < focusedRichTextEditorRecord.Array.Length - 1) 
                {
                    var rowIndex = focusedRichTextEditorRecord.CurrentRowIndex + 1;

                    var rowKey = focusedRichTextEditorRecord.Array[rowIndex];

                    var row = focusedRichTextEditorRecord.Map[rowKey];

                    var tokenIndex = 0;

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
                var tokenIndex = focusedRichTextEditorRecord.CurrentTokenIndex + 1;

                var tokenKey = currentRow.Array[tokenIndex];
                
                var token = currentRow.Map[tokenKey];

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
                    .GetCurrentRichTextEditorRowAs<RichTextEditorRow>();

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
                    Map = nextRowMap.ToImmutableDictionary(),
                    CurrentTokenIndex = previousTokenTuple.tokenIndex
                };
            }
            else
            {
                // There was a previous token HOWEVER, it was located on previous row
                var previousRowKey = focusedRichTextEditorRecord.Array[previousTokenTuple.rowIndex];

                var previousRow = focusedRichTextEditorRecord.Map[previousRowKey]
                    as RichTextEditorRow
                    ?? throw new ApplicationException($"Expected {nameof(RichTextEditorRow)}");

                var replacementRowDictionary = new Dictionary<TextTokenKey, ITextToken>(previousRow.Map);

                replacementRowDictionary[previousTokenTuple.token.Key] = previousTokenTuple.token with
                {
                    IndexInPlainText = previousTokenTuple.token.PlainText.Length - 1
                };

                var nextRowMap = new Dictionary<RichTextEditorRowKey, IRichTextEditorRow>(
                    focusedRichTextEditorRecord.Map
                );

                nextRowMap[previousRowKey] = previousRow with
                    {
                        Map = replacementRowDictionary.ToImmutableDictionary()
                    };

                return focusedRichTextEditorRecord with
                {
                    Map = nextRowMap.ToImmutableDictionary(),
                    CurrentTokenIndex = previousTokenTuple.tokenIndex,
                    CurrentRowIndex = previousTokenTuple.rowIndex
                };
            }
        }
        
        private static RichTextEditorRecord SetNextTokenAsCurrent(RichTextEditorRecord focusedRichTextEditorRecord)
        {
            var replacementCurrentToken = focusedRichTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedRichTextEditorRecord = ReplaceCurrentTokenWith(focusedRichTextEditorRecord, replacementCurrentToken);

            var nextTokenTuple = GetNextTokenTuple(focusedRichTextEditorRecord);

            if (nextTokenTuple.rowIndex == focusedRichTextEditorRecord.CurrentRowIndex)
            {
                if (nextTokenTuple.token.Key == focusedRichTextEditorRecord.CurrentTextTokenKey)
                {
                    // No tokens next to me
                    replacementCurrentToken = focusedRichTextEditorRecord
                        .GetCurrentTextTokenAs<TextTokenBase>() with
                        {
                            IndexInPlainText = focusedRichTextEditorRecord.CurrentTextToken.PlainText.Length - 1
                        };

                    return ReplaceCurrentTokenWith(focusedRichTextEditorRecord, replacementCurrentToken);
                }

                // There is a token next to me on my current row
                var currentRow = focusedRichTextEditorRecord
                    .GetCurrentRichTextEditorRowAs<RichTextEditorRow>();

                var replacementRowDictionary = new Dictionary<TextTokenKey, ITextToken>(currentRow.Map);

                replacementRowDictionary[nextTokenTuple.token.Key] = nextTokenTuple.token with
                {
                    IndexInPlainText = 0
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
                    Map = nextRowMap.ToImmutableDictionary(),
                    CurrentTokenIndex = nextTokenTuple.tokenIndex
                };
            }
            else
            {
                // There was a next token HOWEVER, it was located on the next row
                var nextRowKey = focusedRichTextEditorRecord.Array[nextTokenTuple.rowIndex];

                var nextRow = focusedRichTextEditorRecord.Map[nextRowKey]
                    as RichTextEditorRow
                    ?? throw new ApplicationException($"Expected {nameof(RichTextEditorRow)}");

                var replacementRowDictionary = new Dictionary<TextTokenKey, ITextToken>(nextRow.Map);

                replacementRowDictionary[nextTokenTuple.token.Key] = nextTokenTuple.token with
                {
                    IndexInPlainText = 0
                };

                var nextRowMap = new Dictionary<RichTextEditorRowKey, IRichTextEditorRow>(
                    focusedRichTextEditorRecord.Map
                );

                nextRowMap[nextRowKey] = nextRow with
                    {
                        Map = replacementRowDictionary.ToImmutableDictionary()
                    };

                return focusedRichTextEditorRecord with
                {
                    Map = nextRowMap.ToImmutableDictionary(),
                    CurrentTokenIndex = nextTokenTuple.tokenIndex,
                    CurrentRowIndex = nextTokenTuple.rowIndex
                };
            }
        }

        /// <summary>
		/// Returns the inclusive starting column index
		/// </summary>
		/// <param name="nextPlainTextEditorState"></param>
		/// <returns></returns>
		private static int CalculateCurrentTokenColumnIndexRespectiveToRow(
			RichTextEditorRecord focusedRichTextEditorRecord)
		{
			var rollingCount = 0;
            var currentRow = focusedRichTextEditorRecord
                .GetCurrentRichTextEditorRowAs<RichTextEditorRow>();

			foreach (var tokenKey in currentRow.Array)
			{
				if (tokenKey == focusedRichTextEditorRecord.CurrentTextToken.Key)
                {
					return rollingCount;
				}
				else
				{
                    var token = currentRow.Map[tokenKey];
					rollingCount += token.PlainText.Length;
				}
			}

			return 0;
		}

        private static (int inclusiveStartingColumnIndex, int exclusiveEndingColumnIndex, TextTokenBase token) CalculateTokenAtColumnIndexRespectiveToRow(
			RichTextEditorRecord focusedRichTextEditorRecord,
			RichTextEditorRow row,
			int columnIndex)
		{
			var rollingCount = 0;

            for (int i = 0; i < row.Array.Length; i++)
			{
                TextTokenKey tokenKey = row.Array[i];
                ITextToken token = row.Map[tokenKey];

				rollingCount += token.PlainText.Length;

				if (rollingCount > columnIndex || (i == row.Array.Length - 1))
				{
                    return (
                        rollingCount - token.PlainText.Length,
                        rollingCount,
                        token as TextTokenBase
                            ?? throw new ApplicationException($"Expected type {nameof(TextTokenBase)}")
                    );
                }
			}

            throw new ApplicationException("Row was empty");
		}
    }
}
