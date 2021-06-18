using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using ITF.DataServices.SDK.Models.Media;
using ITF.DataServices.SDK.Models.ViewModels.Circuits;
using ITF.DataServices.SDK.Models.ViewModels.Cms;
using ITF.DataServices.SDK.Models.ViewModels.LiveBlog;
using Newtonsoft.Json;
using Xunit;

namespace ITF.MediaPlatform.API.Tests
{
    public class OlympicsTest : BaseTest
    {
        public OlympicsTest(IisExpressFixture iisExpressFixture) : base(iisExpressFixture)
        {
        }

        #region Player endpoints

        [Theory]
        [InlineData("olympics", 2016, 198)]
        [InlineData("paralympics", 2016, 100)]
        public void TestLatestOlympicsPlayers(string cc, int year, int playerCount)
        {
            var actionResult = CircuitPlayerController.GetLatestOlympicPlayers(cc, false);

            var contentResult = actionResult as OkNegotiatedContentResult<OlympicsPlayersViewModel>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            if (result.Year == year)
            {
                Assert.Equal(result.HostCity, "Rio");
                Assert.Equal(result.HostNationCode, "BRA");
                Assert.Equal(result.TournamentName, "Rio");
                Assert.Equal(result.Players.Count, playerCount);
            }
        }

        [Theory]
        [InlineData("olympics", 2012, 184)]
        [InlineData("paralympics", 2012, 112)]
        public void TestOlympicsPlayers(string cc, int year, int playerCount)
        {
            var actionResult = CircuitPlayerController.GetOlympicPlayers(cc, false);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<OlympicsPlayersViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            var olympicsPlayers = result.FirstOrDefault(x => x.Year == year);
            Assert.NotNull(olympicsPlayers);

            Assert.Equal(olympicsPlayers.HostCity, "London");
            Assert.Equal(olympicsPlayers.HostNationCode, "GBR");
            Assert.Equal(olympicsPlayers.TournamentName, "London");
            Assert.Equal(olympicsPlayers.Players.Count, playerCount);
        }

        [Theory]
        [InlineData("olympics", 1000)]
        [InlineData("paralympics", 300)]
        public void TestOlympicsPlayersForSearch(string cc, int playerCount)
        {
            var actionResult = CircuitPlayerController.GetOlympicPlayersForSearch(cc, false);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<PlayerViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.True(result.Count > playerCount);
            var first = result.FirstOrDefault();
            Assert.NotNull(first);
            Assert.NotNull(first.Gender);
            Assert.NotNull(first.PlayerFamilyName);
            Assert.NotNull(first.PlayerGivenName);
            Assert.NotNull(first.PlayerNationalityCode);
            Assert.NotNull(first.PlayerNationalityName);

            var last = result.LastOrDefault();
            Assert.NotNull(last);
            Assert.NotNull(last.Gender);
            Assert.NotNull(last.PlayerFamilyName);
            Assert.NotNull(last.PlayerGivenName);
            Assert.NotNull(last.PlayerNationalityCode);
            Assert.NotNull(last.PlayerNationalityName);
        }

        #endregion

        #region Player profile endpoints

        [Theory]
        [InlineData("olympics", 1, 1110228, null)]
        [InlineData("olympics", 1226863, 1, null)]
        [InlineData("olympics", 1226863, 1110228, Constants.JsOlyHhKvitovaWozniacki)]
        public void TestOlympicsPlayerHeadToHead(string cc, int playerId, int opponentPlayerId, string expectedJson)
        {
            var actionResult = CircuitPlayerController.GetOlympicHeadToHead(cc, playerId, opponentPlayerId, false);

            if (expectedJson == null)//invalid player id
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<HeadToHeadViewModel>(expectedJson);
            var contentResult = actionResult as OkNegotiatedContentResult<HeadToHeadViewModel>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.Equal(result.Player.GivenName, expectedResult.Player.GivenName);
            Assert.Equal(result.Player.FamilyName, expectedResult.Player.FamilyName);
            Assert.Equal(result.OpponentPlayer.GivenName, expectedResult.OpponentPlayer.GivenName);
            Assert.Equal(result.OpponentPlayer.FamilyName, expectedResult.OpponentPlayer.FamilyName);

            expectedResult.Matches.ToList().ForEach(expectedX =>
            {
                var x = result.Matches.FirstOrDefault(y => y.Year == expectedX.Year
                && y.TournamentName == expectedX.TournamentName
                && y.RoundCode == expectedX.RoundCode);
                Assert.NotNull(x);
                Assert.Equal(JsonConvert.SerializeObject(x), JsonConvert.SerializeObject(expectedX));
            });
        }

