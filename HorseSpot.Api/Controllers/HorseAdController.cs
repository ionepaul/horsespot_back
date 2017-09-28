﻿using HorseSpot.Api.Utils;
using HorseSpot.BLL.Interfaces;
using HorseSpot.Models.Models;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Web;
using System.IO;

namespace HorseSpot.Api.Controllers
{
    public class HorseAdController : ApiController
    {
        private IHorseAdBus _iHorseAdBus;

        public HorseAdController(IHorseAdBus iHorseAdBus)
        {
            _iHorseAdBus = iHorseAdBus;
        }

        [HttpPost]
        [Authorize]
        [Route("api/horses/post")]
        public async Task<string> Post([FromBody] HorseAdDTO horseAdDTO)
        {
            var horseAdId = await _iHorseAdBus.Add(horseAdDTO, UserIdExtractor.GetUserIdFromRequest(Request));
            return horseAdId;
        }

        [HttpPut]
        [Authorize]
        [Route("api/horses/update/{id}")]
        public void Put([FromUri] int id, [FromBody] HorseAdDTO horseAdDTO)
        {
            _iHorseAdBus.Update(id, horseAdDTO, UserIdExtractor.GetUserIdFromRequest(Request));
        }

        [Authorize]
        [HttpDelete]
        [Route("api/horses/delete/{id}/{isSold}")]
        public async Task Delete([FromUri] int id, bool isSold)
        {
            await _iHorseAdBus.Delete(id, UserIdExtractor.GetUserIdFromRequest(Request), isSold);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/horses/validate/{id}")]
        public async Task Validate([FromUri] int id)
        {
            await _iHorseAdBus.Validate(id);
        }

        [HttpPost]
        [Authorize]
        [Route("api/horses/favorite/{id}")]
        public void AddToFavorite([FromUri] int id)
        {
            _iHorseAdBus.AddToFavorite(id, UserIdExtractor.GetUserIdFromRequest(Request));
        }

        /// <summary>
        /// API Interface to increase views on an advertisment
        /// </summary>
        /// <param name="id">Horse advertisment id</param>
        /// <returns>Task</returns>
        [HttpPost]
        [Route("api/horses/views/{id}")]
        public async Task IncreaseViews([FromUri] int id)
        {
            await _iHorseAdBus.IncreaseViews(id);
        }

        /// <summary>
        /// API Interface to get a horse advertisment details
        /// </summary>
        /// <param name="id">Horse Advertisment Id</param>
        /// <returns>Horse Advertisment Model</returns>
        [HttpGet]
        [Route("api/horses/get/{id}")]
        public HorseAdDTO Get([FromUri] int id)
        {
            return _iHorseAdBus.GetById(id);
        }

        /// <summary>
        /// API Interface to get all the unvalidated horses by page number
        /// </summary>
        /// <param name="pageNumber">Number of the page to retrieve</param>
        /// <returns>Model containing total number of unvalidated ads and list of unvalidated ads</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/horses/unvalidated/{pageNumber}")]
        public GetHorseAdListResultsDTO GetAllForAdmin(int pageNumber)
        {
            return _iHorseAdBus.GetAllForAdmin(pageNumber);
        }

        /// <summary>
        /// API Interface to check if user how tries to edit an advertisment is the owner of the post
        /// </summary>
        /// <param name="adId">Horse Advertisment Id</param>
        /// <returns>True/False</returns>
        [HttpGet]
        [Authorize]
        [Route("api/horses/ispostowner/{adId}")]
        public bool CheckPostOwner([FromUri] int adId)
        {
            return _iHorseAdBus.CheckPostOwner(adId, UserIdExtractor.GetUserIdFromRequest(Request));
        }

        /// <summary>
        /// API Interface to search for horses
        /// </summary>
        /// <param name="searchModel">Search Model containing the searching criteria</param>
        /// <returns>Model containing of total number matched and list of horses that were found</returns>
        [HttpGet]
        [Route("api/horses/search")]
        public GetHorseAdListResultsDTO Search(string searchModel = "")
        {
            HorseAdSearchViewModel horseFilter = new HorseAdSearchViewModel();

            JavaScriptSerializer js = new JavaScriptSerializer();
            horseFilter = js.Deserialize<HorseAdSearchViewModel>(searchModel);

            if (horseFilter == null)
            {
                horseFilter = new HorseAdSearchViewModel();
            }

            return _iHorseAdBus.SearchHorses(horseFilter);
        }

        [HttpPost]
        [Authorize]
        [Route("api/horses/images/save/{adId}/{imageName}")]
        public async Task SaveNewImage([FromUri] int adId, [FromUri] string imageName)
        {
            await _iHorseAdBus.SaveNewImage(adId, imageName, UserIdExtractor.GetUserIdFromRequest(Request));
        }

        [HttpPost]
        [Authorize]
        [Route("api/horses/images/delete/{imageId}")]
        public void Delete([FromUri] int imageId)
        {
            var imageName = _iHorseAdBus.DeleteImage(imageId, UserIdExtractor.GetUserIdFromRequest(Request));

            var horseAdvImageDir = ConfigurationManager.AppSettings["HorseAdsImgDirectory"];
            var serverPath = HttpContext.Current.Server.MapPath(horseAdvImageDir);

            if (Directory.Exists(Path.GetDirectoryName(serverPath)))
            {
                var path = Path.Combine(serverPath, imageName);
                File.Delete(path);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/horses/images/profilepic/{imageId}")]
        public void SetAsAdProfilePicture([FromUri] int imageId)
        {
            _iHorseAdBus.SetHorseAdProfilePicture(imageId, UserIdExtractor.GetUserIdFromRequest(Request));
        }
    }
}
