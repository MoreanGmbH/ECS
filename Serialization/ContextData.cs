#if ECS
using Entitas;
#if UNITY_EDITOR && ODIN_INSPECTOR
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
#if UNITY_EDITOR && ODIN_INSPECTOR
        [OnValueChanged(nameof(ContextUpdated), true)]
#endif
        /// <summary>
        /// Entity's context name, to be matched to the actual <see cref="IContext"/>.
        /// </summary>
        public string Context;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [ShowIf(nameof(ContextIsValid))]

        [InfoBox("Context must have at least one Entity!", InfoMessageType.Error,
            nameof(invalidEntities))]

        [InfoBox("Null and Empty Entities are not allowed!", InfoMessageType.Error,
            nameof(invalidEntity))]

        [InfoBox("Null Components are not allowed!", InfoMessageType.Error,
            nameof(nullComponent))]

        [InfoBox("Component doesn't belong to selected Context!", InfoMessageType.Error,
            nameof(componentNotInContext))]

        [InfoBox("Only one Component Type per Entity is allowed!", InfoMessageType.Error,
            nameof(duplicateComponents))]

        [OnValueChanged(nameof(Validate), true)]
#endif
        /// <summary>
        /// Array of entity's components.
        /// </summary>
        public IComponent[][] Entities;

#if UNITY_EDITOR && ODIN_INSPECTOR
        private bool ContextIsValid() => !string.IsNullOrEmpty(Context);

        private bool invalidContext;
        private bool invalidEntities;
        private bool invalidEntity;
        private bool nullComponent;
        private bool componentNotInContext;
        private bool duplicateComponents;

        private void ContextUpdated()
        {
            if (string.IsNullOrEmpty(Context))
            {
                Context = Contexts.sharedInstance.allContexts[0].contextInfo.name;
            }

            Entities = new IComponent[1][];
        }

        private void Validate()
        {
            invalidContext = false;
            invalidEntities = false;
            invalidEntity = false;
            nullComponent = false;
            componentNotInContext = false;
            duplicateComponents = false;

            // Verify that context is not null
            if (string.IsNullOrEmpty(Context))
            {
                invalidContext = true;
                return;
            }

            // Verify that there's at least one entity
            if (Entities == null || Entities.Length < 1)
            {
                invalidEntities = true;
                return;
            };

            foreach (var entity in Entities)
            {
                // Verify that there are no null or empty entities
                if (entity == null || entity.Length < 1)
                {
                    invalidEntity = true;
                    return;
                };

                foreach (var component in entity)
                {
                    // Verify that component is not null
                    if (component == null)
                    {
                        nullComponent = true;
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