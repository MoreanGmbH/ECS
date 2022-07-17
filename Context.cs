#if ECS
using Entitas;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ECS
{
    /// <summary>
    /// Provides <see cref="IContexts"/> related utilities such as creating entities
    /// and serializing and deserializing contexts with entities and components.
    /// </summary>
    public static class Context
    {
        #region Context

        private static string contextCreateEntityMethod = "CreateEntity";
        private static string contextGetEntitiesMethod = "GetEntities";

        /// <summary>
        /// Create <see cref="IEntity"/> in <paramref name="context"/>.
        /// </summary>
        public static IEntity CreateEntity(this IContext context)
            => (IEntity)context.GetType().GetMethod(contextCreateEntityMethod).Invoke(context, null);

        /// <summary>
        /// Get <see cref="IEntity"/> colleciton in <paramref name="context"/>.
        /// </summary>
        public static IEntity[] GetEntities(this IContext context)
            => (IEntity[])context.GetType().GetMethod(contextGetEntitiesMethod).Invoke(context, null);

        #endregion Context

        #region Entity

        /// <summary>
        /// Create <see cref="IEntity"/> collection in <paramref name="context"/>.
        /// </summary>
        public static void CreateEntities(this IContext context, params IComponent[][] entities)
        {
            var createdEntities = context.CreateEntities(entities.Length);
            for (int i = 0; i < createdEntities.Length; i++)
            {
                createdEntities[i].AddComponents(entities[i]);
            }
        }

        /// <summary>
        /// Create <see cref="IEntity"/> in <paramref name="context"/>
        /// and add <paramref name="components"/>.
        /// </summary>
        public static void CreateEntity(this IContext context, params IComponent[] components)
            => context.CreateEntity().AddComponents(components);

        /// <summary>
        /// Create <see cref="IEntity"/> in <paramref name="context"/>
        /// and add <see cref="IComponent"/>s from <paramref name="componentIndices"/>.
        /// </summary>
        public static void CreateEntity(this IContext context, params int[] componentIndices)
            => context.CreateEntity().AddComponents(componentIndices);

        /// <summary>
        /// Create <see cref="IEntity"/> collection in <paramref name="context"/>.
        /// </summary>
        public static IEntity[] CreateEntities(this IContext context, int count)
        {
            var entities = new IEntity[count];
            var creationMethod = context.GetType().GetMethod(contextCreateEntityMethod);
            for (int i = 0; i < count; i++)
            {
                entities[i] = (IEntity)creationMethod.Invoke(context, null);
            }
            return entities;
        }

        #endregion Entity

        #region Serialization / Deserialization

        /// <summary>
        /// Serialize all entities in <paramref name="contexts"/> to Json.
        /// </summary>
        /// <param name="formatting">Json formatting.</param>
        public static string Serialize(Formatting formatting = Formatting.None, params IContext[] contexts)
        {
            var serializedContexts = new SerializedContext[contexts.Length];
            for (int i = 0; i < serializedContexts.Length; i++)
            {
                var context = contexts[i];
                var serializedContext = new SerializedContext();
                serializedContext.Context = context.contextInfo.name;

                var entities = context.GetEntities();
                var serializedEntities = new List<IComponent[]>();

                for (int j = 0; j < entities.Length; j++)
                {
                    var components = new List<IComponent>();
                    foreach (var component in entities[j].GetComponents())
                    {
                        if (component != null && component.GetType().IsDefined(typeof(SerializableAttribute), false))
                            components.Add(component);
                    }

                    if (components.Count > 0)
                    {
                        serializedEntities.Add(components.ToArray());
                    }
                }

                serializedContext.Entities = serializedEntities.ToArray();
                serializedContexts[i] = serializedContext;
            }

            return JsonConvert.SerializeObject(serializedContexts, formatting,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
        }

        /// <summary>
        /// Deserialize and create entities from Json <paramref name="data"/>.
        /// </summary>
        public static void Deserialize(string data)
        {
            var serializedContexts = JsonConvert.DeserializeObject<SerializedContext[]>(data,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

            for (int i = 0; i < serializedContexts.Length; i++)
            {
                var serializedContext = serializedContexts[i];

                Array.Find(Contexts.sharedInstance.allContexts, c => c.ToString() == serializedContext.Context)
                    .CreateEntities(serializedContext.Entities);
            }
        }

        #endregion Serialization / Deserialization
    }
} 
#endif