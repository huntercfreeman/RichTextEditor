using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluxor;

namespace RichTextEditor.ClassLib.Store.RichTextEditorCase;

public partial record RichTextEditorStates
{
    private record RichTextEditorRecord(RichTextEditorKey RichTextEditorKey, 
        ImmutableDictionary<RichTextEditorRowKey, IRichTextEditorRow> Map, 
        ImmutableArray<RichTextEditorRowKey> Array,
        int CurrentRowIndex,
        int CurrentTokenIndex)
            : IRichTextEditor
    {
        public RichTextEditorRecord() : this(RichTextEditorKey.NewRichTextEditorKey(), 
            new Dictionary<RichTextEditorRowKey, IRichTextEditorRow>().ToImmutableDictionary(),
            new RichTextEditorRowKey[0].ToImmutableArray(),
            CurrentRowIndex: 0,
            CurrentTokenIndex: 0)
        {
            
        }

        public RichTextEditorRowKey CurrentRichTextEditorRowKey => Array[CurrentRowIndex];
        public IRichTextEditorRow CurrentRichTextEditorRow => Map[CurrentRichTextEditorRowKey];
        
        public TextTokenKey CurrentTextTokenKey => CurrentRichTextEditorRow.Array[CurrentTokenIndex];
        public ITextToken CurrentTextToken => CurrentRichTextEditorRow.Map[CurrentTextTokenKey];
    }
}
