using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using RichTextEditor.ClassLib.Store.RichTextEditorCase;

namespace RichTextEditor.RazorLib.RichTextEditorCase;

public partial class RichTextEditorRowDisplay : FluxorComponent
{
    [CascadingParameter(Name="CurrentRowIndex")]
    public int RichTextEditorCurrentRowIndex { get; set; }
    
    [Parameter]
    public IRichTextEditorRow RichTextEditorRow { get; set; } = null!;
    [Parameter]
    public int Index { get; set; }

    private string IsActiveCss => RichTextEditorCurrentRowIndex == Index
        ? "rte_active"
        : string.Empty;
}
