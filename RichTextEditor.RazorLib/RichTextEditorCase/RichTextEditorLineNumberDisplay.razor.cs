using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace RichTextEditor.RazorLib.RichTextEditorCase;

public partial class RichTextEditorLineNumberDisplay : ComponentBase
{
    [Parameter]
    public int IndexOfRow { get; set; }
    [Parameter]
    public int MostDigitsInARowNumber { get; set; }

    private int CountOfDigitsInRowNumber => (IndexOfRow + 1).ToString().Length;
    private string WidthStyleCss => $"width: {MostDigitsInARowNumber}ch;";
    private string PaddingLeftStyleCss => $"padding-left: {MostDigitsInARowNumber - CountOfDigitsInRowNumber}ch;";
}
