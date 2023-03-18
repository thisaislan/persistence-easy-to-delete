namespace Thisaislan.PersistenceEasyToDelete.PedSerialize.Interfaces
{
    /// <summary>
    /// Interface for custom serializer implementation to use on Ped.
    /// </summary>
    public interface IPedSerializer
    {
        
        public abstract string Serialize(object obj);

        public abstract T Deserialize<T>(string json);
        
    }
}