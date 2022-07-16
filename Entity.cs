#if ECS
using Entitas;
using System;
using System.Collections.Generic;

namespace ECS
{
    /// <summary>
    /// Provides <see cref="Entitas.Entity"/> related utilities such as
    /// adding, removing and replacing components.
    /// </summary>
    public static class Entity
    {
        #region Components

        /// <summary>
        /// Add <paramref name="components"/> to <paramref name="entity"/>.
        /// </summary>
        internal static void AddComponents(this IEntity entity, params IComponent[] components)
        {
            var componentTypes = entity.contextInfo.componentTypes;
            for (int i = 0; i < components.Length; i++)
            {
                var component = components[i];
                var index = Array.IndexOf(componentTypes, component.GetType());

                if (-1 < index && !entity.HasComponent(index))
                {
                    entity.AddComponent(index, component);
                }
            }
        }

        /// <summary>
        /// Add components matched by <paramref name="indices"/> to <paramref name="entity"/>.
        /// </summary>
        /// <param name="indices">Component indices in the <paramref name="componentTypes"/>.</param>
        internal static void AddComponents(this IEntity entity, int[] indices)
        {
            var componentTypes = entity.contextInfo.componentTypes;
            for (int i = 0; i < indices.Length; i++)
            {
                var index = indices[i];
                var component = entity.CreateComponent(index, componentTypes[index]);

                if (!entity.HasComponent(index))
                {
                    entity.AddComponent(index, component);
                }
            }
        }

        /// <summary>
        /// Replace <paramref name="components"/> to <paramref name="entity"/>.
        /// </summary>
        internal static void ReplaceComponents(this IEntity entity, params IComponent[] components)
        {
            var componentTypes = entity.contextInfo.componentTypes;
            for (int i = 0; i < components.Length; i++)
            {
                var component = components[i];
                var index = Array.IndexOf(componentTypes, component.GetType());

                if (-1 < index && !entity.HasComponent(index))
                {
                    entity.ReplaceComponent(index, component);
                }
            }
        }

        /// <summary>
        /// Subtract <paramref name="entities"/> components from defined <paramref name="indices"/>.
        /// </summary>
        internal static List<IComponent> SubtractComponents(this IEntity[] entities, params int[] indices)
        {
            var subractedComponents = new List<IComponent>();
            for (int i = 0; i < entities.Length; i++)
            {
                subractedComponents.AddRange(
                    entities[i].SubtractComponents(indices));
            }

            return subractedComponents;
        }

        /// <summary>
        /// Subtract <paramref name="entity"/> components from defined <paramref name="indices"/>.
        /// </summary>
        internal static IComponent[] SubtractComponents(this IEntity entity, params int[] indices)
        {
            var entityIndices = entity.GetComponentIndices();
            var subractedIndices = new List<int>();
            for (var i = 0; i < entityIndices.Length; i++)
            {
                var entityIndex = entityIndices[i];
                var found = false;
                for (var j = 0; j < indices.Length; j++)
                {
                    if (entityIndex == indices[j])
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    subractedIndices.Add(entityIndex);
                }
            }

            var subractedComponents = new IComponent[subractedIndices.Count];
            for (int i = 0; i < subractedIndices.Count; i++)
            {
                var index = subractedIndices[i];
                subractedComponents[i] = entity.GetComponent(index);
            }

            return subractedComponents;
        }

        internal static int[] Indices(this IEntity entity, params IComponent[] components)
        {
            var indices = new List<int>();
            var componentTypes = entity.contextInfo.componentTypes;
            foreach (var component in components)
            {
                indices.Add(Array.IndexOf(componentTypes, component.GetType()));
            }
            return indices.ToArray();
        }

        #endregion Components
    }
} 
#endif