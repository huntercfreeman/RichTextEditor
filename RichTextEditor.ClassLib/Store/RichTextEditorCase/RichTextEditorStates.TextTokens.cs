using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RichTextEditor.ClassLib.Store.RichTextEditorCase;

public partial record RichTextEditorStates
{
    private abstract record TextTokenBase : ITextToken
    {
        
    }
    
    private record StartOfRowTextToken : TextTokenBase
    {

    }

    private record DefaultTextToken : TextTokenBase
    {

    }

    private record WhitespaceTextToken : TextTokenBase
    {

    }
}
