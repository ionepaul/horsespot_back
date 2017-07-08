﻿using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;

namespace HorseSpot.DAL.Dao
{
    public class PriceRangeDao : AbstractDao<PriceRange>, IPriceRangeDao
    {
        #region Constuctor
        /// <summary>
        /// PriceRangeDao Constructor
        /// </summary>
        /// <param name="dataContext">HorseSpot Relational Database Context</param>
        public PriceRangeDao(HorseSpotDataContext dataContext)
            : base(dataContext)
        {
        }

        #endregion
    }
}
