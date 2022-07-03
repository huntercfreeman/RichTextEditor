using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;

namespace RichTextEditor.ClassLib.Store.RichTextEditorCase;

public partial record RichTextEditorStates
{
    private record RichTextEditorRecord(RichTextEditorKey RichTextEditorKey) : IRichTextEditor
    {

    }
}
