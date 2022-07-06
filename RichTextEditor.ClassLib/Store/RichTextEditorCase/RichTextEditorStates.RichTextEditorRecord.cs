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
        public RichTextEditorRecord(RichTextEditorKey richTextEditorKey) : this(richTextEditorKey, 
            new Dictionary<RichTextEditorRowKey, IRichTextEditorRow>().ToImmutableDictionary(),
            new RichTextEditorRowKey[0].ToImmutableArray(),
            CurrentRowIndex: 0,
            CurrentTokenIndex: 0)
        {
            var startingRow = new RichTextEditorRow();

            Map = new Dictionary<RichTextEditorRowKey, IRichTextEditorRow>
            {
                { 
                    startingRow.Key,
                    startingRow 
                }
            }.ToImmutableDictionary();

            Array = new RichTextEditorRowKey[]
            {
                startingRow.Key
            }.ToImmutableArray();
        }

        public RichTextEditorRowKey CurrentRichTextEditorRowKey => Array[CurrentRowIndex];
        public IRichTextEditorRow CurrentRichTextEditorRow => Map[CurrentRichTextEditorRowKey];
        private RichTextEditorRow StateMachineCurrentRichTextEditorRow => Map[CurrentRichTextEditorRowKey]
            as RichTextEditorRow
            ?? throw new ApplicationException($"Expected {nameof(RichTextEditorRow)}");
        
        public TextTokenKey CurrentTextTokenKey => CurrentRichTextEditorRow.Array[CurrentTokenIndex];
        public ITextToken CurrentTextToken => CurrentRichTextEditorRow.Map[CurrentTextTokenKey];

        public T GetCurrentTextTokenAs<T>()
            where T : class
        {
            return CurrentTextToken as T
                ?? throw new ApplicationException($"Expected {typeof(T).Name}");
        }
        
        public T GetCurrentRichTextEditorRowAs<T>()
            where T : class
        {
            return CurrentRichTextEditorRow as T
                ?? throw new ApplicationException($"Expected {typeof(T).Name}");
        }
    }
}
