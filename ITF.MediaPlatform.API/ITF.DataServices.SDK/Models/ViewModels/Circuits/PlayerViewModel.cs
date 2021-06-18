using TypeLite;

namespace ITF.DataServices.SDK.Models.ViewModels.Circuits
{
    [TsClass]
    public class PlayerViewModel
    {
        public string Gender { get; set; }
        public string PlayerFamilyName { get; set; }
        public string PlayerGivenName { get; set; }
        public int PlayerId { get; set; }
        public string PlayerNationalityCode { get; set; }
        public string PlayerNationalityName { get; set; }
    }

    [TsClass]
    public class PlayersViewModel : PlayerViewModel
    {
        public string Doubles { get; set; }
        public string MixedDoubles { get; set; }
        public string QuadDoubles { get; set; }
        public string QuadSingles { get; set; }
        public string Singles { get; set; }
        public short? Year { get; set; }
    }
}
