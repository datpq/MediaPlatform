using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using ITF.DataServices.SDK.Models.ViewModels.Circuits;
using ITF.DataServices.SDK.Models.ViewModels.Cms;
using Xunit;

namespace ITF.MediaPlatform.API.Tests
{
    public class VariousTest : BaseTest
    {
        public VariousTest(IisExpressFixture iisExpressFixture) : base(iisExpressFixture)
        {
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("es", "fc")]
        public void TestCmsTicketsInfo(string language, string source)
        {
            var actionResult = CmsController.GetCmsTicketsInfo(language, source, false);
            var actionName = ((ActionNameAttribute)CmsController.GetType().GetMethod("GetCmsTicketsInfo").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<TicketsInfoViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotEmpty(result);
            result.ToList().ForEach(x =>
            {
                Assert.NotNull(x.Event);
                Assert.NotNull(x.Ties);
                Assert.NotEmpty(x.Ties);
                Assert.True(x.Ties.All(y => !string.IsNullOrEmpty(y.Side1Nation) && !string.IsNullOrEmpty(y.Side2Nation)
                && !string.IsNullOrEmpty(y.Side1NationCode) && !string.IsNullOrEmpty(y.Side2NationCode)));
            });

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source);
        }

        [Theory]
        [InlineData("INV", null, null, -1, -1)]
        [InlineData("M-DC-2015-WG-M-GBR-BEL-01", "es", null, 7, 299)]
        [InlineData("W-FC-2010-WG-M-ITA-USA-01", "en", "fc", 2, 65)]
        [InlineData("M-DC-2016-G1-AM-REL-BAR-DOM-01", "en", null, null, null)]
        public void TestCmsTieRelatedAssets(string publicTieId, string language, string source, int? expectedArticles, int? expectedPhotos)
        {
            var actionResult = CmsController.GetCmsTieRelatedAssets(publicTieId, language, source, false);
            var actionName = ((ActionNameAttribute)CmsController.GetType().GetMethod("GetCmsTieRelatedAssets").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedArticles < 0 && expectedPhotos < 0)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<TieRelatedAssetsViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            var culture = result.Cultures.FirstOrDefault(x => x.CultureCode == language);
            Assert.NotNull(culture);

            if (expectedArticles == null)
            {
                Assert.Null(culture.Articles);
            }
            else
            {
                Assert.Equal(culture.Articles.Count, expectedArticles);
                Assert.True(culture.Articles.All(x => x.Id != 0 && x.ImageId != null && !string.IsNullOrEmpty(x.Title) && !string.IsNullOrEmpty(x.Summary)));
            }
            if (expectedPhotos == null)
            {
                Assert.Null(culture.Photos);
            }
            else
            {
                Assert.Equal(culture.Photos.Count, expectedPhotos);
                Assert.True(culture.Photos.All(x => x.ImageId != 0 && !string.IsNullOrEmpty(x.Desc)));
            }

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, publicTieId);
        }

