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
            var nextRichTextEditorMap = new Dictionary<RichTextEditorKey, IRichTextEditor>(previousRichTextEditorStates.Map);
            var nextRichTextEditorList = new List<RichTextEditorKey>(previousRichTextEditorStates.Array);

            var richTextEditor = new RichTextEditorRecord();

            nextRichTextEditorMap[constructRichTextEditorRecordAction.RichTextEditorKey] = richTextEditor;
            nextRichTextEditorList.Add(richTextEditor.RichTextEditorKey);

            return new RichTextEditorStates(nextRichTextEditorMap.ToImmutableDictionary(), nextRichTextEditorList.ToImmutableArray());
        }
        
        [ReducerMethod]
        public static RichTextEditorStates ReduceDeconstructRichTextEditorRecordAction(RichTextEditorStates previousRichTextEditorStates,
            DeconstructRichTextEditorRecordAction deconstructRichTextEditorRecordAction)
        {
            var nextRichTextEditorMap = new Dictionary<RichTextEditorKey, IRichTextEditor>(previousRichTextEditorStates.Map);
            var nextRichTextEditorList = new List<RichTextEditorKey>(previousRichTextEditorStates.Array);

            nextRichTextEditorMap.Remove(deconstructRichTextEditorRecordAction.RichTextEditorKey);
            nextRichTextEditorList.Remove(deconstructRichTextEditorRecordAction.RichTextEditorKey);

            return new RichTextEditorStates(nextRichTextEditorMap.ToImmutableDictionary(), nextRichTextEditorList.ToImmutableArray());
        }
        
        [ReducerMethod]
        public static RichTextEditorStates ReduceKeyDownEventAction(RichTextEditorStates previousRichTextEditorStates,
            KeyDownEventAction keyDownEventAction)
        {
            var nextRichTextEditorMap = new Dictionary<RichTextEditorKey, IRichTextEditor>(previousRichTextEditorStates.Map);
            var nextRichTextEditorList = new List<RichTextEditorKey>(previousRichTextEditorStates.Array);
            
            var focusedRichTextEditor = previousRichTextEditorStates.Map[keyDownEventAction.FocusedRichTextEditorKey]
                as RichTextEditorRecord;

            if (focusedRichTextEditor is null) 
                return previousRichTextEditorStates;
            
            nextRichTextEditorMap[keyDownEventAction.FocusedRichTextEditorKey] = RichTextEditorStates.StateMachine
                .HandleKeyDownEvent(focusedRichTextEditor, keyDownEventAction.KeyDownEventRecord);

            return new RichTextEditorStates(nextRichTextEditorMap.ToImmutableDictionary(), nextRichTextEditorList.ToImmutableArray());
        }
    }
}

