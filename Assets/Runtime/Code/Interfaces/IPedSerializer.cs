namespace Thisaislan.PersistenceEasyToDelete.Interfaces
{
    /// <summary>
    /// Interface for custom serializer implementation to use on Ped.
    /// </summary>
    public interface IPedSerializer
    {
        string Serialize(object obj);

        T Deserialize<T>(string json);

    }
}