namespace Thisaislan.PersistenceEasyToDeleteInEditor.Interfaces
{
    /// <summary>
    /// Interface for custom serializer implementation to use on Pede.
    /// </summary>
    public interface ISerializer
    {
        
        public abstract string Serialize(object obj);

        public abstract T Deserialize<T>(string json);
        
    }
}