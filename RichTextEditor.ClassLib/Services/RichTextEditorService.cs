using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;
using RichTextEditor.ClassLib.Store.RichTextEditorCase;

namespace RichTextEditor.ClassLib.Services;

public class RichTextEditorService : IRichTextEditorService
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
    
    public void DeconstructRichTextEditor(RichTextEditorKey richTextEditorKey)
    {
        dispatcher.Dispatch(new DeconstructRichTextEditorRecordAction(richTextEditorKey));
    }
}
