namespace Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize.Interfaces
{
    /// <summary>
    /// Interface for custom serializer implementation to use on Pede.
    /// </summary>
    public interface IPedeSerializer
    {
        
        public abstract string Serialize(object obj);

        public abstract T Deserialize<T>(string json);
        
    }
}