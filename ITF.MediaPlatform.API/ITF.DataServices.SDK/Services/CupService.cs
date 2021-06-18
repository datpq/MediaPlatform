using System;
using System.Collections.Generic;
using System.Linq;
using ITF.DataServices.Authentication.Services;
using ITF.DataServices.SDK.Data;

namespace ITF.DataServices.SDK.Services
{
    public abstract class CupService : BaseService
    {
        protected readonly ICupDataRepository DavisCupRepo;
        protected readonly ICupDataRepository FedCupRepo;
        protected readonly ISameStructureDataRepository CmsRepo;

        public const string StartDateFormat = "dd-MM";
        public const string EndDateFormat = "dd-MM-yyyy";

        public const string AssetTypeMatchReport = "MatchReport"; //21
        public const string AssetTypePhotographer = "Photographer"; //17
        public const string AssetTypeGallery = "Gallery"; //12
        public const string AssetTypeTie = "Tie"; //33
        public const string AssetTypeImage = "Image"; //3
        public const string AssetTypeNation = "Nation"; //27

        public const string Srb = "SRB"; //Serbia
        public const int SrbYear = 1995; //Serbia Start Year
        public const int NonSrbYearWG = 1980;
        public const string RcW = "W"; //ResultCode Win
        public const string RcL = "L"; //ResultCode Loss
        public const string RcN = "N"; //ResultCode Nil
        public const string ScC = "C"; //SurfaceCode Clay
        public const string ScH = "H"; //SurfaceCode Hard
        public const string ScG = "G"; //SurfaceCode Grass
        public const string ScA = "A"; //SurfaceCode Carpet
        public const string IfI = "I"; //IndoorOutdoorFlag Indoor
        public const string IfO = "O"; //IndoorOutdoorFlag Outdoor
        public const string PsPc = "PC"; //PlayStatusCode: Play Completed
        public const string PsNp = "NP"; //PlayStatusCode: Not Played
        public const string PsIp = "IP"; //PlayStatusCode: In Progress
        public const string DcK = "KO"; //Knock-out
        public const string DcR = "RR"; //Round Robin
        public const string Na = "N/A"; //Not Available
        public const string DrcF = "FR"; // Final
        public const string RscW = "WO"; // Walkover

        public static readonly string[] PcPlayeds = {PsPc, "PA", "PU"};
        public static readonly string[] EdcWg = {"WG", "WG1", "WG2"};
        public static readonly string[] EdcG = { "G1", "G2" };
        public static readonly List<string> EdcOrders = new List<string> {"WG", "WG2", "G1", "G2", "G3" };
        public static readonly List<string> EccOrders = new List<string> {"M", "PO", "PPO", "PP3-4", "PP5-8", "PP9-12", "REL"};
        public static readonly List<string> EccOrdersRoundRobin =
            new List<string> {"PPO", "PP1-2", "PP1-4", "PPO", "PP5-6", "PP5-8", "PP6-7", "PP7-8", "PP9-12", "PP9-10", "PP11-12", "PP13-16", "PP13-14", "PP15-16"};
        public static readonly string[] McT = {McS, McD};

        protected static readonly int[] ExcludingTieIds = {
            10001093, 10000711, 10000712, 10000713, 10003705, 10003706, 10003707, 10003708,
            10003709, 10002520, 10000597, 10003456, 10000640, 10000578, 10005081
        };

        protected static IEnumerable<string> GetAllCultures()
        {
            return Enum.GetValues(typeof(Language)).Cast<Language>().Select(x => x.ToString().ToLower());
        }

        protected CupService(
            ICupDataRepository davisCupRepo = null,
            ICupDataRepository fedCupRepo = null,
            ISameStructureDataRepository cmsRepo = null,
            ICacheConfigurationService cacheConfigurationService = null) : base(cacheConfigurationService)
        {
            DavisCupRepo = davisCupRepo;
            FedCupRepo = fedCupRepo;
            CmsRepo = cmsRepo;
        }
    }
}
