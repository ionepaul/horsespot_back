using HorseSpot.DAL.Entities;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Converters
{
    /// <summary>
    /// Static class used to map the price range database model to domain trasfer objects and vice-versa
    /// </summary>
    public class PriceRangeConverter
    {
        /// <summary>
        /// Converts PriceRange database model to PriceRangeDTO
        /// </summary>
        /// <param name="priceRange">PriceRange database model</param>
        /// <returns>PriceRangeDTO</returns>
        public static PriceRangeDTO FromPriceRangeToPriceRangeDTO(PriceRange priceRange)
        {
            if (priceRange == null)
            {
                return null;
            }

            var priceRangeDTO = new PriceRangeDTO
            {
                Id = priceRange.Id,
                PriceRangeValue = priceRange.PriceRangeValue
            };

            return priceRangeDTO;
        }

        /// <summary>
        /// Converts PriceRangeDTO to PriceRange database model
        /// </summary>
        /// <param name="priceRange">PriceRangeDTO</param>
        /// <returns>PriceRange database model</returns>
        public static PriceRange FromPriceRangeDTOToPriceRange(PriceRangeDTO priceRangeDTO)
        {
            if (priceRangeDTO == null)
            {
                return null;
            }

            var priceRange = new PriceRange
            {
                Id = priceRangeDTO.Id,
                PriceRangeValue = priceRangeDTO.PriceRangeValue
            };

            return priceRange;
        }
    }
}
