#if ECS
namespace ECS
{
    /// <summary>
    /// To make an <see cref="Entitas.IComponent"/> serializable 
    /// via <see cref="Entity.Serialize(Entitas.Entity, Newtonsoft.Json.Formatting)"/>.
    /// </summary>
    public class SerializableAttribute : System.Attribute { }
}

#endif