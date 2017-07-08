using HorseSpot.DAL.Entities;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Converters
{
    /// <summary>
    /// Static class used to map the recommended rider database model to domain trasfer objects and vice-versa
    /// </summary>
    public static class RecommendedRiderConverter
    {
        /// <summary>
        /// Converts RecommendedRider database model to RecommendedRiderDTO
        /// </summary>
        /// <param name="rider">Recommended Rider database model</param>
        /// <returns>RecommendedRiderDTO</returns>
        public static RecommendedRiderDTO FromRiderToRiderDTO(RecommendedRider rider)
        {
            return new RecommendedRiderDTO
            {
                Id = rider.Id,
                Rider = rider.Rider
            };
        }

        /// <summary>
        /// Converts RecommendedRiderDTO to RecommendedRider database model
        /// </summary>
        /// <param name="riderDTO">RecommendedRiderDTO</param>
        /// <returns>Recommended Rider database model</returns>
        public static RecommendedRider FromRiderDTOToRider(RecommendedRiderDTO riderDTO)
        {
            return new RecommendedRider
            {
                Id = riderDTO.Id,
                Rider = riderDTO.Rider
            };
        }
    }
}
