using HorseSpot.BLL.Converters;
using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.Models.Models;
using System.Collections.Generic;
using System.Linq;

namespace HorseSpot.BLL.Bus
{
    public class HorseAbilityBus : IHorseAbilityBus
    {
        #region Local Variables

        private IHorseAbilityDao _iHorseAbilityDao;

        #endregion

        #region Constructor
        /// <summary>
        /// HorseAbilityBus Constructor
        /// </summary>
        /// <param name="iHorseAbilityDao">HorseAbilityDao Interaface</param>
        public HorseAbilityBus(IHorseAbilityDao iHorseAbilityDao)
        {
            _iHorseAbilityDao = iHorseAbilityDao;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Get all horse abilites
        /// </summary>
        /// <returns>List of HorseAbilityDTO</returns>
        public IEnumerable<HorseAbilityDTO> GetAll()
        {
            IEnumerable<HorseAbility> allHorseAbilities = _iHorseAbilityDao.GetAll();

            return allHorseAbilities.Select(HorseAbilityConverter.FromHorseAbilityToHorseAbilityDTO);
        }

        #endregion
    }
}
