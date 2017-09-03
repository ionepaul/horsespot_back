using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;

namespace HorseSpot.DAL.Dao
{
    public class ClientDao : AbstractDao<Client>, IClientDao
    {
        #region Constructor

        /// <summary>
        /// ClientDao Constructor
        /// </summary>
        /// <param name="dataContext">HorseSpot Relational Database Context</param>
        public ClientDao(HorseSpotDataContext dataContext)
            : base(dataContext)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Find application possible client type from database by client id
        /// </summary>
        /// <param name="clientId">Client Id</param>
        /// <returns>Client Model</returns>
        public Client FindClient(string clientId)
        {
            var client = _dbset.Find(clientId);

            return client;
        }

        #endregion
    }
}
