using System.Collections.Immutable;

namespace RichTextEditor.ClassLib.Store.RichTextEditorCase;

public partial record RichTextEditorStates
{
    private record RichTextEditorRow(RichTextEditorRowKey RichTextEditorRowKey, 
        ImmutableDictionary<TextTokenKey, ITextToken> Map, 
        ImmutableArray<TextTokenKey> Array)
            : IRichTextEditorRow
    {
        public RichTextEditorRow() : this(RichTextEditorRowKey.NewRichTextEditorRowKey(), 
            new Dictionary<TextTokenKey, ITextToken>().ToImmutableDictionary(),
            new TextTokenKey[0].ToImmutableArray())
        {
            
        }
    }
}
