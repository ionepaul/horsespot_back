using HorseSpot.DAL.Entities;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Converters
{
    /// <summary>
    /// Static class used to map the gender database model to domain trasfer objects and vice-versa
    /// </summary>
    public class GenderConverter
    {
        /// <summary>
        /// Converts Gender database model to GenderDTO
        /// </summary>
        /// <param name="gender">Gender Model</param>
        /// <returns>GenderDTO</returns>
        public static GenderDTO FromGenderToGenderDTO(Gender gender)
        {
            if (gender == null)
            {
                return null;
            }

            var genderDTO = new GenderDTO
            {
                Id = gender.Id,
                Gender = gender.GenderValue
            };

            return genderDTO;
        }

        /// <summary>
        /// Converts GenderDTO to Gender database model
        /// </summary>
        /// <param name="gender">GenderDTO Model</param>
        /// <returns>Gender Database Model</returns>
        public static Gender FromGenderDTOToGender(GenderDTO genderDTO)
        {
            if (genderDTO == null)
            {
                return null;
            }

            var gender = new Gender
            {
                Id = genderDTO.Id,
                GenderValue = genderDTO.Gender
            };

            return gender;
        }
    }
}
