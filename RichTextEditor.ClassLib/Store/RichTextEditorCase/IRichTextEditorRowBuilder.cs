using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RichTextEditor.ClassLib.Store.RichTextEditorCase;

public interface IRichTextEditorRowBuilder
{
    public IRichTextEditorRowBuilder Add(ITextToken token);
    public IRichTextEditorRowBuilder Insert(int index, ITextToken token);
    public IRichTextEditorRowBuilder Remove(TextTokenKey textTokenKey);
    public IRichTextEditorRowBuilder Replace(TextTokenKey textTokenKey, ITextToken token);
    public IRichTextEditorRow Build();
}