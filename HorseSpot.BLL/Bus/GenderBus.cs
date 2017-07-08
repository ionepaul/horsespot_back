using HorseSpot.BLL.Converters;
using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.Models.Models;
using System.Collections.Generic;
using System.Linq;

namespace HorseSpot.BLL.Bus
{
    public class GenderBus : IGenderBus
    {
        #region Local Variables

        private IGenderDao _iGenderDao;

        #endregion

        #region Constructor
        /// <summary>
        /// GenderBus Constructor
        /// </summary>
        /// <param name="iGenderDao">GenderDao Interface</param>
        public GenderBus(IGenderDao iGenderDao)
        {
            _iGenderDao = iGenderDao;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Gets all genders
        /// </summary>
        /// <returns>List of GenderDTO Model</returns>
        public IEnumerable<GenderDTO> GetAll()
        {
            IEnumerable<Gender> allGenders = _iGenderDao.GetAll();

            return allGenders.Select(GenderConverter.FromGenderToGenderDTO);
        }

        #endregion
    }
}
