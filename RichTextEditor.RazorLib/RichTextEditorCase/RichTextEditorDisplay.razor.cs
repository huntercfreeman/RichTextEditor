using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;
using Microsoft.AspNetCore.Components;
using RichTextEditor.ClassLib.Store.RichTextEditorCase;

namespace RichTextEditor.RazorLib.RichTextEditorCase;

public partial class RichTextEditorDisplay : ComponentBase
{
    [Inject]
    private IStateSelection<RichTextEditorStates, IRichTextEditor> RichTextEditorSelector { get; set; } = null!;

    [Parameter]
    public RichTextEditorKey RichTextEditorKey { get; set; } = null!;

    protected override void OnInitialized()
    {
        RichTextEditorSelector.Select(x => x.Map[RichTextEditorKey]);

        base.OnInitialized();
    }
}
