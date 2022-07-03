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
    private record RichTextEditorRecord(RichTextEditorKey RichTextEditorKey) : IRichTextEditor
    {
        public StringBuilder Content { get; init; } = new();
        public string Text => Content.ToString();
    }
}
