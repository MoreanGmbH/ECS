#if ODIN_INSPECTOR && ECS
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ECS
{
    [PropertySpace(10), HideReferenceObjectPicker]
    [Toggle(nameof(Valid), CollapseOthersOnExpand = false)]
    public struct Condition
    {
        [ReadOnly]
        public bool Valid;

        [ShowIf("@!Valid"), HideLabel]
        [DisplayAsString(Overflow = false), LabelWidth(50)]
        public string Issue;

        public Color GetColor()
        {
            GUIHelper.RequestRepaint();
            Color color;
            ColorUtility.TryParseHtmlString(Valid ? "#32CD32" : "#EE4B2B", out color);
            return color;
        }
    }
}

#endif