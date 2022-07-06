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

public partial class RichTextEditorDisplay : FluxorComponent
{
    [Inject]
    private IStateSelection<RichTextEditorStates, IRichTextEditor?> RichTextEditorSelector { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter]
    public RichTextEditorKey RichTextEditorKey { get; set; } = null!;

    private bool _isFocused;

    private string IsFocusedCssClass => _isFocused
        ? "rte_focused"
        : "";

    private ElementReference _inputFocusTrap;

    protected override void OnInitialized()
    {
        RichTextEditorSelector.Select(x => 
        {
            x.Map.TryGetValue(RichTextEditorKey, out var value);
            return value;
        });

        base.OnInitialized();
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

        JsRuntime.InvokeVoidAsync("richTextEditor.clearInputElement", _inputFocusTrap);

        // TODO: Conditionally call 'preventdefault' for onkeydown events
        if (e.Code == KeyboardKeyFacts.WhitespaceKeys.TAB_CODE)
        {
            _inputFocusTrap.FocusAsync();
        }
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
}
