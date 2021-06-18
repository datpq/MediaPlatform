using System.Web.Http.Results;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ITF.DataServices.SDK;
using ITF.DataServices.SDK.Models.ViewModels;
using ITF.MediaPlatform.API.ViewModels;
using Newtonsoft.Json;
using Xunit;

namespace ITF.MediaPlatform.API.Tests
{
    public class EventTest : BaseTest
    {
        public EventTest(IisExpressFixture iisExpressFixture) : base(iisExpressFixture)
        {
        }

        [Theory]
        [InlineData(1, null, null, null, null, null)]
        [InlineData(2014, "WG", "M", null, null, Constants.JsDs2014WgM)]
        [InlineData(2015, "G1", "AM", null, null, Constants.JsDs2015G1Am)]
        public void TestDrawSheet(int year, string section, string subSection, string language, string source, string expectedJson)
        {
            var actionResult = EventController.GetDrawSheet(year, section, subSection, language, source, false);
            var actionName = ((ActionNameAttribute)EventController.GetType().GetMethod("GetDrawSheet").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<DrawSheetViewModel>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<DrawSheetViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            expectedResult.Events.SelectMany(x => x.Rounds.SelectMany(y => y.Ties)).ToList().ForEach(x =>
            {
                x.Side1NationName = null;
                x.Side1NationNameES = null;
                x.Side2NationName = null;
                x.Side2NationNameES = null;
                x.Side1H2HWin = 0;
                x.Side2H2HWin = 0;
            });
            var duplicateResult = result.CloneJson();
            duplicateResult.Events.SelectMany(x => x.Rounds.SelectMany(y => y.Ties)).ToList().ForEach(x =>
            {
                x.Side1NationName = null;
                x.Side1NationNameES = null;
                x.Side2NationName = null;
                x.Side2NationNameES = null;
                x.Side1H2HWin = 0;
                x.Side2H2HWin = 0;
            });
            Assert.Equal(JsonConvert.SerializeObject(duplicateResult), JsonConvert.SerializeObject(expectedResult));

            //Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(expectedResult));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, year.ToString(), section, subSection);
        }

        [Theory]
        [InlineData(1, null, null, null, null)]
        [InlineData(2010, "G2", "AM", null, null)]
        public void TestGetResultsByYear(int year, string divisionCode, string zoneCode, string language, string source)
        {
            var actionResult = EventController.GetResultsByYear(year, language, source, false);
            var actionName = ((ActionNameAttribute)EventController.GetType().GetMethod("GetResultsByYear").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (divisionCode == null || zoneCode == null)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<ResultsByYearViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            Assert.NotEmpty(result.Events);
            Assert.NotEmpty(result.Events.SelectMany(x => x.Rounds));
            Assert.NotEmpty(result.Events.SelectMany(x => x.Rounds).SelectMany(x => x.Ties));
            Assert.NotEmpty(result.Events.SelectMany(x => x.Rounds).SelectMany(x => x.Ties).SelectMany(x => x.Matches));
            Assert.NotEmpty(result.Events.SelectMany(x => x.Rounds).SelectMany(x => x.Ties).SelectMany(x => x.Teams));

            //Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(expectedResult));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, year.ToString());
        }

        [Theory]
        [InlineData(1, null, null, null)]
        [InlineData(2015, null, null, Constants.JsRbyl2015)]
        public void TestResultsByYearLite(int year, string language, string source, string expectedJson)
        {
            var actionResult = EventController.GetResultsByYearLite(year, null, null, language, source, false);
            var actionName = ((ActionNameAttribute)EventController.GetType().GetMethod("GetResultsByYearLite").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<DrawSheetViewModel>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<DrawSheetViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            expectedResult.Events.SelectMany(x => x.Rounds.SelectMany(y => y.Ties)).ToList().ForEach(x =>
            {
                x.Side1NationName = null;
                x.Side1NationNameES = null;
                x.Side2NationName = null;
                x.Side2NationNameES = null;
                x.Side1H2HWin = 0;
                x.Side2H2HWin = 0;
            });
            var duplicateResult = result.CloneJson();
            duplicateResult.Events.SelectMany(x => x.Rounds.SelectMany(y => y.Ties)).ToList().ForEach(x =>
            {
                x.Side1NationName = null;
                x.Side1NationNameES = null;
                x.Side2NationName = null;
                x.Side2NationNameES = null;
                x.Side1H2HWin = 0;
                x.Side2H2HWin = 0;
            });
            Assert.Equal(JsonConvert.SerializeObject(expectedResult), JsonConvert.SerializeObject(duplicateResult));

            //Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(expectedResult));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, year.ToString());
        }

        [Theory]
        [InlineData(1, null, null, null, null, null)]
        [InlineData(2015, "G3", "EUR", null, null, Constants.JsRr2015G3Eur)]
        public void TestRoundRobinEvents(int year, string section, string subSection, string language, string source, string expectedJson)
        {
            var actionResult = EventController.GetRoundRobinEvents(year, section, subSection, language, source, false);
            var actionName = ((ActionNameAttribute)EventController.GetType().GetMethod("GetRoundRobinEvents").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<ICollection<RoundRobinEventViewModel>>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<RoundRobinEventViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(expectedResult));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, year.ToString(), section, subSection);
        }

        [Theory]
        [InlineData(1, null, null, null, null, null)]
        [InlineData(2015, "G3", "EUR", null, null, Constants.JsTm2015G3Eur)]
        public void TestTournaments(int year, string section, string subSection, string language, string source, string expectedJson)
        {
            var actionResult = EventController.GetTournaments(year, section, subSection, language, source, false);
            var actionName = ((ActionNameAttribute)EventController.GetType().GetMethod("GetTournaments").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<ICollection<TournamentViewModel>>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<TournamentViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(expectedResult));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, year.ToString(), section, subSection);
        }

        [Theory]
        [InlineData(1, null, null, null, null, null, null)]
        [InlineData(2015, "G3", "EUR", null, null, Constants.JsRr2015G3Eur, Constants.JsTm2015G3Eur)]
        public void TestRoundRobinEventsApp(int year, string section, string subSection, string language, string source, string expectedRrJson, string expectedTmJson)
        {
            var actionResult = BtdController.GetRoundRobinEvents(year, section, subSection, language, source, false);
            var actionName = ((ActionNameAttribute)BtdController.GetType().GetMethod("GetRoundRobinEvents").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;
            var routePrefix = ((RoutePrefixAttribute)BtdController.GetType().GetCustomAttributes(typeof(RoutePrefixAttribute), true).FirstOrDefault())?.Prefix;
            if (routePrefix != null)
            {
                actionName = $"{routePrefix}/{actionName}";
            }

            if (expectedRrJson == null || expectedTmJson == null)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedTmResult = JsonConvert.DeserializeObject<ICollection<BtdTournamentViewModel>>(expectedTmJson);
            var expectedRrResult = JsonConvert.DeserializeObject<ICollection<RoundRobinEventViewModel>>(expectedRrJson);

            var contentResult = actionResult as OkNegotiatedContentResult<BtdRoundRobinEventAppViewModel>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.Equal(JsonConvert.SerializeObject(result.Tournaments), JsonConvert.SerializeObject(expectedTmResult));
            Assert.Equal(JsonConvert.SerializeObject(result.Events), JsonConvert.SerializeObject(expectedRrResult));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, year.ToString(), section, subSection);
        }

        [Theory]
        [InlineData(1, null, null, null, null, null, null, 0)]
        [InlineData(2015, "G3", "EUR", null, null, "ALB", "Nallbani", 4)]
        public void TestRoundRobinNominations(int year, string section, string subSection, string language, string source,
            string expectedNationCode, string expectedCaptainFamilyName, int expectedPlayers)
        {
            var actionResult = EventController.GetRoundRobinNominations(year, section, subSection, language, source, false);
            var actionName = ((ActionNameAttribute)EventController.GetType().GetMethod("GetRoundRobinNominations").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedNationCode == null)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<RoundRobinNominationViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            var expectedNation = result.SingleOrDefault(x => x.NationCode == expectedNationCode);
            Assert.NotNull(expectedNation);
            Assert.Equal(expectedNation.CaptainFamilyName, expectedCaptainFamilyName);
            Assert.Equal(expectedNation.Players.Count, expectedPlayers);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, year.ToString(), section, subSection);
        }

