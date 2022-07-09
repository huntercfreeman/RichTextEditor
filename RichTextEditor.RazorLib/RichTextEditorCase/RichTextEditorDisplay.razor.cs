using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RichTextEditor.ClassLib.Keyboard;
using RichTextEditor.ClassLib.Store.KeyDownEventCase;
using RichTextEditor.ClassLib.Store.RichTextEditorCase;

namespace RichTextEditor.RazorLib.RichTextEditorCase;

public partial class RichTextEditorDisplay : FluxorComponent, IDisposable
{
    [Inject]
    private IStateSelection<RichTextEditorStates, IRichTextEditor?> RichTextEditorSelector { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter, EditorRequired]
    public RichTextEditorKey RichTextEditorKey { get; set; } = null!;

    private bool _isFocused;
    private ElementReference _inputFocusTrap;

    private string RichTextEditorDisplayId => $"rte_rich-text-editor-display_{RichTextEditorKey.Guid}";
    private string InputFocusTrapId => $"rte_focus-trap_{RichTextEditorKey.Guid}";
    private string ActiveRowId => $"rte_active-row_{RichTextEditorKey.Guid}";

    private string IsFocusedCssClass => _isFocused
        ? "rte_focused"
        : "";
    
    private string InputFocusTrapTopStyleCss => $"top: calc({RichTextEditorSelector.Value!.CurrentRowIndex + 1}em + {RichTextEditorSelector.Value!.CurrentRowIndex * 8.6767}px - 25px)";

    protected override void OnInitialized()
    {
        RichTextEditorSelector.Select(x => 
        {
            x.Map.TryGetValue(RichTextEditorKey, out var value);
            return value;
        });

        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
           JsRuntime.InvokeVoidAsync("richTextEditor.subscribeScrollIntoView",
                InputFocusTrapId,
                RichTextEditorKey.Guid);
        }

        JsRuntime.InvokeVoidAsync("richTextEditor.scrollIntoViewIfOutOfViewport",
            _inputFocusTrap);

        base.OnAfterRender(firstRender);
    }

    private void OnKeyDown(KeyboardEventArgs e)
    {
        Dispatcher.Dispatch(new KeyDownEventAction(RichTextEditorKey, new ClassLib.Keyboard.KeyDownEventRecord(
            e.Key,
            e.Code,
            e.CtrlKey,
            e.ShiftKey,
            e.AltKey
        )));
    }
    
    private void OnFocusIn()
    {
        _isFocused = true;
    }

    private void OnFocusOut()
    {
        _isFocused = false;
    }

    private void FocusInputFocusTrapOnClick()
    {
        _inputFocusTrap.FocusAsync();
    }

    protected override void Dispose(bool disposing)
    {
        JsRuntime.InvokeVoidAsync("richTextEditor.disposeScrollIntoView",
            InputFocusTrapId);
        
        base.Dispose(disposing);
    }
}