        [Theory]
        [InlineData("olympics", 1, null)]
        [InlineData("olympics", 800233481, Constants.JsOlyPaMurray)]
        [InlineData("paralympics", 800249103, Constants.JsOlyPaPeifer)]
        public void TestOlympicsPlayerActivity(string cc, int playerId, string expectedJson)
        {
            var actionResult = CircuitPlayerController.GetOlympicPlayerActivity(cc, playerId, false);

            if (expectedJson == null)//invalid player id
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<ICollection<TournamentViewModel>>(expectedJson);
            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<TournamentViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            expectedResult.ToList().ForEach(expectedX =>
            {
                var x = result.FirstOrDefault(y => y.Year == expectedX.Year);
                Assert.NotNull(x);
                expectedX.Events.ToList().ForEach(expectedY =>
                {
                    var y = x.Events.FirstOrDefault(z => z.EventId == expectedY.EventId);
                    Assert.NotNull(y);
                    expectedY.PlayerActivityMatches.ToList().ForEach(expectedZ =>
                    {
                        var z = y.PlayerActivityMatches.FirstOrDefault(t => t.PartnerPlayerID == expectedZ.PartnerPlayerID
                        && t.OpponentPlayerID == expectedZ.OpponentPlayerID
                        && t.OpponentPartnerPlayerID == expectedZ.OpponentPartnerPlayerID);
                        Assert.NotNull(z);
                        Assert.Equal(JsonConvert.SerializeObject(z), JsonConvert.SerializeObject(expectedZ));
                    });
                });
            });
        }

        [Theory]
        [InlineData("olympics", 1, null)]
        [InlineData("olympics", 800233481, Constants.JsOlyPlMurray)]
        [InlineData("olympics", 800246767, Constants.JsOlyPlCornet)]
        [InlineData("olympics", 1116240, Constants.JsOlyPlMakarova)]
        [InlineData("paralympics", 800249103, Constants.JsOlyPlPeifer)]
        public void TestOlympicsPlayerProfile(string cc, int playerId, string expectedJson)
        {
            var actionResult = CircuitPlayerController.GetOlympicPlayerProfile(cc, playerId, false);

            if (expectedJson == null)//invalid player id
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = cc == "olympics"
                ? (BasePlayerProfileViewModel) JsonConvert.DeserializeObject<PlayerProfileViewModel>(expectedJson)
                : JsonConvert.DeserializeObject<WheelchairPlayerProfileViewModel>(expectedJson);
            var contentResult = actionResult as OkNegotiatedContentResult<BasePlayerProfileViewModel>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.Equal(result.PlayerID, expectedResult.PlayerID);
            Assert.Equal(result.BirthDate, expectedResult.BirthDate);
            Assert.Equal(result.BirthPlace, expectedResult.BirthPlace);
            Assert.Equal(result.GivenName + " " + result.FamilyName, expectedResult.GivenName + " " + expectedResult.FamilyName);
            Assert.Equal(result.Gender, expectedResult.Gender);
            Assert.Equal(result.AgeBeganTennis, expectedResult.AgeBeganTennis);
            Assert.Equal(result.AgeTurnPro, expectedResult.AgeTurnPro);
            Assert.Equal(result.NationalityCode, expectedResult.NationalityCode);
        }

        #endregion

        #region Result endpoints

