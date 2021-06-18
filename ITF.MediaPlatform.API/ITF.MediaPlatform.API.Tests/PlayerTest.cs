using System;
using System.Collections.Generic;
using ITF.DataServices.SDK;
using ITF.DataServices.SDK.Models.ViewModels;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMapper;
using ITF.DataServices.SDK.Models.ViewModels.Btd;
using Xunit;

namespace ITF.MediaPlatform.API.Tests
{
    public class PlayerTest : BaseTest
    {
        public PlayerTest(IisExpressFixture iisExpressFixture) : base(iisExpressFixture)
        {
        }

        [Theory]
        [InlineData(null, "dc", 300)]
        [InlineData(null, "fc", 100)]
        public void TestCommitmentAwardEligiblePlayers(string language, string source, int? expectedCount)
        {
            var actionResult = HomeController.GetCommitmentAwardEligiblePlayers(language, source);
            var actionName = ((ActionNameAttribute)HomeController.GetType().GetMethod("GetCommitmentAwardEligiblePlayers").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedCount == null)
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<PlayerViewModelCoreOld>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;
            Assert.True(result.Count >= expectedCount.Value);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source);
        }

        [Theory]
        [InlineData(null, "dc", 5)]
        [InlineData(null, "fc", 3)]
        public void TestCommitmentAwardOneTieToPlayPlayers(string language, string source, int? expectedCount)
        {
            var actionResult = HomeController.GetCommitmentAwardOneTieToPlayPlayers(language, source);
            var actionName = ((ActionNameAttribute)HomeController.GetType().GetMethod("GetCommitmentAwardOneTieToPlayPlayers").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedCount == null)
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<PlayerViewModelCoreOld>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;
            Assert.True(result.Count >= expectedCount.Value);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source);
        }

        [Theory]
        [InlineData(null, null, 1, null)]
        [InlineData(null, null, 133571, 1)]
        [InlineData("es", null, 133621, 1)]
        public void TestFeaturedPlayers(string language, string source, int cmsNodeId, int? expectedCount)
        {
            var actionResult = HomeController.GetFeaturedPlayers(cmsNodeId, language, source);
            var actionName = ((ActionNameAttribute)HomeController.GetType().GetMethod("GetFeaturedPlayers").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedCount == null)//invalid player id
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<PlayerViewModelCoreOld>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;
            Assert.True(result.Count >= expectedCount.Value);
            Assert.True(result.All(player => player.HeadshotImgId.HasValue));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, cmsNodeId.ToString());
        }

        [Theory]
        [InlineData(1, null)]
        [InlineData(800233481, Constants.JsPlMurray)]
        [InlineData(800246767, Constants.JsPlCornetEsFc)]
        public void TestPlayerCoreProfile(int id, string expectedJson)
        {
            var actionResult = HomeController.GetPlayerCoreProfile(id, false);
            var actionName = ((ActionNameAttribute)HomeController.GetType().GetMethod("GetPlayerCoreProfile").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;
            if (actionName == null)
            {
                actionName = ((RouteAttribute)HomeController.GetType().GetMethod("GetPlayerCoreProfile")
                    .GetCustomAttributes(typeof(RouteAttribute), true).FirstOrDefault())?.Template ?? string.Empty;
                actionName = Regex.Replace(actionName, @"(\/\{.*\})", string.Empty);
            }

            if (expectedJson == null)//invalid player id
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<PlayerViewModelCore>(expectedJson);
            var contentResult = actionResult as OkNegotiatedContentResult<PlayerViewModelCoreOld>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            ComparePlayerCoreInfo(result, expectedResult);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, id.ToString());
        }

        [Theory]
        [InlineData(null, null, 1, null)]
        [InlineData(null, null, 800233481, Constants.JsPlMurray)]
        [InlineData("es", "fc", 800246767, Constants.JsPlCornetEsFc)]
        public void TestPlayerProfile(string language, string source, int id, string expectedJson)
        {
            var actionResult = HomeController.GetPlayer(id, language, source, false);
            var actionName = ((ActionNameAttribute)HomeController.GetType().GetMethod("GetPlayer").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid player id
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<PlayerViewModel>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<PlayerViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            ComparePlayerInfo(result, expectedResult);

            if (language == null) // Player/{Id} is a different endpoint return PlayerViewModelCore, so must be clear here
            {
                language = DataServices.SDK.Constants.DefaultLanguage;
            }
            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, id.ToString());
        }

        [Theory]
        [InlineData("M-DC-2016-G1-AM-REL-BAR-DOM-01", null, null, 800277989)]
        [InlineData("M-DC-2016-G1-AM-REL-BAR-DOM-01", null, null, 800202586)]
        public void TestPlayersFromTie(string publicTieId, string language, string source, int expectedPlayerId)
        {
            var actionResult = HomeController.GetPlayersFromTie(publicTieId, language, source, false);
            var actionName = ((ActionNameAttribute)HomeController.GetType().GetMethod("GetPlayersFromTie").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<PlayerViewModelCore>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains(result, x => x.PlayerId == expectedPlayerId);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, publicTieId);
        }

        private static void ComparePlayerCoreCoreInfo(PlayerViewModelCoreCore result, PlayerViewModelCoreCore expectedResult)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (expectedResult == null) throw new ArgumentNullException(nameof(expectedResult));
            Assert.Equal(result.NationCode, expectedResult.NationCode);
            Assert.Equal(result.GivenName + " " + result.FamilyName, expectedResult.GivenName + " " + expectedResult.FamilyName);
            Assert.Equal(result.Gender, expectedResult.Gender);
        }

        private static void ComparePlayerCoreInfo(PlayerViewModelCore result, PlayerViewModelCore expectedResult)
        {
            ComparePlayerCoreCoreInfo(result, expectedResult);
            Assert.True(result.HeadshotImgId.HasValue);
            Assert.True(!string.IsNullOrEmpty(result.HeadshotUrl));
        }

        private static void ComparePlayerInfo(PlayerViewModel result, PlayerViewModel expectedResult)
        {
            ComparePlayerCoreInfo(result, expectedResult);

            Assert.Equal(result.BirthDate, expectedResult.BirthDate);
            Assert.Equal(result.BirthPlace, expectedResult.BirthPlace);
            Assert.Equal(result.FirstYearPlayed, expectedResult.FirstYearPlayed);
            Assert.True(int.Parse(result.TotalNominations) >= int.Parse(expectedResult.TotalNominations), "unexpected TotalNominations");
            Assert.True(result.TiesPlayed >= expectedResult.TiesPlayed, "unexpected TiesPlayed");
        }

        private static void ComparePlayerActivity(PlayerActivityViewModel result, PlayerActivityViewModel expectedResult)
        {
            Assert.True(result.TotalWinTotal >= expectedResult.TotalWinTotal, "unexpected TotalWinTotal");
            Assert.True(result.TotalWinSingles >= expectedResult.TotalWinSingles, "unexpected TotalWinSingles");
            Assert.True(result.TotalWinDoubles >= expectedResult.TotalWinDoubles, "unexpected TotalWinDoubles");
            Assert.True(result.TotalLossTotal >= expectedResult.TotalLossTotal, "unexpected TotalLossTotal");
            Assert.True(result.TotalLossSingles >= expectedResult.TotalLossSingles, "unexpected TotalLossSingles");
            Assert.True(result.TotalLossDoubles >= expectedResult.TotalLossDoubles, "unexpected TotalLossDoubles");

            expectedResult.Ties.ForEach(expectedTie =>
            {
                var resultTie = result.Ties.Single(x => x.TieId == expectedTie.TieId);
                Assert.NotNull(resultTie);

                //clear all the matches before compare
                var duplicateX = expectedTie.CloneJson();
                var duplicateY = resultTie.CloneJson();
                duplicateX.Matches.Clear();
                duplicateY.Matches.Clear();
                Assert.Equal(JsonConvert.SerializeObject(duplicateX), JsonConvert.SerializeObject(duplicateY));

                expectedTie.Matches.ForEach(expectedMatch =>
                {
                    var resultMatch = resultTie.Matches.Single(x => x.OpponentPlayerId == expectedMatch.OpponentPlayerId);
                    Assert.NotNull(resultMatch);
                    Assert.Equal(JsonConvert.SerializeObject(expectedMatch), JsonConvert.SerializeObject(resultMatch));
                });
            });
        }

        [Theory]
        [InlineData(null, null, 1, null)]
        [InlineData(null, null, 800233481, Constants.JsPaMurray)]
        [InlineData("es", "fc", 800246767, Constants.JsPaCornetEsFc)]
        public void TestPlayerActivity(string language, string source, int id, string expectedJson)
        {
            var actionResult = HomeController.GetPlayerActivity(id, language, source, false);
            var actionName = ((ActionNameAttribute)HomeController.GetType().GetMethod("GetPlayerActivity").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid player id
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<PlayerActivityViewModel>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<PlayerActivityViewModelOld>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = Mapper.Map<PlayerActivityViewModel>(contentResult.Content.FirstOrDefault());
            Assert.NotNull(result);

            ComparePlayerActivity(result, expectedResult);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, id.ToString());
        }

        [Theory]
        [InlineData(null, null, 1, 1, null)]
        [InlineData(null, null, 800235018, 800202678, Constants.JsH2Hp2PMofilsFederer)]
        public void TestHeadToHeadPlayerToPlayer(string language, string source, int id, int opponentId, string expectedJson)
        {
            var actionResult = HomeController.GetHeadToHeadPlayerToPlayer(id, opponentId, language, source, false);
            var actionName = ((ActionNameAttribute)HomeController.GetType().GetMethod("GetHeadToHeadPlayerToPlayer").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedJson == null)//invalid player id
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<HeadToHeadPlayerToPlayerViewModel>(expectedJson);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<HeadToHeadPlayerToPlayerViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = Mapper.Map<HeadToHeadPlayerToPlayerViewModel>(contentResult.Content.FirstOrDefault());
            Assert.NotNull(result);

            Assert.True(result.WinTotal >= expectedResult.WinTotal, "unexpected WinTotal");
            Assert.True(result.LossTotal >= expectedResult.LossTotal, "unexpected LossTotal");
            Assert.Equal(result.PlayerGivenName, expectedResult.PlayerGivenName);
            Assert.Equal(result.PlayerNationCode, expectedResult.PlayerNationCode);
            Assert.Equal(result.OppositionPlayerNationCode, expectedResult.OppositionPlayerNationCode);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, id.ToString(), opponentId.ToString());
        }

        [Theory]
        [InlineData(null, null, 800233481, "ARG", 1, 1)]
        public void TestHeadToHeadPlayerToNation(string language, string source, int id, string opponentNation, int expectedWinTotal, int expectedLossTotal)
        {
            var actionResult = HomeController.GetHeadToHeadPlayerToNation(id, opponentNation, language, source, false);
            var actionName = ((ActionNameAttribute)HomeController.GetType().GetMethod("GetHeadToHeadPlayerToNation").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<HeadToHeadPlayerToNationViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = Mapper.Map<HeadToHeadPlayerToNationViewModel>(contentResult.Content.FirstOrDefault());
            Assert.NotNull(result);

            Assert.True(result.WinTotal >= expectedWinTotal, "unexpected WinTotal");
            Assert.True(result.LossTotal >= expectedLossTotal, "unexpected LossTotal");

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, id.ToString(), opponentNation);
        }

        [Theory]
        [InlineData(null, null, 1, null, null)]
        [InlineData(null, null, 800233481, Constants.JsPlMurray, Constants.JsPaMurray)]
        [InlineData("es", "fc", 800246767, Constants.JsPlCornetEsFc, Constants.JsPaCornetEsFc)]
        public void TestBtdPlayer(string language, string source, int id, string expectedPlJson, string expectedPaJson)
        {
            var actionResult = BtdController.GetBtdPlayer(id, language, source, false);
            var actionName = ((ActionNameAttribute)BtdController.GetType().GetMethod("GetBtdPlayer").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;
            var routePrefix = ((RoutePrefixAttribute)BtdController.GetType().GetCustomAttributes(typeof(RoutePrefixAttribute), true).FirstOrDefault())?.Prefix;
            if (routePrefix != null)
            {
                actionName = $"{routePrefix}/{actionName}";
            }

            if (expectedPlJson == null)//invalid player id
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }

            var expectedPlResult = JsonConvert.DeserializeObject<PlayerViewModelCoreCore>(expectedPlJson);
            var expectedPaResult = JsonConvert.DeserializeObject<PlayerActivityViewModel>(expectedPaJson);

            var contentResult = actionResult as OkNegotiatedContentResult<BtdPlayerViewModel>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            ComparePlayerCoreCoreInfo(result.PlayerInfo, expectedPlResult);
            ComparePlayerActivity(result.PlayerActivityInfo, expectedPaResult);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, $"{actionName}", language, source, id.ToString());
        }

        [Theory]
        [InlineData(null, null, "murray", "Andy")]
        [InlineData(null, "fc", "williams", "Serena")]
        public void TestSearchPlayers(string language, string source, string searchText, string expectedPlayer)
        {
            var actionResult = HomeController.SearchPlayers(searchText, language, source, false);
            var actionName = ((ActionNameAttribute)HomeController.GetType().GetMethod("SearchPlayers").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<PlayerViewModelCore>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains(result, x => x.GivenName.Equals(expectedPlayer));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, searchText);
        }
    }
}
