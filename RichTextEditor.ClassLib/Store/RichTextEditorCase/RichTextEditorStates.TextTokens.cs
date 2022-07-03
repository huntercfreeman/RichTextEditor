using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RichTextEditor.ClassLib.Keyboard;

namespace RichTextEditor.ClassLib.Store.RichTextEditorCase;

public partial record RichTextEditorStates
{
    private abstract record TextTokenBase : ITextToken
    {
        public abstract string PlainText { get; }
    }

    private record StartOfRowTextToken : TextTokenBase
    {
        public override string PlainText => "\n";
    }

    private record DefaultTextToken : TextTokenBase
    {
        // TODO: Immutable, efficient, updating of the _content string when user types.
        private string _content;

        public DefaultTextToken(KeyDownEventRecord keyDownEventRecord)
        {
            _content = keyDownEventRecord.Key;
        }
        
        public override string PlainText => _content;
    }

    private record WhitespaceTextToken : TextTokenBase
    {
        // TODO: Immutable, efficient, updating of the _content string when user types.
        private string _content;

        public WhitespaceTextToken(KeyDownEventRecord keyDownEventRecord)
        {
            switch (keyDownEventRecord.Key)
            {
                case KeyboardKeyFacts.WhitespaceKeys.SPACE_KEY:
                    _content = " ";
                    break;
                case KeyboardKeyFacts.WhitespaceKeys.TAB_KEY:
                    _content = "\t";
                    break;
                default:
                    throw new ApplicationException(
                        $"The whitespace key: {keyDownEventRecord.Key} was not recognized.");
            }
        }
        
        public override string PlainText => _content;
    }
}
