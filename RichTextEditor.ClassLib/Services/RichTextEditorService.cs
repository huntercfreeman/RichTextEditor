using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;
using RichTextEditor.ClassLib.Store.RichTextEditorCase;

namespace RichTextEditor.BlazorServerSide.Services;

public class RichTextEditorService
{
    private readonly IDispatcher dispatcher;

    public RichTextEditorService(IDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
    }

    public void ConstructRichTextEditor(RichTextEditorKey richTextEditorKey)
    {
        dispatcher.Dispatch(new ConstructRichTextEditorRecordAction(richTextEditorKey));
    }
}
