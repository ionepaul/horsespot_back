namespace HorseSpot.Models.Models
{
    public class PedigreeDTO
    {
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


        #region GrandGrandGrandParents-FatherLine
        public string Father_Father_Father_Father { get; set; }

        public string Father_Father_Father_Mother { get; set; }

        public string Father_Father_Mother_Father { get; set; }

        public string Father_Father_Mother_Mother { get; set; }

        public string Father_Mother_Father_Father { get; set; }

        public string Father_Mother_Father_Mother { get; set; }

        public string Father_Mother_Mother_Father { get; set; }

        public string Father_Mother_Mother_Mother { get; set; }
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

        #region GrandGrandGrandParents-MotherLine

        public string Mother_Father_Father_Father { get; set; }

        public string Mother_Father_Father_Mother { get; set; }

        public string Mother_Father_Mother_Father { get; set; }

        public string Mother_Father_Mother_Mother { get; set; }

        public string Mother_Mother_Father_Father { get; set; }

        public string Mother_Mother_Father_Mother { get; set; }

        public string Mother_Mother_Mother_Father { get; set; }

        public string Mother_Mother_Mother_Mother { get; set; }

        #endregion

        #endregion
    }
}
