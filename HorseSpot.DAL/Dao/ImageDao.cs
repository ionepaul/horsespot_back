using System.Data.Entity;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;

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
            _ctx.SaveChanges();
        }

        #endregion
    }
}
