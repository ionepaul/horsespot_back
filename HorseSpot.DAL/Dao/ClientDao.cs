using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;

namespace HorseSpot.DAL.Dao
{
    public class ClientDao : AbstractDao<Client>, IClientDao
    {
        #region Constructor

        public ClientDao(HorseSpotDataContext dataContext)
            : base(dataContext)
        {
        }

        #endregion

        #region Public Methods

        public Client FindClient(string clientId)
        {
            var client = _dbset.Find(clientId);

            return client;
        }

        #endregion
    }
}
