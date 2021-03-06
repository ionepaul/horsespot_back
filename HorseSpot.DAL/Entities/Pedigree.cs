﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HorseSpot.DAL.Entities
{
    public class Pedigree
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PedigreeId { get; set; }

        #region FatherLine

        public string Father { get; set; }

        #region GrandParents-FatherLine

        public string Father_Father { get; set; }

        public string Father_Mother { get; set; }

        #endregion

        #region GrandGrandParents-FatherLine
        public string Father_Father_Father { get; set; }

        public string Father_Father_Mother { get; set; }

        public string Father_Mother_Father { get; set; }

        public string Father_Mother_Mother { get; set; }
        #endregion
        
        #endregion

        #region Mother

        public string Mother { get; set; }

        #region GrandParents-MotherLine

        public string Mother_Father { get; set; }

        public string Mother_Mother { get; set; }

        #endregion

        #region GrandGrandParents-MotherLine

        public string Mother_Father_Father { get; set; }

        public string Mother_Father_Mother { get; set; }

        public string Mother_Mother_Father { get; set; }

        public string Mother_Mother_Mother { get; set; }

        #endregion

        #endregion
    }
}