        [Theory]
        [InlineData("olympics", 1896, "Athens")]
        [InlineData("olympics", 2016, "Rio")]
        [InlineData("paralympics", 1992, "Barcelona")]
        [InlineData("paralympics", 2016, "Rio")]
        public void TestAllOlympics(string cc, int year, string tournamentName)
        {
            var actionResult = CircuitPlayerController.GetAllOlympics(cc, false);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<OlympicsViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.Contains(result, x => x.Year == year && x.TournamentName == tournamentName);
        }

        [Theory]
        [InlineData("olympics", 1, 0, 0)]
        [InlineData("olympics", 1100288090, 63, 1)]
        [InlineData("paralympics", 1100151890, 31, 1)]
        public void TestGetDrawsheetByEventId(string cc, int eventId, int matchCount, int thirdFourthEventMatchCount)
        {
            var actionResult = CircuitPlayerController.GetDrawsheetByEventId(cc, eventId, false);

            if (matchCount == 0)//invalid eventId
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var contentResult = actionResult as OkNegotiatedContentResult<DrawsheetViewModel>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result.Matches);
            Assert.Equal(result.Matches.Count, matchCount);
            Assert.NotNull(result.ThirdFourthEvent);
            Assert.NotNull(result.ThirdFourthEvent.Matches);
            Assert.Equal(result.ThirdFourthEvent.Matches.Count, thirdFourthEventMatchCount);
        }

        [Theory]
        [InlineData("olympics", 2015, "[]")]
        [InlineData("olympics", 2016, Constants.JsOlyOe2016)]
        [InlineData("paralympics", 2012, Constants.JsOlyPe2012)]
        public void TestGetOlympicEventsByYear(string cc, int year, string expectedJson)
        {
            var actionResult = CircuitPlayerController.GetOlympicEventsByYear(cc, year, false);

            if (expectedJson == null)//invalid year
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var expectedResult = JsonConvert.DeserializeObject<ICollection<EventViewModel>>(expectedJson);
            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<EventViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.Equal(JsonConvert.SerializeObject(result.OrderBy(x => x.EventId)),
                JsonConvert.SerializeObject(expectedResult.OrderBy(x => x.EventId)));
        }

        #endregion

        #region Instagram

        [Theory]
        [InlineData("fakeprofile", 0)]
        [InlineData("fedcuptennis", 1)]
        public void TestGetInstagramProfileData(string profile, int expectedItemCount)
        {
            var actionResult = CircuitPlayerController.GetInstagramProfileData(profile, false);

            var contentResult = actionResult as OkNegotiatedContentResult<Instagram>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            if (expectedItemCount == 0)//fake profile
            {
                Assert.Equal(result.items.Count, expectedItemCount);
            }
            else
            {
                Assert.True(result.items.Count > expectedItemCount);
            }
        }

        #endregion

        #region LiveBlog

        [Theory]
        [InlineData("fakeSiteLanguage", "fakeFileName", null)]
        [InlineData("olympics-en", "06-03-2016", "")]
        [InlineData("olympics-es", "06-03-2016", null)]
        public void TestGetLiveBlogData(string siteLanguage, string fileName, string expectedJson)
        {
            var actionResult = CircuitPlayerController.GetLiveBlogData(siteLanguage, fileName, false);

            if (expectedJson == null)//invalid data
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var contentResult = actionResult as OkNegotiatedContentResult<LiveBlogDataViewModel>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
        }

        #endregion

        #region Translations

        [Theory]
        [InlineData("en", "Profile", "Profile")]
        [InlineData("es", "Profile", "Perfil")]
        [InlineData("fakeLanguage", "Profile", null)]
        public void TestGetTranslationsByIso(string languageIsoCode, string word, string expectedTranslation)
        {
            var actionResult = CircuitPlayerController.GetTranslationsByIso(languageIsoCode, false);

            var contentResult = actionResult as OkNegotiatedContentResult<Dictionary<string, string>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            if (expectedTranslation == null)//invalid data
            {
                Assert.Empty(result);
                return;
            }
            Assert.True(result.ContainsKey(word));
            Assert.Equal(result[word], expectedTranslation);
        }

