namespace Morean.ECS
{
    /// <summary>
    /// Structure to be used when serializing / deserializing an entity to / from json.
    /// </summary>
    internal struct SerializedContext
    {
        /// <summary>
        /// Entity's context name, to be matched to the actual <see cref="Entitas.IContext"/>.
        /// </summary>
        public string Context;

        /// <summary>
        /// Array of entity's components.
        /// </summary>
        public Entitas.IComponent[][] Entities;
    }
}