        [Theory]
        [InlineData(1, null, null, 0)]
        [InlineData(2035, "es", null, 100)]
        public void TestCmsPhotoGalleries(int nodeId, string language, string source, int expectedCount)
        {
            var actionResult = CmsController.GetCmsPhotoGalleries(nodeId, language, source, false);
            var actionName = ((ActionNameAttribute)CmsController.GetType().GetMethod("GetCmsPhotoGalleries").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedCount == 0)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<CmsPhotoGalleriesViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotEmpty(result);
            Assert.True(result.Count > expectedCount);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nodeId.ToString());
        }

        [Theory]
        [InlineData(1, "murray", null, null, 0)]
        [InlineData(2035, "murray", "es", null, 20)]
        public void TestSearchCmsContent(int nodeId, string search, string language, string source, int expectedCount)
        {
            var actionResult = CmsController.GetSearchCmsContent(nodeId, search, language, source, false);
            var actionName = ((ActionNameAttribute)CmsController.GetType().GetMethod("GetSearchCmsContent").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedCount == 0)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<SearchContentViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            Assert.NotEmpty(result);
            Assert.Equal(result.Count, expectedCount);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nodeId.ToString(), search);
        }

        [Theory]
        [InlineData(1, null, null, 0)]
        [InlineData(241348, "es", "fc", 4)]
        [InlineData(241348, null, null, 2)]
        public void TestCmsPhotoGallery(int nodeId, string language, string source, int expectedCount)
        {
            var actionResult = CmsController.GetCmsPhotoGallery(nodeId, expectedCount, language, source, false);
            var actionName = ((ActionNameAttribute)CmsController.GetType().GetMethod("GetCmsPhotoGallery").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedCount == 0)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<CmsPhotoGalleryViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            Assert.Equal(result.GalleryId, nodeId);
            Assert.True(!string.IsNullOrEmpty(result.Title));
            Assert.Equal(result.Photos.Count, expectedCount);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nodeId.ToString(), expectedCount.ToString());
        }

        [Theory]
        [InlineData(1, null, null, null)]
        [InlineData(239207, null, null, "Davis Cup Survey 2016")]
        public void TestCmsHtmlComponent(int nodeId, string language, string source, string expectedTitle)
        {
            var actionResult = CmsController.GetCmsHtmlComponent(nodeId, language, source, false);
            var actionName = ((ActionNameAttribute)CmsController.GetType().GetMethod("GetCmsHtmlComponent").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedTitle == null)//invalid parameters
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }
            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<HtmlComponentViewModel>>;
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.Content.Count, 1);
            var result = contentResult.Content.FirstOrDefault();
            Assert.NotNull(result);

            Assert.True(!string.IsNullOrEmpty(result.Title));
            Assert.True(!string.IsNullOrEmpty(result.Html));
            Assert.Equal(result.Title, expectedTitle);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source, nodeId.ToString());
        }

        [Theory]
        [InlineData(1896, "GRE", 7, 22, null, "itf")]
        [InlineData(1992, "ESP", 62, 482, null, "itf")]
        public void TestGetOlympicTennis(int expectedYear, string expectedHostNationCode, int expectedRoundsTotal, int expectedMatchesTotal,
            string language, string source)
        {
            var actionResult = CircuitController.GetOlympicTennis(language, source, false);
            var actionName = ((ActionNameAttribute)CircuitController.GetType().GetMethod("GetOlympicTennis").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<OlympicTennisViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            var olympic = result.FirstOrDefault(x => x.Year == expectedYear);
            Assert.NotNull(olympic);
            Assert.Equal(olympic.HostNationCode, expectedHostNationCode);
            Assert.Equal(olympic.Events.SelectMany(x => x.Rounds).Count(), expectedRoundsTotal);
            Assert.Equal(olympic.Events.SelectMany(x => x.Rounds).SelectMany(x => x.Matches).Count(), expectedMatchesTotal);

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, language, source);
        }

        [Theory]
        [InlineData("INV", 0, null, 0)]
        [InlineData("FRA", 1925, "7-5 6-1 6-4", 100151319)]
        public void TestGetGrandSlam(string nationCode, int expectedYear, string expectedScore, int expectedPlayerId)
        {
            var actionResult = CircuitController.GetGrandSlam(nationCode, useCache: false);
            var actionName = ((ActionNameAttribute)CircuitController.GetType().GetMethod("GetGrandSlam").GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;

            if (expectedYear == 0)
            {
                Assert.IsType(typeof(NotFoundResult), actionResult);
                return;
            }

            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<GrandSlamViewModel>>;
            Assert.NotNull(contentResult);
            var result = contentResult.Content;

            var tournament = result.FirstOrDefault(x => x.Year == expectedYear);
            Assert.NotNull(tournament);
            Assert.NotNull(tournament.Events.FirstOrDefault(x => x.FinalScore == expectedScore));
            Assert.NotNull(tournament.Events.SelectMany(x => x.Players).FirstOrDefault(x => x.PlayerId == expectedPlayerId));

            IisExpressFixture.CallWebApiInThreadPool(contentResult.Content, actionName, nationCode);
        }
    }
}
