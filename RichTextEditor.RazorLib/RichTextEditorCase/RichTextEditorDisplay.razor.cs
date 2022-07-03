using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RichTextEditor.ClassLib.Keyboard;
using RichTextEditor.ClassLib.Store.KeyDownEventCase;
using RichTextEditor.ClassLib.Store.RichTextEditorCase;

namespace RichTextEditor.RazorLib.RichTextEditorCase;

public partial class RichTextEditorDisplay : ComponentBase
{
    [Inject]
    private IStateSelection<RichTextEditorStates, IRichTextEditor> RichTextEditorSelector { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    public RichTextEditorKey RichTextEditorKey { get; set; } = null!;

    private bool _isFocused;

    private string IsFocusedCssClass => _isFocused
        ? "rte_focused"
        : "";

    private ElementReference _beforeInputFocusTrap;
    // TODO: _beforeInputFocusTrap, and _afterInputFocusTrap are incredibly awkward and the user can type into them and the text will be 'written' but never seen causing memory to be eaten with time if they type into them but this is likely inconsequential and instead escaping the focus trap should be reworked entirely.
    private ElementReference _inputFocusTrap;
    private ElementReference _afterInputFocusTrap;

    protected override void OnInitialized()
    {
        RichTextEditorSelector.Select(x => x.Map[RichTextEditorKey]);

        base.OnInitialized();
    }

    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == KeyboardKeyFacts.WhitespaceKeys.TAB_KEY &&
            e.ShiftKey) 
        {
            _beforeInputFocusTrap.FocusAsync();
        }
        else if (e.Key == KeyboardKeyFacts.WhitespaceKeys.TAB_KEY) 
        {
            _afterInputFocusTrap.FocusAsync();
        }
        else
        {
            Dispatcher.Dispatch(new KeyDownEventAction(RichTextEditorKey, new ClassLib.Keyboard.KeyDownEventRecord(
                e.Key,
                e.Code,
                e.CtrlKey,
                e.ShiftKey,
                e.AltKey
            )));
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
