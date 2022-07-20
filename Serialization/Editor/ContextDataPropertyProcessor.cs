#if ODIN_INSPECTOR
using Entitas;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;

namespace ECS
{
    public class ContextDataPropertyProcessor : OdinPropertyProcessor<ContextData>
    {
        public override void ProcessMemberProperties(List<InspectorPropertyInfo> propertyInfos)
        {
            var contextPropetyOrder = 0;
            for (int i = 0; i < propertyInfos.Count; i++)
            {
                // Hide original Context property
                if (propertyInfos[i].PropertyName == nameof(ContextData.Context))
                {
                    contextPropetyOrder = i - 1;
                    propertyInfos.RemoveAt(i);
                }
            }

            // Use IContext type for Context property
            propertyInfos.AddValue(nameof(ContextData.Context),
                // Convert to IContext
                (ref ContextData contextData) => contextData.GetContext(),
                // Convert to oritinal Context type
                (ref ContextData contextData, IContext context) => contextData.Context = context?.contextInfo.name,
                // Place where original Context property was
                new PropertyOrderAttribute(contextPropetyOrder));
        }
    }
}
#endif