        [Theory]
        [InlineData("INV", "INV", null, null, 0)]
        [InlineData("WG", "M", null, null, 37)]
        [InlineData("G1", "AM", null, null, 24)]
        [InlineData("G2", "AO", null, "fc", 3)]
        public void TestEventYears(string section, string subSection, string language, string source, int expectedCount)
        {
            var actionResult = EventController.GetEventYears(section, subSection, language, source, false);
            var actionName = ((ActionNameAttribute)EventController.GetType().GetMethod("GetEventYears").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<EventYearViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.True(result.Count >= expectedCount);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, section, subSection);
        }

        [Theory]
        [InlineData(244546, null, null, "M-DC-2016-G1-AM-REL-BAR-DOM-01")]
        public void TestNodeRelatedTieDetails(int nodeId, string language, string source, string expectedTieId)
        {
            var actionResult = EventController.GetNodeRelatedTieDetails(nodeId, language, source, false);
            var actionName = ((ActionNameAttribute)EventController.GetType().GetMethod("GetNodeRelatedTieDetails").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<TieDetailsWebViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains(result, x => x.TieId == expectedTieId);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nodeId.ToString());
        }

        [Theory]
        [InlineData("INV", null, null, null)]
        [InlineData("M-DC-2015-WG-M-GBR-BEL-01", null, null, Constants.JsTdwBelGbr2015)]
        [InlineData("W-FC-2010-WG-M-ITA-USA-01", null, "fc", Constants.JsTdwFcUsaIta2010)]
        public void TestTieDetailsWeb(string publicTieId, string language, string source, string expectedJson)
        {
            var actionResult = EventController.GetTieDetailsWeb(publicTieId, language, source, false);
            var actionName = ((ActionNameAttribute)EventController.GetType().GetMethod("GetTieDetailsWeb").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<TieDetailsWebViewModel>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<TieDetailsWebViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            Assert.True(result.Side1NationH2HWinCount >= expectedResult.Side1NationH2HWinCount);
            Assert.True(result.Side2NationH2HWinCount >= expectedResult.Side2NationH2HWinCount);

            var duplicateResult = result.CloneJson();
            duplicateResult.Side1NationH2HWinCount = 0;
            duplicateResult.Side2NationH2HWinCount = 0;
            var expectedDuplicateResult = expectedResult.CloneJson();
            expectedDuplicateResult.Side1NationH2HWinCount = 0;
            expectedDuplicateResult.Side2NationH2HWinCount = 0;
            Assert.Equal(JsonConvert.SerializeObject(duplicateResult), JsonConvert.SerializeObject(expectedDuplicateResult));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, publicTieId);
        }

