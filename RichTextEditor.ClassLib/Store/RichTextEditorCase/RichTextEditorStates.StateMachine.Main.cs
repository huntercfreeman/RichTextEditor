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
    }
}