        #endregion

        #region Umbraco Media

        [Theory]
        [InlineData(30022225, 224393)]
        [InlineData(1, 0)]
        public void TestGetPlayerImageAssetId(int playerId, int? expectedResult)
        {
            var actionResult = CircuitPlayerController.GetPlayerImageAssetId(playerId, false);

            var contentResult = actionResult as OkNegotiatedContentResult<int?>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.Equal(result, expectedResult);
        }

        [Theory]
        [InlineData(65613, 40)]
        [InlineData(1, 0)]
        public void TestGetGalleriesByWebScopeId(int webScopeNodeId, int expectedCount)
        {
            var actionResult = CircuitPlayerController.GetGalleriesByWebScopeId(webScopeNodeId, false);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<PhotosGalleryViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);

            Assert.True(result.Count >= expectedCount);
            result.ToList().ForEach(x =>
            {
                Assert.NotNull(x.TitleEn);
                Assert.NotNull(x.TitleEs);
                Assert.True(x.PhotoId > 0);
            });
        }

        [Theory]
        [InlineData(183200, Constants.JsOlyApPaulZimmer)]
        [InlineData(1, Constants.JsOlyApFake1)]
        public void TestGetAssetProvider(int assetId, string expectedJson)
        {
            var actionResult = CircuitPlayerController.GetAssetProvider(assetId, false);

            if (expectedJson == null)//invalid data
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }

            var expectedResult = JsonConvert.DeserializeObject<ImageAssetViewModel>(expectedJson);
            var contentResult = actionResult as OkNegotiatedContentResult<ImageAssetViewModel>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.Equal(result.AssetId, expectedResult.AssetId);
            Assert.Equal(result.Photographer, expectedResult.Photographer);
            Assert.Equal(result.PhotographerId, expectedResult.PhotographerId);
            Assert.Equal(JsonConvert.SerializeObject(result.Descriptions.OrderBy(x => x.CultureCode)),
                JsonConvert.SerializeObject(expectedResult.Descriptions.OrderBy(x => x.CultureCode)));
        }

        [Theory]
        [InlineData(182613, 247)]
        [InlineData(0, 0)]
        public void TestGetRelatedMediaAssets(int assetId, int expectedCount)
        {
            var actionResult = CircuitPlayerController.GetRelatedMediaAssets(assetId, false);

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<RelatedAssetViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.Equal(result.Count, expectedCount);
        }

        [Theory]
        [InlineData(182613, 2, Constants.JsOlyGaPaulZimmer2)]
        [InlineData(1, 0, Constants.JsOlyGaFake)]
        public void TestGetGalleryByAssetId(int galleryAssetId, int limitImages, string expectedJson)
        {
            var actionResult = CircuitPlayerController.GetGalleryByAssetId(galleryAssetId, limitImages, false);

            if (expectedJson == null)//invalid data
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }

            var expectedResult = JsonConvert.DeserializeObject<GalleryAssetViewModel>(expectedJson);
            var contentResult = actionResult as OkNegotiatedContentResult<GalleryAssetViewModel>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.Equal(JsonConvert.SerializeObject(result.Images.OrderBy(x => x.AssetId)),
                JsonConvert.SerializeObject(expectedResult.Images.OrderBy(x => x.AssetId)));
        }

        [Theory]
        [InlineData(225937, "Sudamericano Individual")]
        [InlineData(1, null)]
        public void TestGetHtmlByNodeId(int nodeId, string expectedJson)
        {
            var actionResult = CircuitPlayerController.GetHtmlByNodeId(nodeId, false);

            if (expectedJson == null)//invalid nodeId
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }

            var contentResult = actionResult as OkNegotiatedContentResult<HtmlViewModel>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotNull(result);
            Assert.NotNull(result.html);
            Assert.True(result.html.Contains(expectedJson));
        }

        #endregion
    }
}
