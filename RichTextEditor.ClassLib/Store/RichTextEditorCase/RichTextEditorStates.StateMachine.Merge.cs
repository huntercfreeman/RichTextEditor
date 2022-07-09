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
        // Given:
        //
        // 'z Bill'
        //   ^
        //    (Remove Whitespace)
        //
        // tokens: 'z' and 'Bill' must be
        // merged to make the token: 'zBill'
        public static RichTextEditorRecord MergeTokensIfApplicable(RichTextEditorRecord focusedRichTextEditorRecord)
        {
            if (focusedRichTextEditorRecord.CurrentTextToken.Kind != TextTokenKind.Default)
                return focusedRichTextEditorRecord;
            
            var nextTokenTuple = GetNextTokenTuple(focusedRichTextEditorRecord);

            if (nextTokenTuple.token.Kind != TextTokenKind.Default ||
                nextTokenTuple.token.Key == focusedRichTextEditorRecord.CurrentTextTokenKey)
            {
                return focusedRichTextEditorRecord;
            }

            var replacementToken = new DefaultTextToken()
            {
                Content = focusedRichTextEditorRecord.CurrentTextToken.PlainText +
                    nextTokenTuple.token.PlainText,
                IndexInPlainText = focusedRichTextEditorRecord.CurrentTextToken.IndexInPlainText
            };

            var currentRow = focusedRichTextEditorRecord
                .GetCurrentRichTextEditorRowAs<RichTextEditorRow>();

            var replacementTokenMap = new Dictionary<TextTokenKey, ITextToken>(currentRow.Map);
            var replacementTokenList = new List<TextTokenKey>(currentRow.Array);

            replacementTokenList.Remove(nextTokenTuple.token.Key);
            replacementTokenMap.Remove(nextTokenTuple.token.Key);

            replacementTokenList.Remove(focusedRichTextEditorRecord.CurrentTextTokenKey);
            replacementTokenMap.Remove(focusedRichTextEditorRecord.CurrentTextTokenKey);
            
            replacementTokenMap.Add(replacementToken.Key, replacementToken);
            replacementTokenList.Insert(focusedRichTextEditorRecord.CurrentTokenIndex, replacementToken.Key);

            var replacementRowInstance = new RichTextEditorRow(
                currentRow.Key,
                replacementTokenMap.ToImmutableDictionary(), 
                replacementTokenList.ToImmutableArray()
            );

            var nextRowMap = new Dictionary<RichTextEditorRowKey, IRichTextEditorRow>(
                focusedRichTextEditorRecord.Map
            );

            nextRowMap[replacementRowInstance.Key] = replacementRowInstance;

            return focusedRichTextEditorRecord with
            {
                Map = nextRowMap.ToImmutableDictionary()
            };
        }
    }
}
