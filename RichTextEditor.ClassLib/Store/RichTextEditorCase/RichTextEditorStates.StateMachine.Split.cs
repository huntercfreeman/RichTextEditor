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
            
            var rememberTokenIndex = focusedRichTextEditorRecord.CurrentTokenIndex;
            var rememberRowIndex = focusedRichTextEditorRecord.CurrentRowIndex;

            (ImmutableArray<TextTokenBase> tokens, int indexOfInsertedToken) tokenChanges;

            switch (currentToken.Kind)
            {
                case TextTokenKind.Default:
                    tokenChanges = SplitDefaultToken(focusedRichTextEditorRecord, tokenToInsertBetweenSplit);
                    break;
                case TextTokenKind.Whitespace:
                    tokenChanges = SplitWhitespaceToken(focusedRichTextEditorRecord, tokenToInsertBetweenSplit);
                    break;
                default:
                    return focusedRichTextEditorRecord;
            }

            focusedRichTextEditorRecord = RemoveCurrentToken(focusedRichTextEditorRecord);

            foreach (var token in tokenChanges.tokens)
            {
                focusedRichTextEditorRecord = InsertNewCurrentTokenAfterCurrentPosition(
                    focusedRichTextEditorRecord,
                    token
                );
            }

            focusedRichTextEditorRecord = focusedRichTextEditorRecord with
            {
                CRI
            };

            return currentToken.Kind switch
            {
                TextTokenKind.Default => ,
                TextTokenKind. => SplitWhitespaceToken(focusedRichTextEditorRecord, tokenToInsertBetweenSplit),
                _ => ( new TextTokenBase[] { currentToken }.ToImmutableArray(), 0 /* Index of tokenToInsertBetweenSplit */ )
            };
        }
        
        public static (ImmutableArray<TextTokenBase> tokens, int indexOfInsertedToken) SplitDefaultToken(RichTextEditorRecord focusedRichTextEditorRecord,
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

            return (
                new TextTokenBase[] { tokenFirst, tokenToInsertBetweenSplit, tokenSecond }.ToImmutableArray(),
                1 // Index of tokenToInsertBetweenSplit
            );
        }

        public static (ImmutableArray<TextTokenBase> tokens, int indexOfInsertedToken) SplitWhitespaceToken(RichTextEditorRecord focusedRichTextEditorRecord,
            TextTokenBase tokenToInsertBetweenSplit)
        {
            var currentToken = focusedRichTextEditorRecord
                    .GetCurrentTextTokenAs<WhitespaceTextToken>();

            if (currentToken.WhitespaceKind == WhitespaceKind.Tab)
            {
                var spaceKeyDownEventRecord = new KeyDownEventRecord(
                    KeyboardKeyFacts.WhitespaceKeys.SPACE_CODE,
                    KeyboardKeyFacts.WhitespaceKeys.SPACE_CODE,
                    false,
                    false,
                    false
                );

                List<TextTokenBase> tokens = new List<TextTokenBase>();

                for (int i = 0; i < 4; i++)
                {
                    tokens.Add(new WhitespaceTextToken(spaceKeyDownEventRecord));
                }

                int indexOfTokenToInsertBetweenSplit = currentToken.IndexInPlainText!.Value + 1;
    
                tokens.Insert(indexOfTokenToInsertBetweenSplit, tokenToInsertBetweenSplit);

                return (
                    tokens.ToImmutableArray(),
                    indexOfTokenToInsertBetweenSplit
                );
            }

            return (
                new TextTokenBase[] { currentToken }.ToImmutableArray(),
                0 // Index of tokenToInsertBetweenSplit
            );
        }
    }
}