        [Theory]
        [InlineData("INV", null, null, null)]
        [InlineData("M-DC-2015-WG-M-GBR-BEL-01", null, null, Constants.JsTdwBelGbr2015)]
        [InlineData("W-FC-2010-WG-M-ITA-USA-01", null, "fc", Constants.JsTdwFcUsaIta2010)]
        public void TestTieDetailsApp(string publicTieId, string language, string source, string expectedJson)
        {
            var actionResult = BtdController.GetTieDetails(publicTieId, language, source, false);
            var actionName = ((ActionNameAttribute)BtdController.GetType().GetMethod("GetTieDetails").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;
            var routePrefix = ((RoutePrefixAttribute)BtdController.GetType().GetCustomAttributes(typeof(RoutePrefixAttribute), true).FirstOrDefault())?.Prefix;
            if (routePrefix != null)
            {
                actionName = $"{routePrefix}/{actionName}";
            }

            if (expectedJson == null)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<TieDetailsAppViewModel>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<TieDetailsAppViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            Assert.True(result.Side1NationH2HWinCount >= expectedResult.Side1NationH2HWinCount);
            Assert.True(result.Side2NationH2HWinCount >= expectedResult.Side2NationH2HWinCount);

            Assert.NotNull(result.Report);
            Assert.True(result.Results.Any());
            Assert.True(result.Nominations.Any());

            var duplicateResult = result.CloneJson();
            duplicateResult.Side1NationH2HWinCount = 0;
            duplicateResult.Side2NationH2HWinCount = 0;
            duplicateResult.Report = null;
            duplicateResult.Results = null;
            duplicateResult.Nominations = null;
            var expectedDuplicateResult = expectedResult.CloneJson();
            expectedDuplicateResult.Side1NationH2HWinCount = 0;
            expectedDuplicateResult.Side2NationH2HWinCount = 0;
            Assert.Equal(JsonConvert.SerializeObject(duplicateResult), JsonConvert.SerializeObject(expectedDuplicateResult));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, publicTieId);
        }

