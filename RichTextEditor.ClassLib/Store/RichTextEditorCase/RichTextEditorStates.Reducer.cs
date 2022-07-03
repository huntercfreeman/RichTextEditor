using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluxor;
using RichTextEditor.ClassLib.Keyboard;
using RichTextEditor.ClassLib.Store.KeyDownEventCase;

namespace RichTextEditor.ClassLib.Store.RichTextEditorCase;

public partial record RichTextEditorStates
{
    public class RichTextEditorStatesReducer
    {
        [ReducerMethod]
        public static RichTextEditorStates ReduceConstructRichTextEditorAction(RichTextEditorStates previousRichTextEditorStates,
            ConstructRichTextEditorRecordAction constructRichTextEditorRecordAction)
        {
            var nextMap = new Dictionary<RichTextEditorKey, IRichTextEditor>(previousRichTextEditorStates.Map);
            var nextList = new List<RichTextEditorKey>(previousRichTextEditorStates.Array);

            var richTextEditor = new RichTextEditorRecord(constructRichTextEditorRecordAction.RichTextEditorKey);

            nextMap[constructRichTextEditorRecordAction.RichTextEditorKey] = richTextEditor;
            nextList.Add(richTextEditor.RichTextEditorKey);

            return new RichTextEditorStates(nextMap.ToImmutableDictionary(), nextList.ToImmutableArray());
        }
        
        [ReducerMethod]
        public static RichTextEditorStates ReduceDeconstructRichTextEditorRecordAction(RichTextEditorStates previousRichTextEditorStates,
            DeconstructRichTextEditorRecordAction deconstructRichTextEditorRecordAction)
        {
            var nextMap = new Dictionary<RichTextEditorKey, IRichTextEditor>(previousRichTextEditorStates.Map);
            var nextList = new List<RichTextEditorKey>(previousRichTextEditorStates.Array);

            nextMap.Remove(deconstructRichTextEditorRecordAction.RichTextEditorKey);
            nextList.Remove(deconstructRichTextEditorRecordAction.RichTextEditorKey);

            return new RichTextEditorStates(nextMap.ToImmutableDictionary(), nextList.ToImmutableArray());
        }
        
        [ReducerMethod]
        public static RichTextEditorStates ReduceKeyDownEventAction(RichTextEditorStates previousRichTextEditorStates,
            KeyDownEventAction keyDownEventAction)
        {
            var nextMap = new Dictionary<RichTextEditorKey, IRichTextEditor>(previousRichTextEditorStates.Map);
            var nextList = new List<RichTextEditorKey>(previousRichTextEditorStates.Array);
            
            var focusedRichTextEditor = previousRichTextEditorStates.Map[keyDownEventAction.FocusedRichTextEditorKey]
                as RichTextEditorRecord;

            if (focusedRichTextEditor is null) 
                return previousRichTextEditorStates;
            
            nextMap[keyDownEventAction.FocusedRichTextEditorKey] = RichTextEditorStates.StateMachine
                .HandleKeyDownEvent(focusedRichTextEditor, keyDownEventAction.KeyDownEventRecord);

            return new RichTextEditorStates(nextMap.ToImmutableDictionary(), nextList.ToImmutableArray());
        }
    }
}

