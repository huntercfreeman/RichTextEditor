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
            TextTokenBase? tokenToInsertBetweenSplit)
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
            TextTokenBase? tokenToInsertBetweenSplit)
        {            
            var rememberCurrentToken = focusedRichTextEditorRecord
                    .GetCurrentTextTokenAs<DefaultTextToken>();

            var rememberTokenIndex = focusedRichTextEditorRecord.CurrentTokenIndex;

            var firstSplitContent = rememberCurrentToken.Content
                .Substring(0, rememberCurrentToken.IndexInPlainText!.Value + 1);

            var secondSplitContent = rememberCurrentToken.Content
                    .Substring(rememberCurrentToken.IndexInPlainText!.Value + 1);

            var tokenFirst = new DefaultTextToken()
            {
                Content = firstSplitContent,
            };
            
            var tokenSecond = new DefaultTextToken()
            {
                Content = secondSplitContent
            };

            var toBeRemovedTokenKey = focusedRichTextEditorRecord.CurrentTextTokenKey;
            var toBeChangedRowKey = focusedRichTextEditorRecord.CurrentRichTextEditorRowKey;

            focusedRichTextEditorRecord = SetPreviousTokenAsCurrent(focusedRichTextEditorRecord);

            var replacementCurrentToken = focusedRichTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedRichTextEditorRecord = ReplaceCurrentTokenWith(focusedRichTextEditorRecord, replacementCurrentToken);

            var toBeChangedRow = focusedRichTextEditorRecord.Map[toBeChangedRowKey]
                as RichTextEditorRow
                ?? throw new ApplicationException($"Expected typeof, '{nameof(RichTextEditorRow)}'");

            var nextTokenMap = new Dictionary<TextTokenKey, ITextToken>(
                toBeChangedRow.Map
            );

            nextTokenMap.Remove(toBeRemovedTokenKey);

            nextTokenMap.Add(tokenFirst.Key, tokenFirst);

            if (tokenToInsertBetweenSplit is not null)
                nextTokenMap.Add(tokenToInsertBetweenSplit.Key, tokenToInsertBetweenSplit);
            
            nextTokenMap.Add(tokenSecond.Key, tokenSecond);
            
            var nextTokenList = new List<TextTokenKey>(
                toBeChangedRow.Array
            );

            nextTokenList.Remove(toBeRemovedTokenKey);

            int insertionOffset = 0;

            nextTokenList.Insert(rememberTokenIndex + insertionOffset++, tokenFirst.Key);
            
            if (tokenToInsertBetweenSplit is not null)
                nextTokenList.Insert(rememberTokenIndex + insertionOffset++, tokenToInsertBetweenSplit.Key);

            nextTokenList.Insert(rememberTokenIndex + insertionOffset++, tokenSecond.Key);
            
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
                CurrentTokenIndex = focusedRichTextEditorRecord.CurrentTokenIndex +
                    (tokenToInsertBetweenSplit is not null ? 2 : 1)
            };
        }

        public static RichTextEditorRecord SplitWhitespaceToken(RichTextEditorRecord focusedRichTextEditorRecord,
            TextTokenBase? tokenToInsertBetweenSplit)
        {
            var rememberCurrentToken = focusedRichTextEditorRecord
                    .GetCurrentTextTokenAs<WhitespaceTextToken>();

            if (rememberCurrentToken.WhitespaceKind != WhitespaceKind.Tab)
                return focusedRichTextEditorRecord;

            var toBeRemovedTokenKey = focusedRichTextEditorRecord.CurrentTextTokenKey;
            var toBeRemovedTokenIndexInPlainText = focusedRichTextEditorRecord.CurrentTextToken.IndexInPlainText;
            var rememberTokenIndex = focusedRichTextEditorRecord.CurrentTokenIndex;
            var toBeChangedRowKey = focusedRichTextEditorRecord.CurrentRichTextEditorRowKey;

            focusedRichTextEditorRecord = SetPreviousTokenAsCurrent(focusedRichTextEditorRecord);

            var replacementCurrentToken = focusedRichTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedRichTextEditorRecord = ReplaceCurrentTokenWith(focusedRichTextEditorRecord, replacementCurrentToken);

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

            for (int i = 0; i < 4; i++)
            {
                var spaceWhiteSpaceToken = new WhitespaceTextToken(spaceKeyDownEventRecord)
                {
                    IndexInPlainText = null
                };

                nextTokenMap.Add(spaceWhiteSpaceToken.Key, spaceWhiteSpaceToken);
                nextTokenList.Insert(rememberTokenIndex + i, spaceWhiteSpaceToken.Key);
            }

            if (tokenToInsertBetweenSplit is not null)
                nextTokenMap.Add(tokenToInsertBetweenSplit.Key, tokenToInsertBetweenSplit);

            if (tokenToInsertBetweenSplit is not null)
                nextTokenList.Insert(rememberTokenIndex + toBeRemovedTokenIndexInPlainText!.Value + 1, 
                    tokenToInsertBetweenSplit.Key);
            
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
                CurrentTokenIndex = rememberTokenIndex + toBeRemovedTokenIndexInPlainText!.Value + 
                    (tokenToInsertBetweenSplit is not null ? 1 : 0)
            };
        }
    }
}
