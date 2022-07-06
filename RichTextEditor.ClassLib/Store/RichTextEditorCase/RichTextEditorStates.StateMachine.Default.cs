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
        public static RichTextEditorRecord HandleDefaultInsert(RichTextEditorRecord focusedRichTextEditorRecord, 
            KeyDownEventRecord keyDownEventRecord)
        {
            if (focusedRichTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Default)
            {
                var previousDefaultToken = focusedRichTextEditorRecord.GetCurrentTextTokenAs<DefaultTextToken>();

                var content = previousDefaultToken.Content + keyDownEventRecord.Key;

                var nextDefaultToken = previousDefaultToken with
                {
                    Content = content,
                    IndexInPlainText = content.Length - 1
                };
                
                return ReplaceCurrentTokenWith(focusedRichTextEditorRecord, nextDefaultToken);
            }
            else
            {
                var replacementCurrentToken = focusedRichTextEditorRecord
                    .GetCurrentTextTokenAs<TextTokenBase>() with
                    {
                        IndexInPlainText = null
                    };

                focusedRichTextEditorRecord = ReplaceCurrentTokenWith(focusedRichTextEditorRecord, replacementCurrentToken);

                var defaultTextToken = new DefaultTextToken
                {
                    Content = keyDownEventRecord.Key,
                    IndexInPlainText = 0
                };
                
                return InsertNewCurrentTokenAfterCurrentPosition(focusedRichTextEditorRecord,
                    defaultTextToken);
            }
        }
        
        public static RichTextEditorRecord HandleDefaultBackspace(RichTextEditorRecord focusedRichTextEditorRecord, 
            KeyDownEventRecord keyDownEventRecord)
        {
            var previousDefaultTextToken = focusedRichTextEditorRecord.GetCurrentTextTokenAs<DefaultTextToken>();

            var firstSplitContent = previousDefaultTextToken.Content
                .Substring(0, previousDefaultTextToken.IndexInPlainText!.Value);

            var secondSplitContent = string.Empty;

            if (previousDefaultTextToken.IndexInPlainText != previousDefaultTextToken.Content.Length - 1)
            {
                secondSplitContent = previousDefaultTextToken.Content
                    .Substring(previousDefaultTextToken.IndexInPlainText!.Value + 1);
            }

            var nextDefaultToken = previousDefaultTextToken with
                {
                    Content = firstSplitContent + secondSplitContent,
                    IndexInPlainText = previousDefaultTextToken.IndexInPlainText - 1
                };

            if (nextDefaultToken.Content.Length == 0)
                return RemoveCurrentToken(focusedRichTextEditorRecord);

            focusedRichTextEditorRecord = ReplaceCurrentTokenWith(focusedRichTextEditorRecord, nextDefaultToken);

            if (nextDefaultToken.IndexInPlainText == -1)
                return SetPreviousTokenAsCurrent(focusedRichTextEditorRecord);

            return focusedRichTextEditorRecord;
        }
    }
}
