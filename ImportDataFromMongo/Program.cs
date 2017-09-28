using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HorseSpot.DAL.Models;
using System.Drawing;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Dao;
using HorseSpot.DAL;

namespace ImportDataFromMongo
{
    class Program
    {
        static void Main(string[] args)
        {
            var _dao = new HorseAdDaoOld(new MongoDataContext());
            var sqlDao = new HorseAdDao(new HorseSpotDataContext());

            Console.WriteLine("Before getting horse ads");

            var list = _dao.GetAllForAdmin(1);

            Console.WriteLine("Number of horses to import: {0}", list.Count());
            var y = 0;
            foreach (var ad in list)
            {
                Console.WriteLine("Start to import  horse: {0}", y);

                if (ad.IsValidated)
                {
                    Console.WriteLine("Importing Base information for horse: {0}", y);

                    var horseAd = new HorseAd()
                    {
                        HorseName = ad.HorseName,
                        Age = ad.Age,
                        Breed = ad.Breed,
                        DatePosted = ad.DatePosted,
                        Description = ad.Description,
                        Gender = ad.Gender.GenderValue,
                        IsSold = false,
                        IsDeleted = false,
                        IsValidated = ad.IsValidated,
                        IsSponsorized = false,
                        Price = ad.Price,
                        PriceRangeId = ad.PriceRange.Id,
                        Height = ad.Height,
                        HaveCompetionalExperience = ad.HaveCompetionalExperience,
                        HaveXRays = ad.HaveXRays,
                        Address = ad.Address,
                        Title = ad.Title,
                        UserId = ad.UserId,
                        VideoLink = ad.VideoLink,
                        Views = ad.Views,    
                    };

                    Console.WriteLine("Finished base information for horse: {0}", y);

                    var recRider = new List<int>();
                    var ab = new List<int>();

                    Console.WriteLine("Importing abilities for horse: {0}", y);

                    foreach (var ability in ad.Abilities)
                    {
                        ab.Add(ability.Id);
                    }

                    Console.WriteLine("Finish abilities for horse: {0}", y);
                    Console.WriteLine("Importing riders for horse: {0}", y);

                    foreach (var rider in ad.RecomendedRiders)
                    {
                        recRider.Add(rider.Id);
                    }

                    Console.WriteLine("Finish riders for horse: {0}", y);

                    horseAd.RecommendedRiderIds = recRider;
                    horseAd.HorseAbilitesIds = ab;

                    if (ad.Pedigree != null)
                    {
                        Console.WriteLine("Importing pedigree for horse: {0}", y);
                        horseAd.Pedigree = new Pedigree
                        {
                            Father = ad.Pedigree.Father,
                            Father_Father = ad.Pedigree.Father_Father,
                            Father_Father_Father = ad.Pedigree.Father_Father_Father,
                            Father_Father_Mother = ad.Pedigree.Father_Father_Mother,
                            Father_Mother = ad.Pedigree.Father_Mother,
                            Father_Mother_Father = ad.Pedigree.Father_Mother_Father,
                            Father_Mother_Mother = ad.Pedigree.Father_Mother_Mother,
                            Mother = ad.Pedigree.Mother,
                            Mother_Father = ad.Pedigree.Mother_Father,
                            Mother_Father_Father = ad.Pedigree.Mother_Father_Father,
                            Mother_Father_Mother = ad.Pedigree.Mother_Father_Mother,
                            Mother_Mother = ad.Pedigree.Mother_Mother,
                            Mother_Mother_Father = ad.Pedigree.Mother_Mother_Father,
                            Mother_Mother_Mother = ad.Pedigree.Mother_Mother_Mother
                        };

                        Console.WriteLine("Finish pedigree for horse: {0}", y);
                    }
                    var imgList = new List<ImageModel>();
                    
                    foreach (var imageId in ad.ImageIds)
                    {
                        Console.WriteLine("Importing images for horse: {0}", y);
                        var i = _dao.GetImages(imageId);
                        if (i != null)
                        {
                            var imageStream = i.Item1;
                            var imageName = Guid.NewGuid() + ad.HorseName.Replace(" ", "") + ".jpg";
                            var location = "D:\\HORSE_SPOT\\horsespot_back\\HorseSpot.Api\\Images\\HorseAdsImg\\" + imageName;

                            Image.FromStream(imageStream, false).Save(location, System.Drawing.Imaging.ImageFormat.Jpeg);

                            var img = new ImageModel { Name = imageName };
                            imgList.Add(img);
                        }
                        Console.WriteLine("Finish images for horse: {0}", y);
                    }
                    horseAd.Images = imgList;

                    Console.WriteLine("Setting profile pic for horse: {0}", y);
                    horseAd.Images.ToList()[0].IsProfilePic = true;

                    Console.WriteLine("START SAVING: {0}", y);

                    sqlDao.AddHorse(horseAd);

                    Console.WriteLine("FINISHED SAVING: {0}", y);
                    y++;

                    Console.WriteLine("#############");
                    Console.WriteLine("-------------");
                }
            }

            Console.WriteLine("SUCCESS!!!");
            Console.WriteLine("SUCCESS!!!");
            Console.WriteLine("ALL HORSES IMPORTED!!!");
            Console.ReadLine();
        }
    }
}
