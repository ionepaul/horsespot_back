using HorseSpot.DAL.Entities;

namespace HorseSpot.DAL.Interfaces
{
    public interface IClientDao : IDao<Client>
    {
        Client FindClient(string clientId);
    }
}
