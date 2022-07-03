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
    private class StateMachine
    {
        public static IRichTextEditor HandleKeyDownEvent(RichTextEditorRecord focusedRichTextEditorRecord, 
            KeyDownEventRecord keyDownEventRecord)
        {
            if (KeyboardKeyFacts.IsWhitespaceKey(keyDownEventRecord))
            {
                if (KeyboardKeyFacts.WhitespaceKeys.ENTER_CODE == keyDownEventRecord.Code)
                {
                    return InsertNewLine(focusedRichTextEditorRecord);
                }
                else
                {
                    var whitespaceTextToken = new WhitespaceTextToken(keyDownEventRecord);
                
                    return InsertNewCurrentTokenAfterCurrentPosition(focusedRichTextEditorRecord,
                        whitespaceTextToken);
                }
            }
            else if (KeyboardKeyFacts.IsMovementKey(keyDownEventRecord))
            {
                
            }
            else if (KeyboardKeyFacts.IsMetaKey(keyDownEventRecord)) 
            {
                
            }
            else
            {
                if (focusedRichTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Default)
                {
                    var previousDefaultToken = focusedRichTextEditorRecord.GetCurrentTextTokenAs<DefaultTextToken>();

                    var nextDefaultToken = previousDefaultToken with
                    {
                        Content = previousDefaultToken.Content + keyDownEventRecord.Key
                    };
                    
                    return ReplaceCurrentTokenWith(focusedRichTextEditorRecord, nextDefaultToken);
                }
                else
                {
                    var defaultTextToken = new DefaultTextToken
                    {
                        Content = keyDownEventRecord.Key
                    };
                    
                    return InsertNewCurrentTokenAfterCurrentPosition(focusedRichTextEditorRecord,
                        defaultTextToken);
                }
            }

            return focusedRichTextEditorRecord;   
        }

        private static IRichTextEditor InsertNewCurrentTokenAfterCurrentPosition(RichTextEditorRecord focusedRichTextEditorRecord,
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
        
        private static IRichTextEditor ReplaceCurrentTokenWith(RichTextEditorRecord focusedRichTextEditorRecord,
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

        private static IRichTextEditor InsertNewLine(RichTextEditorRecord focusedRichTextEditorRecord)
        {
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
    }
}
