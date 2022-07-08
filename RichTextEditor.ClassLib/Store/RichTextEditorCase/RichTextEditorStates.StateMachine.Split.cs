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
        // Used when cursor is within text and the 'Enter' key is pressed as an example. That token would get split into two separate tokens.
        public static RichTextEditorRecord SplitCurrentToken(RichTextEditorRecord focusedRichTextEditorRecord,
            TextTokenBase tokenToInsertBetweenSplit)
        {
            var currentToken = focusedRichTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();
            
            switch (currentToken.Kind)
            {
                case TextTokenKind.Default:
                    return SplitDefaultToken(focusedRichTextEditorRecord, tokenToInsertBetweenSplit);
                case TextTokenKind.Whitespace:
                    return SplitWhitespaceToken(focusedRichTextEditorRecord, tokenToInsertBetweenSplit);
                default:
                    return focusedRichTextEditorRecord;
            }
        }
        
        public static RichTextEditorRecord SplitDefaultToken(RichTextEditorRecord focusedRichTextEditorRecord,
            TextTokenBase tokenToInsertBetweenSplit)
        {            
            var currentToken = focusedRichTextEditorRecord
                    .GetCurrentTextTokenAs<DefaultTextToken>();

            var firstSplitContent = currentToken.Content
                .Substring(0, currentToken.IndexInPlainText!.Value);

            var secondSplitContent = currentToken.Content
                    .Substring(currentToken.IndexInPlainText!.Value + 1);

            var tokenFirst = new DefaultTextToken()
            {
                Content = firstSplitContent
            };
            
            var tokenSecond = new DefaultTextToken()
            {
                Content = secondSplitContent
            };

            var toBeRemovedTokenKey = focusedRichTextEditorRecord.CurrentTextTokenKey;
            var toBeChangedRowKey = focusedRichTextEditorRecord.CurrentRichTextEditorRowKey;

            focusedRichTextEditorRecord = SetPreviousTokenAsCurrent(focusedRichTextEditorRecord);

            var toBeChangedRow = focusedRichTextEditorRecord.Map[toBeChangedRowKey]
                as RichTextEditorRow
                ?? throw new ApplicationException($"Expected typeof, '{nameof(RichTextEditorRow)}'");

            var nextTokenMap = new Dictionary<TextTokenKey, ITextToken>(
                toBeChangedRow.Map
            );

            nextTokenMap.Remove(toBeRemovedTokenKey);

            nextTokenMap.Add(tokenFirst.Key, tokenFirst);
            nextTokenMap.Add(tokenToInsertBetweenSplit.Key, tokenToInsertBetweenSplit);
            nextTokenMap.Add(tokenSecond.Key, tokenSecond);
            
            var nextTokenList = new List<TextTokenKey>(
                toBeChangedRow.Array
            );

            nextTokenList.Remove(toBeRemovedTokenKey);

            int insertionOffset = 0;

            nextTokenList.Insert(focusedRichTextEditorRecord.CurrentTokenIndex + insertionOffset++, tokenFirst.Key);
            nextTokenList.Insert(focusedRichTextEditorRecord.CurrentTokenIndex + insertionOffset++, tokenToInsertBetweenSplit.Key);
            nextTokenList.Insert(focusedRichTextEditorRecord.CurrentTokenIndex + insertionOffset++, tokenSecond.Key);
            
            var nextRowInstance = toBeChangedRow with
            {
                Map = nextTokenMap.ToImmutableDictionary(),
                Array = nextTokenList.ToImmutableArray()
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

        public static RichTextEditorRecord SplitWhitespaceToken(RichTextEditorRecord focusedRichTextEditorRecord,
            TextTokenBase tokenToInsertBetweenSplit)
        {
            var currentToken = focusedRichTextEditorRecord
                    .GetCurrentTextTokenAs<WhitespaceTextToken>();

            if (currentToken.WhitespaceKind != WhitespaceKind.Tab)
                return focusedRichTextEditorRecord;

            var toBeRemovedTokenKey = focusedRichTextEditorRecord.CurrentTextTokenKey;
            var toBeRemovedTokenIndexInPlainText = focusedRichTextEditorRecord.CurrentTextToken.IndexInPlainText;
            var toBeChangedRowKey = focusedRichTextEditorRecord.CurrentRichTextEditorRowKey;

            focusedRichTextEditorRecord = SetPreviousTokenAsCurrent(focusedRichTextEditorRecord);

            var toBeChangedRow = focusedRichTextEditorRecord.Map[toBeChangedRowKey]
                as RichTextEditorRow
                ?? throw new ApplicationException($"Expected typeof, '{nameof(RichTextEditorRow)}'");

            var nextTokenMap = new Dictionary<TextTokenKey, ITextToken>(
                toBeChangedRow.Map
            );

            nextTokenMap.Remove(toBeRemovedTokenKey);

            var nextTokenList = new List<TextTokenKey>(
                toBeChangedRow.Array
            );

            nextTokenList.Remove(toBeRemovedTokenKey);

            var spaceKeyDownEventRecord = new KeyDownEventRecord(
                KeyboardKeyFacts.WhitespaceKeys.SPACE_CODE,
                KeyboardKeyFacts.WhitespaceKeys.SPACE_CODE,
                false,
                false,
                false
            );

            int insertionOffset = 0;

            for (; insertionOffset < 4; insertionOffset++)
            {
                var spaceWhiteSpaceToken = new WhitespaceTextToken(spaceKeyDownEventRecord);

                nextTokenMap.Add(spaceWhiteSpaceToken.Key, spaceWhiteSpaceToken);
                nextTokenList.Insert(focusedRichTextEditorRecord.CurrentTokenIndex + insertionOffset, spaceWhiteSpaceToken.Key);
            }

            nextTokenMap.Add(tokenToInsertBetweenSplit.Key, tokenToInsertBetweenSplit);
            nextTokenList.Insert(focusedRichTextEditorRecord.CurrentTokenIndex + insertionOffset, tokenToInsertBetweenSplit.Key);
            
            var nextRowInstance = toBeChangedRow with
            {
                Map = nextTokenMap.ToImmutableDictionary(),
                Array = nextTokenList.ToImmutableArray()
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
    }
}
