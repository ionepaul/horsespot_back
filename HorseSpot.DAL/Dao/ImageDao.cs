using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.DAL.Models;
using System.Data.Entity;

namespace HorseSpot.DAL.Dao
{
    public class ImageDao : AbstractDao<ImageModel>, IImageDao
    {
        #region Constructor

        public ImageDao(HorseSpotDataContext dataContext)
            : base(dataContext)
        {
        }

        #endregion

        #region Public Methods
        public void Update(ImageModel image)
        {
            _ctx.Entry(image).State = EntityState.Modified;
            //_ctx.Entry(horseAd).State = EntityState.Modified;

            _ctx.SaveChanges();
        }

        #endregion
    }
}
