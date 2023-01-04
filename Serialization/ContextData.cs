#if MOREAN_ECS
using Entitas;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
#endif

namespace Morean.ECS
{
    /// <summary>
    /// Structure to be used when serializing / deserializing an entity to / from json.
    /// </summary>
    public struct ContextData
    {
#if UNITY_EDITOR && ODIN_INSPECTOR
        [PropertyOrder(0), HideLabel, DisplayAsString]
#endif
        /// <summary>
        /// Entity's context name, to be matched to the actual <see cref="IContext"/>.
        /// </summary>
        public string Context;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [PropertyOrder(1)]
        [Button(ButtonSizes.Large, ButtonStyle.Box, Expanded = true)]
        private void AddEntity()
        {
            Array.Resize(ref Entities, Entities.Length + 1);
            ValidateEntities();
        }

        [PropertyOrder(2), PropertySpace(SpaceAfter = 20), ListDrawerSettings(HideAddButton = true)]
        [InfoBox("Only one Component Type per Entity is allowed!", InfoMessageType.Error,
            nameof(duplicateComponents))]
        [OnValueChanged(nameof(ValidateEntities), true)]
        [ValueDropdown(nameof(Components))]
#endif
        /// <summary>
        /// Entities are collection of components.
        /// </summary>
        public IComponent[][] Entities;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [UnityEngine.HideInInspector]
        private bool duplicateComponents;

        public bool IsValid() => !duplicateComponents;

        private IEnumerable<IComponent> Components()
        {
            var components = new List<IComponent>();
            var componentTypes = ECS.Context.GetContext(Context).contextInfo.componentTypes;

            foreach (var componentType in componentTypes)
            {
                components.Add((IComponent)Activator.CreateInstance(componentType));
            }
            return components;
        }

        private void ValidateEntities()
        {
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
                    // Verify that there are no duplicate components
                    var componentTypeCount = 0;
                    foreach (var otherComponent in entity)
                    {
                        if (otherComponent.GetType() == entity[i].GetType())
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