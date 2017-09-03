using HorseSpot.DAL.Entities;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Converters
{
    /// <summary>
    /// Static class used to map the horse ability database model to domain trasfer objects and vice-versa
    /// </summary>
    public class HorseAbilityConverter
    {
        /// <summary>
        /// Converts Horse ability database model to HorseAbilityDTO
        /// </summary>
        /// <param name="horseAbility">HorseAbility database Model</param>
        /// <returns>HorseAbilityDTO</returns>
        public static HorseAbilityDTO FromHorseAbilityToHorseAbilityDTO(HorseAbility horseAbility)
        {
            
            if (horseAbility == null)
            {
                return null;
            }

            var horseAbilityDTO = new HorseAbilityDTO
            {
                Id = horseAbility.Id,
                Ability = horseAbility.Ability
            };

            return horseAbilityDTO;
        }

        /// <summary>
        /// Converts HorseAbilityDTO to Horse ability database model
        /// </summary>
        /// <param name="horseAbility">HorseAbilityDTO</param>
        /// <returns>Horse ability database model</returns>
        public static HorseAbility FromHorseAbilityDTOToHorseAbility(HorseAbilityDTO abilityDTO)
        {
            if (abilityDTO == null)
            {
                return null;
            }

            var horseAbility = new HorseAbility
            {
                Id = abilityDTO.Id,
                Ability = abilityDTO.Ability
            };

            return horseAbility;
        }
    }
}