        [Theory]
        [InlineData(1, null, null, null)]
        [InlineData(100029624, null, null, Constants.JsTdCroFra2016)]
        public void TestTieDetails(int tieId, string language, string source, string expectedJson)
        {
            var actionResult = EventController.GetTieDetails(tieId, language, source, false);
            var actionName = ((ActionNameAttribute)EventController.GetType().GetMethod("GetTieDetails").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<TieDetailsViewModel>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<TieDetailsViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(expectedResult));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, tieId.ToString());
        }

        [Theory]
        [InlineData("INV", null, null, null)]
        [InlineData("M-DC-2015-WG-M-GBR-BEL-01", null, null, Constants.JsTmBelGbr2015)]
        [InlineData("W-FC-2010-WG-M-ITA-USA-01", null, "fc", Constants.JsTmFcUsaIta2010)]
        public void TestTieNominations(string publicTieId, string language, string source, string expectedJson)
        {
            var actionResult = EventController.GetTieNominations(publicTieId, language, source, false);
            var actionName = ((ActionNameAttribute)EventController.GetType().GetMethod("GetTieNominations").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<ICollection<TieNominationViewModel>>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<TieNominationViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotEmpty(result);

            result.ToList().ForEach(x =>
            {
                var expectedX = expectedResult.FirstOrDefault(y => y.NationCode == x.NationCode);
                Assert.NotNull(expectedX);
                Assert.Equal(x.CapFN, expectedX.CapFN);
                Assert.Equal(x.CapFN, expectedX.CapFN);
                Assert.Equal(x.P1Id ?? string.Empty, expectedX.P1Id ?? string.Empty);
                Assert.Equal(x.P2Id ?? string.Empty, expectedX.P2Id ?? string.Empty);
                Assert.Equal(x.P3Id ?? string.Empty, expectedX.P3Id ?? string.Empty);
                Assert.Equal(x.P4Id ?? string.Empty, expectedX.P4Id ?? string.Empty);
                Assert.Equal(x.P5Id ?? string.Empty, expectedX.P5Id ?? string.Empty);
            });

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, publicTieId);
        }

        [Theory]
        [InlineData("INV", null, null, null)]
        [InlineData("M-DC-2015-WG-M-GBR-BEL-01", null, null, Constants.JsTrwBelGbr2015)]
        [InlineData("W-FC-2010-WG-M-ITA-USA-01", null, "fc", Constants.JsTrwFcUsaIta2010)]
        public void TestTieResultsWeb(string tieId, string language, string source, string expectedJson)
        {
            var actionResult = EventController.GetTieResultsWeb(tieId, language, source, false);
            var actionName = ((ActionNameAttribute)EventController.GetType().GetMethod("GetTieResultsWeb").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<ICollection<TieResultsWebViewModel>>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<TieResultsWebViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotEmpty(result);

            Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(expectedResult));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, tieId);
        }

        [Theory]
        [InlineData(0, null, null, null)]
        [InlineData(100029612, null, null, Constants.JsTrSrbKaz2016)]
        public void TestTieResults(int tieId, string language, string source, string expectedJson)
        {
            var actionResult = EventController.GetTieResults(tieId, language, source, false);
            var actionName = ((ActionNameAttribute)EventController.GetType().GetMethod("GetTieResults").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<ICollection<TieResultsViewModel>>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<TieResultsViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotEmpty(result);

            Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(expectedResult));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, tieId.ToString());
        }

        [Theory]
        [InlineData(null, null, "GBR", "ARG", "M-DC-2016-WG-M-GBR-ARG-01")]
        [InlineData(null, null, "GBR", "Argentina", "M-DC-2016-WG-M-GBR-ARG-01")]
        [InlineData(null, "fc", "France", "USA", "W-FC-2014-WG-PO-USA-FRA-01")]
        [InlineData(null, "fc", "ARG", "GBR", "W-FC-2013-WG2-PO-GBR-ARG-01")]
        public void TestSearchTies(string language, string source, string searchText1, string searchText2,
            string expectedTie)
        {
            var actionResult = EventController.SearchTies(searchText1, searchText2, language, source, false);
            var actionName = ((ActionNameAttribute)EventController.GetType().GetMethod("SearchTies").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<TieDetailsViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains(result, x => x.PublicTieId.Equals(expectedTie));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, searchText1, searchText2);
        }
    }
}
