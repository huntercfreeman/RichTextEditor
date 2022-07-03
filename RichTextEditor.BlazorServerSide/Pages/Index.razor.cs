using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RichTextEditor.ClassLib.Services;
using RichTextEditor.ClassLib.Store.RichTextEditorCase;

namespace RichTextEditor.BlazorServerSide.Pages;

public partial class Index : ComponentBase, IDisposable
{
    [Inject]
    private IRichTextEditorService RichTextEditorService { get; set; } = null!;

    private RichTextEditorKey _richTextEditorKey = RichTextEditorKey.NewRichTextEditorKey();

    protected override void OnInitialized()
    {
        RichTextEditorService.ConstructRichTextEditor(_richTextEditorKey);

        base.OnInitialized();
    }

    public void Dispose()
    {
        RichTextEditorService.DeconstructRichTextEditor(_richTextEditorKey);
    }
}
