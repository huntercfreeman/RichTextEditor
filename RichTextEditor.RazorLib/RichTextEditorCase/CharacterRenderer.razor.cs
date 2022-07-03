using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace RichTextEditor.RazorLib.RichTextEditorCase;

public partial class CharacterRenderer : ComponentBase
{
    [CascadingParameter]
    public bool IsFocused { get; set; }
    
    [Parameter]
    // The html escaped character for space is &nbsp; which
    // requires more than 1 character to represent therefore this is of type string
    public string Character { get; set; } = null!;
    [Parameter]
    public bool ShouldDisplayCursor { get; set; }
}
