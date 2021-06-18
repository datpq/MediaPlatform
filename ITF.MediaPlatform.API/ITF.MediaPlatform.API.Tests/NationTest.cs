using System;
using ITF.DataServices.SDK.Models.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using ITF.DataServices.SDK.Models.ViewModels.Btd;
using Xunit;

namespace ITF.MediaPlatform.API.Tests
{
    public class NationTest : BaseTest
    {
        public NationTest(IisExpressFixture iisExpressFixture) : base(iisExpressFixture)
        {
        }

        [Theory]
        [InlineData("INV", null, null, null)]
        [InlineData("aus", null, null, Constants.JsNpAus)]
        [InlineData("fra", "es", null, Constants.JsNpFraEs)]
        [InlineData("gbr", null, "fc", Constants.JsNpGbrFc)]
        [InlineData("gbr", "es", "dc", Constants.JsNpGbrEsDc)]
        [InlineData("esp", "es", null, Constants.JsNpEspEs)]
        public void TestNationProfile(string nationCode, string language, string source, string expectedJson)
        {
            var actionResult = NationController.GetNation(nationCode, language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetNation").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid nation code
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<NationViewModel>(expectedJson);

            //var contentResult = Assert.IsType<OkNegotiatedContentResult<NationViewModel>>(actionResult);
            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<NationViewModelOld>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            Assert.Equal(result.Nation, expectedResult.Nation);
            Assert.True(result.Champion >= expectedResult.Champion, $"champion={result.Champion}, expected>={expectedResult.Champion}");
            if (expectedResult.Champion > 0)
            {
                Assert.Contains(expectedResult.ChampionYears.Substring(1, expectedResult.ChampionYears.Length - 2), result.ChampionYears);
            }
            Assert.Equal(result.FirstYearPlayed, expectedResult.FirstYearPlayed);
            Assert.True(result.YearsPlayed >= expectedResult.YearsPlayed);
            Assert.True(int.Parse(result.TiesPlayed.Substring(0, result.TiesPlayed.IndexOf("(", StringComparison.Ordinal)))
                >= int.Parse(expectedResult.TiesPlayed.Substring(0, expectedResult.TiesPlayed.IndexOf("(", StringComparison.Ordinal))), "unexpected TiesPlayed");

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nationCode);
        }

        [Theory]
        [InlineData("INV", null, null, null)]
        [InlineData("gbr", null, null, Constants.JsNpWebGbr)]
        [InlineData("fra", "es", "fc", Constants.JsNpWebFraEs)]
        public void TestNationProfileWeb(string nationCode, string language, string source, string expectedJson)
        {
            var actionResult = NationController.GetNationProfileWeb(nationCode, language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetNationProfileWeb").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid nation code
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<NationProfileWebViewModel>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<NationProfileWebViewModelOld>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            Assert.Equal(result.OfficialName, expectedResult.OfficialName);
            Assert.Equal(result.WebsiteURL, expectedResult.WebsiteURL);
            Assert.True(!string.IsNullOrEmpty(result.NextTieDesc));
            Assert.True(!string.IsNullOrEmpty(result.LastTieDesc));
            Assert.True(!string.IsNullOrEmpty(result.History));
            Assert.Equal(result.Record, expectedResult.Record);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nationCode);
        }

