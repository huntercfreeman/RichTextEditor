using RichTextEditor.ClassLib.Store.RichTextEditorCase;

namespace RichTextEditor.ClassLib.Services;

public interface IRichTextEditorService
{
    public void ConstructRichTextEditor(RichTextEditorKey richTextEditorKey);
    public void DeconstructRichTextEditor(RichTextEditorKey richTextEditorKey);
}
