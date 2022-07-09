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
    [CascadingParameter(Name="ActiveRowId")]
    public string ActiveRowId { get; set; } = null!;
    [CascadingParameter(Name="RowIndex")]
    public int RowIndex { get; set; }

    [Parameter, EditorRequired]
    public IRichTextEditorRow RichTextEditorRow { get; set; } = null!;
    [Parameter, EditorRequired]
    public int MostDigitsInARowNumber { get; set; }

    private string IsActiveCss => RichTextEditorCurrentRowIndex == RowIndex
        ? "rte_active"
        : string.Empty;

    private string WidthStyleCss => $"width: calc(100% - {MostDigitsInARowNumber}ch);";
    
    private string IsActiveRowId => RichTextEditorCurrentRowIndex == RowIndex
        ? ActiveRowId
        : string.Empty;
}
