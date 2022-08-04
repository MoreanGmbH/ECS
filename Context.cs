#if ECS
using Entitas;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
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

        /// <summary>
        /// Find context by name.
        /// </summary>
        public static IContext GetContext(this ContextData data) => GetContext(data.Context);

        /// <summary>
        /// Find context by name.
        /// </summary>
        public static IContext GetContext(string context)
            => Array.Find(Contexts.sharedInstance.allContexts, match => match.ToString() == context);

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

        private static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        /// <summary>
        /// Deserialize contexts from Json or Bson data and create their entities.
        /// </summary>
        /// <param name="json">From Json if True, from Bson otherwise.</param>
        public static void LoadEntities(string data, bool json = true)
        {
            var contextsData = json ? DeserializeContexsFromJson(data)
                : DeserializeContexsFromJson(data);

            foreach (var contextData in contextsData)
            {
                contextData.GetContext().CreateEntities(contextData.Entities);
            }
        }

        /// <summary>
        /// Serialize all entities in <paramref name="contexts"/> to Json.
        /// </summary>
        /// <param name="formatting">Json formatting.</param>
        /// <param name="json">To Json if True, to Bson otherwise.</param>
        public static string SerializeContexts(Formatting formatting = Formatting.None, bool json = true, params IContext[] contexts)
        {
            var serializedContexts = new ContextData[contexts.Length];
            for (int i = 0; i < serializedContexts.Length; i++)
            {
                var context = contexts[i];
                var serializedContext = new ContextData();
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

            return json ? SerializeContextsDataToJson(formatting, serializedContexts)
                : SerializeContextsDataToBson(serializedContexts);
        }

        /// <summary>
        /// Serialize all entities in <paramref name="contextsData"/> to Json.
        /// </summary>
        /// <param name="formatting">Json formatting.</param>
        public static string SerializeContextsDataToJson(Formatting formatting = Formatting.None, params ContextData[] contextsData)
            => JsonConvert.SerializeObject(contextsData, formatting, serializerSettings);

        /// <summary>
        /// Serialize all entities in <paramref name="contextsData"/> to Bson.
        /// </summary>
        public static string SerializeContextsDataToBson(params ContextData[] contextsData)
        {
            var memoryStream = new System.IO.MemoryStream();
            using (var writer = new BsonWriter(memoryStream))
            {
                new JsonSerializer().Serialize(writer, contextsData);
            }
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        /// <summary>
        /// Deserialize <see cref="ContextData"/> from Json data.
        /// </summary>
        public static ContextData[] DeserializeContexsFromJson(string data)
        => JsonConvert.DeserializeObject<ContextData[]>(data, serializerSettings);

        /// <summary>
        /// Deserialize <see cref="ContextData"/> from Bson data.
        /// </summary>
        public static ContextData[] DeserializeContexsFromBson(string data)
        {
            var byteData = Convert.FromBase64String(data);
            var memoryStream = new System.IO.MemoryStream(byteData);
            using (var reader = new BsonReader(memoryStream))
            {
                reader.ReadRootValueAsArray = true;
                return new JsonSerializer().Deserialize<ContextData[]>(reader);
            }
        }

        #endregion Serialization / Deserialization
    }
}
#endif