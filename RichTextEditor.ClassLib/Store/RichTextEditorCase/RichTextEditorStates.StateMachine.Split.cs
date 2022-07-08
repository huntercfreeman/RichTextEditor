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
        public static ImmutableArray<TextTokenBase> SplitCurrentToken(RichTextEditorRecord focusedRichTextEditorRecord,
            TextTokenBase tokenToInsertBetweenSplit)
        {
            var currentToken = focusedRichTextEditorRecord
                    .GetCurrentTextTokenAs<TextTokenBase>();

            return currentToken.Kind switch
            {
                TextTokenKind.Default => SplitDefaultToken(focusedRichTextEditorRecord, tokenToInsertBetweenSplit),
                TextTokenKind.Whitespace => SplitWhitespaceToken(focusedRichTextEditorRecord, tokenToInsertBetweenSplit),
                _ => new TextTokenBase[] { currentToken }.ToImmutableArray()
            };
        }
        
        public static ImmutableArray<TextTokenBase> SplitDefaultToken(RichTextEditorRecord focusedRichTextEditorRecord,
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

            return new TextTokenBase[] { tokenFirst, tokenToInsertBetweenSplit, tokenSecond }.ToImmutableArray();
        }

        public static ImmutableArray<TextTokenBase> SplitWhitespaceToken(RichTextEditorRecord focusedRichTextEditorRecord,
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
                    
                    if (currentToken.IndexInPlainText == i)
                    {
                        tokens.Add(tokenToInsertBetweenSplit);
                    }
                }

                return tokens.ToImmutableArray();
            }

            return new TextTokenBase[] { currentToken }.ToImmutableArray();
        }
    }
}