        [Theory]
        [InlineData("INV", null, null, null)]
        [InlineData("aus", null, null, "Australia")]
        public void TestRankingsNation(string nationCode, string language, string source, string expectedNation)
        {
            var actionResult = NationController.GetRankingsNation(nationCode, language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetRankingsNation").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedNation == null)
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<NationRankingCoreViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.True(result.Count >= 3 && result.Count <= 5, "unexpected result.Count");
            Assert.Contains(result, x => x.Nation == expectedNation);

            //TODO contentResult is returned with type NationRankingViewModel, but web api returns NationRankingCoreViewModel
            //IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nationCode);
        }

        [Theory]
        [InlineData(0, "es", null, "Gran Bretaña")]
        [InlineData(10, "es", "fc", "Francia")]
        public void TestRankings(int top, string language, string source, string expectedNation)
        {
            var actionResult = NationController.GetRankings(top, language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetRankings").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<NationRankingViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains(result, x => x.Nation == expectedNation);
            if (top == 0)
            {
                Assert.True(result.Count > 100);
            }
            else
            {
                Assert.Equal(result.Count, top);
            }

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, top.ToString());
        }

        [Theory]
        [InlineData("INV", "gbr", null, null, null)]
        [InlineData("arg", "aus", null, null, Constants.JsH2hN2NArgAus)]
        public void TestHeadToHeadNationToNation(string nationCode, string opponentNationCode, string language, string source, string expectedJson)
        {
            var actionResult = NationController.GetHeadToHeadNationToNation(nationCode, opponentNationCode, language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetHeadToHeadNationToNation").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid nation code
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<HeadToHeadNationToNationViewModel>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<HeadToHeadNationToNationViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            Assert.Equal(expectedResult.NationCode, result.NationCode);
            Assert.Equal(expectedResult.NationName, result.NationName);
            Assert.Equal(expectedResult.OppositionNationCode, result.OppositionNationCode);
            Assert.Equal(expectedResult.OppositionNationName, result.OppositionNationName);
            Assert.True(result.WinTotal >= expectedResult.WinTotal);
            Assert.True(result.LossTotal >= expectedResult.LossTotal);
            Assert.True(result.WinClay >= expectedResult.WinClay);
            Assert.True(result.LossClay >= expectedResult.LossClay);

            expectedResult.Ties.ToList().ForEach(expectedX =>
            {
                var resultX = result.Ties.Single(x => x.PublicTieId == expectedX.PublicTieId);
                Assert.NotNull(resultX);
                Assert.Equal(JsonConvert.SerializeObject(resultX), JsonConvert.SerializeObject(expectedX));
            });

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nationCode, opponentNationCode);
        }

        [Theory]
        [InlineData("INV", null, null, null)]
        [InlineData("aus", null, null, Constants.JsWlAus)]
        [InlineData("gbr", "es", "fc", Constants.JsWlGbrEsFc)]
        public void TestNationWinLossRecords(string nationCode, string language, string source, string expectedJson)
        {
            var actionResult = NationController.GetNationWinLossRecords(nationCode, language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetNationWinLossRecords").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid nation code
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<ICollection<NationWinLossRecordsViewModel>>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<NationWinLossRecordsViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            expectedResult.ToList().ForEach(expectedX =>
            {
                var resultY = result.Single(x => x.Year == expectedX.Year);
                Assert.NotNull(resultY);
                Assert.Equal(expectedX.Ties.Count, resultY.Ties.Count);
                for (var i = 0; i < expectedX.Ties.Count; i++)
                {
                    Assert.Equal(expectedX.Ties[i].Score, resultY.Ties[i].Score);
                    Assert.Equal(expectedX.Ties[i].OpponentNationCode, resultY.Ties[i].OpponentNationCode);
                }
            });

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nationCode);
        }

        [Theory]
        [InlineData("INV", null, null, 0)]
        [InlineData("gbr", null, null, 100)]
        [InlineData("gbr", null, "fc", 45)]
        public void TestNationPlayers(string nationCode, string language, string source, int expectedCount)
        {
            var actionResult = NationController.GetNationPlayers(nationCode, language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetNationPlayers").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedCount == 0)//invalid nation code
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<PlayerViewModelCore>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.True(result.Count > expectedCount);
            //Assert.True(result.All(x => x.NationCode == nationCode));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, $"{actionName}", language, source, nationCode);
        }

        [Theory]
        [InlineData("INV", 1, null, null, null)]
        [InlineData("arg", 2015, null, null, Constants.JsNpwlArg2015)]
        [InlineData("gbr", 2015, "es", "fc", Constants.JsNpwlEsFcGbr)]
        public void TestNationPlayersWinLossRecords(string nationCode, int year, string language, string source, string expectedJson)
        {
            var actionResult = NationController.GetNationPlayersWinLossRecords(nationCode, year, language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetNationPlayersWinLossRecords").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid nation code
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<ICollection<NationPlayersWinLossRecords>>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<NationPlayersWinLossRecords>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(expectedResult));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nationCode, year.ToString());
        }

        [Theory]
        [InlineData("INV", null, null, null)]
        [InlineData("arg", null, null, Constants.JsPcArg)]
        [InlineData("gbr", "es", "fc", Constants.JsPcFcGbr)]
        public void TestNationPlayersCareerRecords(string nationCode, string language, string source, string expectedJson)
        {
            var actionResult = NationController.GetNationPlayersCareerRecords(nationCode, language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetNationPlayersCareerRecords").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid nation code
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<ICollection<NationPlayersCareerRecords>>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<NationPlayersCareerRecords>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            expectedResult.ToList().ForEach(expectedX =>
            {
                var x = result.FirstOrDefault(y => y.PlayerId == expectedX.PlayerId);
                Assert.NotNull(x);
                Assert.Equal(JsonConvert.SerializeObject(x), JsonConvert.SerializeObject(expectedX));
            });

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nationCode);
        }

        [Theory]
        [InlineData("INV", null, null, null)]
        [InlineData("arg", null, null, Constants.JsNsArg)]
        public void TestNationStatsRecords(string nationCode, string language, string source, string expectedJson)
        {
            var actionResult = NationController.GetNationStatsRecords(nationCode, language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetNationStatsRecords").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid nation code
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<NationStatsRecordsViewModel>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<NationStatsRecordsViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            Assert.True(result.YoungestPlayerAgeYearsPlayed <= expectedResult.YoungestPlayerAgeYearsPlayed);
            if (result.YoungestPlayerAgeYearsPlayed == expectedResult.YoungestPlayerAgeYearsPlayed)
            {
                Assert.True(result.YoungestPlayerAgeDaysPlayed <= expectedResult.YoungestPlayerAgeDaysPlayed);
            }
            Assert.True(result.OldestPlayerAgeYearsPlayed >= expectedResult.OldestPlayerAgeYearsPlayed);
            if (result.OldestPlayerAgeYearsPlayed == expectedResult.OldestPlayerAgeYearsPlayed)
            {
                Assert.True(result.OldestPlayerAgeDaysPlayed >= expectedResult.OldestPlayerAgeDaysPlayed);
            }
            Assert.True(string.CompareOrdinal(result.LongestRubberDurationTime.PadLeft(4, '0'),
                expectedResult.LongestRubberDurationTime.PadLeft(4, '0')) >= 0);
            Assert.True(string.CompareOrdinal(result.LongestTieDurationTime.PadLeft(4, '0'),
                expectedResult.LongestTieDurationTime.PadLeft(4, '0')) >= 0);
            Assert.NotEmpty(result.LongestTieBreak);
            Assert.True(result.LongestTieBreak.All(x => x.TotalPoints >= expectedResult.LongestTieBreak.FirstOrDefault()?.TotalPoints));
            Assert.NotEmpty(result.LongestFinalSet);
            Assert.True(result.LongestFinalSet.All(x => x.TotalGames >= expectedResult.LongestFinalSet.FirstOrDefault()?.TotalGames));
            Assert.NotEmpty(result.MostGamesInRubber);
            Assert.True(result.MostGamesInRubber.All(x => x.TotalGames >= expectedResult.MostGamesInRubber.FirstOrDefault()?.TotalGames));
            Assert.NotEmpty(result.MostGamesInSet);
            Assert.True(result.MostGamesInSet.All(x => x.TotalGames >= expectedResult.MostGamesInSet.FirstOrDefault()?.TotalGames));
            //Assert.NotEmpty(result.MostGamesInTie);
            //Assert.True(result.MostGamesInTie.All(x => x.TotalGames >= expectedResult.MostGamesInTie.FirstOrDefault()?.TotalGames));
            Assert.NotEmpty(result.MostDecisiveVictoryInTie);
            Assert.True(result.LongestWinRunNumber >= expectedResult.LongestWinRunNumber);
            Assert.NotEmpty(result.LongestWinRun);
            Assert.NotEmpty(result.ComebackTwoOneDown);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nationCode);
        }

        [Theory]
        [InlineData("INV", null, null, null)]
        [InlineData("gbr", null, null, Constants.JsNatrGbr)]
        [InlineData("fra", null, "fc", Constants.JsNatrFraFc)]
        public void TestNationAllTimeRecords(string nationCode, string language, string source, string expectedJson)
        {
            var actionResult = NationController.GetNationAllTimeRecords(nationCode, language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetNationAllTimeRecords").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid nation code
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<NationAllTimeRecordsViewModel>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<NationAllTimeRecordsViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            Assert.NotNull(result);
            Assert.Equal(result.MostTotalWins, expectedResult.MostTotalWins);
            Assert.Equal(result.MostDoublesWins, expectedResult.MostDoublesWins);
            Assert.Equal(result.MostSinglesWins, expectedResult.MostSinglesWins);
            Assert.Equal(result.BestDoublesPair, expectedResult.BestDoublesPair);
            Assert.Equal(result.MostTiesPlayed, expectedResult.MostTiesPlayed);
            Assert.Equal(result.MostYearsPlayed, expectedResult.MostYearsPlayed);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nationCode);
        }

        [Theory]
        [InlineData(1, null, null, null)]
        [InlineData(2016, "es", null, Constants.JsNgGbrEs2016)]
        [InlineData(2015, null, "fc", Constants.JsNgFinFc2015)]
        public void TestNationsGroup(int year, string language, string source, string expectedJson)
        {
            var actionResult = NationController.GetNationsGroup(year, language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetNationsGroup").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid year
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<ICollection<NationsGroupViewModel>>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<NationsGroupViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            expectedResult.ToList().ForEach(x =>
            {
                Assert.Contains(result, y => y.Nation == x.Nation && y.ZoneCode == x.ZoneCode && y.DivisionCode == x.DivisionCode);
            });

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, year.ToString());
        }

        [Theory]
        [InlineData(null, null, "Great Britain")]
        [InlineData("es", null, "Gran Bretaña")]
        [InlineData(null, "fc", "Great Britain")]
        public void TestNations(string language, string source, string expectedNation)
        {
            var actionResult = NationController.GetNations(language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetNations").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<NationCoreViewModelOld>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains(result, x => x.Nation == expectedNation);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source);
        }


        [Theory]
        [InlineData(244546, null, null, "Barbados")]
        [InlineData(244546, null, null, "Dominican Republic")]
        [InlineData(244546, "es", null, "República Dominicana")]
        public void TestNodeRelatedNations(int nodeId, string language, string source, string expectedNation)
        {
            var actionResult = NationController.GetNodeRelatedNation(nodeId, language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetNodeRelatedNation").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<NationCoreViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains(result, x => x.Nation == expectedNation);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nodeId.ToString());
        }

        [Theory]
        [InlineData(null, null, 2015, "GBR", "Great Britain", "3 - 1")]
        [InlineData("es", null, 2015, "GBR", "Gran Bretaña", "3 - 1")]
        [InlineData(null, null, 1974, "RSA", "South Africa", " - ")]
        [InlineData("es", null, 1974, "RSA", "Sudáfrica", " - ")]
        [InlineData("es", "fc", 2015, "CZE", "República Checa", "3 - 2")]
        public void TestChampions(string language, string source,
            int expectedYear, string expectedChampionNationCode, string expectedChampionNationName, string expectedScore)
        {
            var actionResult = NationController.GetChampions(language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("GetChampions").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<ChampionViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains(result, x => x.Year == expectedYear && x.ChampionNationCode == expectedChampionNationCode
            && $"{x.ChampionScore} - {x.FinalistScore}" == expectedScore);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source);
        }

        [Theory]
        [InlineData(null, null, "ar", "Argentina")]
        [InlineData("es", null, "ar", "Argentina")]
        [InlineData(null, "fc", "gb", "Guinea-Bissau")]
        [InlineData("es", "fc", "gb", "Gran Bretaña")]
        public void TestSearchNations(string language, string source, string searchText, string expectedNation)
        {
            var actionResult = NationController.SearchNations(searchText, language, source, false);
            var actionName = ((ActionNameAttribute)NationController.GetType().GetMethod("SearchNations").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<NationCoreViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains(result, x => x.Nation == expectedNation);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, searchText);
        }

        #region Btd Tests

        [Theory]
        [InlineData("INV", null, null, null)]
        [InlineData("GBR", null, null, Constants.JsBtdNaRrGbr)]
        [InlineData("GBR", null, "fc", Constants.JsBtdNaRrFcGbr)]
        public void TestBtdNation(string nationCode, string language, string source, string expectedJson)
        {
            var actionResult = BtdController.GetBtdNation(nationCode, language, source, false);
            var actionName = ((ActionNameAttribute)BtdController.GetType().GetMethod("GetBtdNation").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;
            var routePrefix = ((RoutePrefixAttribute)BtdController.GetType().GetCustomAttributes(typeof(RoutePrefixAttribute), true).FirstOrDefault())?.Prefix;
            if (routePrefix != null)
            {
                actionName = $"{routePrefix}/{actionName}";
            }

            if (expectedJson == null)//invalid nation code
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<BtdNationViewModel>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<BtdNationViewModel>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotNull(result.Nation);
            Assert.NotNull(result.NationProfile);
            Assert.NotNull(result.NationProfile.History);
            Assert.NotNull(result.NationAllTimeRecords);
            Assert.True(result.RecentPlayers.All(x => x.NationCode == nationCode));
            result.RecentResults.Where(x => x.Year != 2016).ToList().ForEach(x =>
            {
                var expectedX = expectedResult.RecentResults.FirstOrDefault(y => y.Year == x.Year);
                if (expectedX != null)
                {
                    Assert.Equal(JsonConvert.SerializeObject(expectedX), JsonConvert.SerializeObject(x));
                }
            });

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, $"{actionName}", language, source, nationCode);
        }

        [Theory]
        [InlineData("INV", null, null)]
        [InlineData("gbr", null, null)]
        [InlineData("gbr", null, "fc")]
        public void TestBtdMyTeam(string nationCode, string language, string source)
        {
            var actionResult = BtdController.GetBtdMyTeam(nationCode, language, source, false);
            var actionName = ((ActionNameAttribute)BtdController.GetType().GetMethod("GetBtdMyTeam").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;
            var routePrefix = ((RoutePrefixAttribute)BtdController.GetType().GetCustomAttributes(typeof(RoutePrefixAttribute), true).FirstOrDefault())?.Prefix;
            if (routePrefix != null)
            {
                actionName = $"{routePrefix}/{actionName}";
            }

            if (nationCode == "INV")//invalid nation code
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }

            var contentResult = actionResult as OkNegotiatedContentResult<BtdMyTeamViewModel>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotNull(result.NextTieId);
            Assert.NotNull(result.NextTieDesc);
            Assert.NotNull(result.NextTieDate);
            Assert.NotNull(result.LastTieId);
            Assert.NotNull(result.LastTieDesc);
            Assert.NotNull(result.LastTieDate);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, $"{actionName}", language, source, nationCode);
        }

        [Theory]
        [InlineData("INV", null, null, 0)]
        [InlineData("gbr", null, null, 100)]
        [InlineData("gbr", null, "fc", 45)]
        public void TestBtdNationPlayers(string nationCode, string language, string source, int expectedCount)
        {
            var actionResult = BtdController.GetBtdNationPlayers(nationCode, language, source, false);
            var actionName = ((ActionNameAttribute)BtdController.GetType().GetMethod("GetBtdNationPlayers").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;
            var routePrefix = ((RoutePrefixAttribute)BtdController.GetType().GetCustomAttributes(typeof(RoutePrefixAttribute), true).FirstOrDefault())?.Prefix;
            if (routePrefix != null)
            {
                actionName = $"{routePrefix}/{actionName}";
            }

            if (expectedCount == 0)//invalid nation code
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }

            var contentResult = actionResult as OkNegotiatedContentResult<BtdNationPlayersViewModel>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.True(result.AllPlayers.Count > expectedCount);
            Assert.True(result.RecentPlayers.Count > 0);
            Assert.True(result.RecentPlayers.All(x => x.HeadshotImgId.HasValue));
            //Assert.True(result.All(x => x.NationCode == nationCode));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, $"{actionName}", language, source, nationCode);
        }

        #endregion
    }
}
