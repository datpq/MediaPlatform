namespace ITF.DataServices.SDK.Models.ViewModels.Btd
{
    public class BtdPlayerViewModel
    {
        public PlayerViewModelBtd PlayerInfo { get; set; }
        public PlayerActivityViewModel PlayerActivityInfo { get; set; }
    }

    public class PlayerViewModelBtd : PlayerViewModelCore
    {
        public string BirthPlace { get; set; }
        public string Residence { get; set; }
        public string Plays { get; set; }
        public string RankSingles { get; set; }
        public string RankDoubles { get; set; }
        public string FirstYearPlayed { get; set; }
        public string TotalNominations { get; set; }
        public string TiesPlayed { get; set; }
        public string WLSingles { get; set; }
        public string WLDoubles { get; set; }
        public string WLTotal { get; set; }
        public string DCCAward { get; set; }
    }
}
