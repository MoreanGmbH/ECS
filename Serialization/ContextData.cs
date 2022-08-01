#if ECS
using Entitas;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
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
        [OnValueChanged(nameof(InitializeEntities), true)]
        [ValueDropdown(nameof(ContextNames))]
#endif
        /// <summary>
        /// Entity's context name, to be matched to the actual <see cref="IContext"/>.
        /// </summary>
        public string Context;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [InfoBox("Component doesn't belong to the selected Context!", InfoMessageType.Error,
            nameof(componentNotInContext))]

        [InfoBox("Only one Component Type per Entity is allowed!", InfoMessageType.Error,
            nameof(duplicateComponents))]

        [OnValueChanged(nameof(ValidateEntities), true)]
#endif
        /// <summary>
        /// Array of entity's components.
        /// </summary>
        public IComponent[][] Entities;

#if UNITY_EDITOR && ODIN_INSPECTOR
        private bool componentNotInContext;
        private bool duplicateComponents;

        private IEnumerable<string> ContextNames()
            => Contexts.sharedInstance.allContexts.Select(context => context.contextInfo.name);

        private void InitializeEntities() => Entities = new IComponent[1][];

        private void ValidateEntities()
        {
            componentNotInContext = false;
            duplicateComponents = false;

            for (int i = 0; i < Entities.Length; i++)
            {
                // Initialize null or empty entities
                if (Entities[i] == null || Entities[i].Length < 1)
                {
                    Entities[i] = new IComponent[1];
                }

                // Initialize null components
                for (int j = 0; j < Entities[i].Length; j++)
                {
                    if (Entities[i][j] == null)
                    {
                        Entities[i][j] = (IComponent)Activator.CreateInstance(
                            ECS.Context.GetContext(Context).contextInfo.componentTypes[0]);
                    }
                }
            }

            foreach (var entity in Entities)
            {
                for (int i = 0; i < entity.Length; i++)
                {
                    var componentType = entity[i].GetType();

                    // Verify that the component is in context
                    if (!this.GetContext().contextInfo.componentTypes.Contains(componentType))
                    {
                        componentNotInContext = true;
                        return;
                    }

                    // Verify that there are no duplicate components
                    var componentTypeCount = 0;
                    foreach (var otherComponent in entity)
                    {
                        if (otherComponent.GetType() == componentType)
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
