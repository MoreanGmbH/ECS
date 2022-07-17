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
        public static void AddComponents(this IEntity entity, params IComponent[] components)
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
        public static void AddComponents(this IEntity entity, int[] indices)
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
        public static void ReplaceComponents(this IEntity entity, params IComponent[] components)
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
        /// Subtract components with <paramref name="indices"/> from <paramref name="entities"/>.
        /// </summary>
        public static List<IComponent> SubtractComponents(this IEntity[] entities, params int[] indices)
        {
            var remainderComponents = new List<IComponent>();
            for (int i = 0; i < entities.Length; i++)
            {
                remainderComponents.AddRange(
                    entities[i].SubtractComponents(indices));
            }

            return remainderComponents;
        }

        /// <summary>
        /// Subtract components with <paramref name="indices"/> from <paramref name="entity"/>.
        /// </summary>
        public static IComponent[] SubtractComponents(this IEntity entity, params int[] indices)
        {
            var componentIndices = entity.GetComponentIndices();
            var remainderIndices = new List<int>();
            for (var i = 0; i < componentIndices.Length; i++)
            {
                var entityIndex = componentIndices[i];
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
                    remainderIndices.Add(entityIndex);
                }
            }

            var remainderComponents = new IComponent[remainderIndices.Count];
            for (int i = 0; i < remainderIndices.Count; i++)
            {
                var index = remainderIndices[i];
                remainderComponents[i] = entity.GetComponent(index);
            }

            return remainderComponents;
        }

        public static int[] Indices(this IEntity entity, params IComponent[] components)
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