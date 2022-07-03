using System.Collections.Immutable;

namespace RichTextEditor.ClassLib.Store.RichTextEditorCase;

public partial record RichTextEditorStates
{
    private record RichTextEditorRow(RichTextEditorRowKey Key, 
        ImmutableDictionary<TextTokenKey, ITextToken> Map, 
        ImmutableArray<TextTokenKey> Array)
            : IRichTextEditorRow
    {
        public RichTextEditorRow() : this(RichTextEditorRowKey.NewRichTextEditorRowKey(), 
            new Dictionary<TextTokenKey, ITextToken>().ToImmutableDictionary(),
            new TextTokenKey[0].ToImmutableArray())
        {
            var startOfRowToken = new StartOfRowTextToken();

            Map = new Dictionary<TextTokenKey, ITextToken>
            {
                { 
                    startOfRowToken.Key, 
                    startOfRowToken
                }
            }.ToImmutableDictionary();

            Array = new TextTokenKey[]
            {
                startOfRowToken.Key
            }.ToImmutableArray();
        }
    }
}
