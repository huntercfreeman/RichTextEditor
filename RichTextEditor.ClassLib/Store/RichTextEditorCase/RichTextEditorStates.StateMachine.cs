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
                    
                    var nextMap = new Dictionary<TextTokenKey, ITextToken>(
                        focusedRichTextEditorRecord.CurrentRichTextEditorRow.Map
                    );

                    nextMap[previousDefaultToken.Key] = previousDefaultToken with
                    {
                        Content = previousDefaultToken.Content + keyDownEventRecord.Key
                    };

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
                else
                {
                    var defaultTextToken = new DefaultTextToken
                    {
                        Content = keyDownEventRecord.Key
                    };
                    
                    var nextMap = new Dictionary<TextTokenKey, ITextToken>(
                        focusedRichTextEditorRecord.CurrentRichTextEditorRow.Map
                    );

                    nextMap[defaultTextToken.Key] = defaultTextToken;
                    
                    var nextList = new List<TextTokenKey>(
                        focusedRichTextEditorRecord.CurrentRichTextEditorRow.Array
                    );

                    nextList.Add(defaultTextToken.Key);
                    
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
            }

            return focusedRichTextEditorRecord;   
        }
    }
}
