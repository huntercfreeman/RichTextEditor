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
}
