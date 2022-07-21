#if ECS
using Entitas;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using System;
using System.Linq;
#endif

namespace ECS
{
    /// <summary>
    /// Structure to be used when serializing / deserializing an entity to / from json.
    /// </summary>
    public struct ContextData
    {
        /// <summary>
        /// Entity's context name, to be matched to the actual <see cref="IContext"/>.
        /// </summary>
        public string Context;

        /// <summary>
        /// Array of entity's components.
        /// </summary>
#if ODIN_INSPECTOR
        [ShowIf(nameof(ContextIsValid))]

        [InfoBox("Null and Empty Entities are not allowed!", InfoMessageType.Error,
            nameof(nullEntities))]

        [InfoBox("Null Components are not allowed!", InfoMessageType.Error,
            nameof(nullComponents))]

        [InfoBox("Component doesn't belong to selected Context!", InfoMessageType.Error,
            nameof(componentNotInContext))]

        [InfoBox("Only one Component Type per Entity is allowed!", InfoMessageType.Error,
            nameof(duplicateComponents))]

        [OnValueChanged(nameof(OnComponentAdded), true)]
#endif
        public IComponent[][] Entities;

#if ODIN_INSPECTOR
        private bool ContextIsValid() => !string.IsNullOrEmpty(Context);

        private bool nullEntities;
        private bool nullComponents;
        private bool componentNotInContext;
        private bool duplicateComponents;

        private void OnComponentAdded()
        {
            nullEntities = false;
            nullComponents = false;
            componentNotInContext = false;
            duplicateComponents = false;

            foreach (var entity in Entities)
            {
                // Verify that there are no null or empty entities
                if (entity == null || entity.Length < 1)
                {
                    nullEntities = true;
                    return;
                };

                foreach (var component in entity)
                {
                    // Verify that component is not null
                    if (component == null)
                    {
                        nullComponents = true;
                        return;
                    }

                    // Verify that component is in context
                    if (!this.GetContext().contextInfo.componentTypes.Contains(component.GetType()))
                    {
                        componentNotInContext = true;
                        return;
                    }

                    // Verify that there are no duplicate components
                    var componentTypeCount = 0;
                    foreach (var otherComponent in entity)
                    {
                        if (otherComponent.GetType() == component.GetType())
                        {
                            componentTypeCount++;
                        }
                        if (componentTypeCount > 1)
                        {
                            duplicateComponents = true;
                            return;
                        }
                    }
                }
            }
        }
#endif
    }
}
#endif