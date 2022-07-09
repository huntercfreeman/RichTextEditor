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
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [CascadingParameter(Name="CurrentRowIndex")]
    public int RichTextEditorCurrentRowIndex { get; set; }
    [CascadingParameter(Name="ActiveRowId")]
    public string ActiveRowId { get; set; } = null!;
    [CascadingParameter(Name="RowIndex")]
    public int RowIndex { get; set; }
    [CascadingParameter]
    public RichTextEditorKey RichTextEditorKey { get; set; } = null!;

    [Parameter, EditorRequired]
    public IRichTextEditorRow RichTextEditorRow { get; set; } = null!;
    [Parameter, EditorRequired]
    public int MostDigitsInARowNumber { get; set; }

    private bool _characterWasClicked;

    private string IsActiveCss => RichTextEditorCurrentRowIndex == RowIndex
        ? "rte_active"
        : string.Empty;

    private string WidthStyleCss => $"width: calc(100% - {MostDigitsInARowNumber}ch);";
    
    private string IsActiveRowId => RichTextEditorCurrentRowIndex == RowIndex
        ? ActiveRowId
        : string.Empty;
    
    private void DispatchRichTextEditorOnClickAction()
    {
        if (!_characterWasClicked)
        {
            Dispatcher.Dispatch(new RichTextEditorOnClickAction(
                RichTextEditorKey,
                RowIndex,
                RichTextEditorRow.Array.Length - 1,
                null
            ));
        }
        else
        {
            _characterWasClicked = false;
        }
    }
}
