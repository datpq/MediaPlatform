using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using ITF.DataServices.Authentication.Services;
using ITF.DataServices.SDK.Data;
using ITF.DataServices.SDK.Interfaces;
using ITF.DataServices.SDK.Models;
using ITF.DataServices.SDK.Models.Cms;
using ITF.DataServices.SDK.Models.ViewModels;
using ITF.DataServices.SDK.Models.ViewModels.Cms;
using Ninject;
using NLog;

namespace ITF.DataServices.SDK.Services
{
    public class CmsService : CupService, ICmsService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public CmsService(
            [Named("DavisCupRepo")] ICupDataRepository davisCupRepo,
            [Named("FedCupRepo")] ICupDataRepository fedCupRepo,
            [Named("CmsRepo")] ISameStructureDataRepository cmsRepo,
            ICacheConfigurationService cacheConfigurationService)
            : base(davisCupRepo, fedCupRepo, cmsRepo, cacheConfigurationService)
        {
        }

        public ICollection<TicketsInfoViewModel> GetCmsTicketsInfo
            (Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<TicketsInfoViewModel>;
                    if (cacheValue != null)
                    {
                        return cacheValue;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var repo = CmsRepo;

                var lResult = (from t in repo.GetManySpecific<CupTickets>(language, source, x => x.Display.Value)
                               group t by new
                               {
                                   t.ID,
                                   t.Title
                               } into g
                               orderby g.Key.ID
                               select new
                               {
                                   Event = g.Key.Title,
                                   Ties = from gl in g.ToList()
                                          select new
                                          {
                                              TieId = gl.TieID,
                                              gl.PublicTieId,
                                              Side1Nation = gl.NationName1,
                                              Side1NationCode = gl.Nation1,
                                              Side2Nation = gl.NationName2,
                                              Side2NationCode = gl.Nation2,
                                              OnSaleDate = gl.Tickets,
                                              gl.Website,
                                              Telephon = gl.Tel,
                                              Price = gl.Prices
                                          }
                               }).ToList();

                var result = Mapper.Map<ICollection<TicketsInfoViewModel>>(lResult);

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;

            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public TieRelatedAssetsViewModel GetCmsTieRelatedAssets(string publicTieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{publicTieId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as TieRelatedAssetsViewModel;
                    if (cacheValue != null)
                    {
                        return cacheValue;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var repo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;

                var tieId = repo.GetMany<Tie>(x => x.PublicTieId == publicTieId).FirstOrDefault()?.TieID;
                if (tieId == null)
                {
                    Logger.Warn($"Tie not found: publicTieId={publicTieId}");
                    return null;
                }

                IEnumerable<int> articleIds;
                var articles = GetArticles(tieId.Value, out articleIds);

                var lResult = (from atc in articles
                               let photos = GetImageInfos(AssetTypeImage, AssetTypeTie, null, tieId, atc.CultureCode)
                               orderby atc.CultureCode
                               select new CultureViewModel
                               {
                                   CultureCode = atc.CultureCode,
                                   Report = GetItfBaselineContents(CmsRepo, tieId.Value, AssetTypeTie, "info", atc.CultureCode),
                                   TV = GetItfBaselineContents(CmsRepo, tieId.Value, AssetTypeTie, "tv", atc.CultureCode),
                                   CourtPaceRating = GetItfBaselineContents(CmsRepo, tieId.Value, AssetTypeTie, "courtpace", atc.CultureCode),
                                   Articles = articleIds.Any() ? (atc.Articles == null || !atc.Articles.Any() ? null : atc.Articles) : null,
                                   Photos = photos == null || photos.Any() ? photos : null
                               }).ToList();

                var result = new TieRelatedAssetsViewModel
                {
                    InternalId = tieId.Value,
                    PublicId = publicTieId,
                    Cultures = lResult
                };

                result.Cultures.Where(x => x.Articles != null).SelectMany(x => x.Articles).Where(x => x.ImageId != null).ToList()
                    .ForEach(x => { x.Media = $"{x.ImageId}/{x.ImageId}.jpg"; });

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public ICollection<CmsPhotoGalleriesViewModel> GetCmsPhotoGalleries(int nodeId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nodeId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<CmsPhotoGalleriesViewModel>;
                    if (cacheValue != null)
                    {
                        return cacheValue;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var repo = CmsRepo;
                var photoIds = repo.GetMany<ITFGallery>(x => x.ITFWebScopes.Any(y => y.webScopeNodeId == nodeId))
                    .Where(x => x.NodeId.HasValue).Select(x => x.NodeId.Value).ToList();

                if (!photoIds.Any()) return null;

                //var photoResults = (from t in repo.GetMany<ITFRelationships>(x => x.AssetType.assetTypeName == AssetTypeGallery
                //                    && x.RelatedAssetType.assetTypeName == AssetTypeImage && photoIds.Contains(x.assetId.Value))
                //                    group t by t.assetId into g
                //                    select g.OrderBy(x => x.sortOrder).ThenByDescending(x => x.relationshipId).FirstOrDefault()).ToList();

                var photoResults = (from t in GetItfRelationShipsByIds(AssetTypeGallery, AssetTypeImage, photoIds)
                                    group t by t.assetId into g
                                    select g.OrderBy(x => x.sortOrder).ThenByDescending(x => x.relationshipId).FirstOrDefault()).ToList();

                var lResult = (from t in repo.GetMany<ITFGallery>(x => x.ITFWebScopes.Any(y => y.webScopeNodeId == nodeId))
                               //join rs in GetItfRelationShipsByIds(AssetTypeGallery, AssetTypeImage, photoIds) on t.NodeId equals rs.assetId into imgs
                               join rs in photoResults on t.NodeId equals rs.assetId
                               let dt = t.LastUpdated < t.CreatedDate ? t.CreatedDate : t.LastUpdated
                               orderby t.CreatedDate descending
                               select new
                               {
                                   GalleryId = t.NodeId,
                                   Title = language == Language.En ? t.Name : t.DescriptionText,
                                   dt.Value.Date,
                                   //Date = dt.FormatMediumByLanguage(language),
                                   //PhotoId = GetImageId(AssetTypeGallery, t.NodeId)
                                   //PhotoId = imgs.OrderBy(x => x.sortOrder).ThenByDescending(x => x.relationshipId).FirstOrDefault()?.relatedAssetId
                                   PhotoId = rs.relatedAssetId
                               }).ToList();

                var result = Mapper.Map<ICollection<CmsPhotoGalleriesViewModel>>(lResult);

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;

            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public CmsPhotoGalleryViewModel GetCmsPhotoGallery(int nodeId, int top = 0,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nodeId}.{top}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as CmsPhotoGalleryViewModel;
                    if (cacheValue != null)
                    {
                        return cacheValue;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var lResult = (from t in CmsRepo.GetMany<ITFGallery>(x => x.NodeId == nodeId)
                               let dt = t.LastUpdated < t.CreatedDate ? t.CreatedDate : t.LastUpdated
                               orderby t.CreatedDate descending
                               select new
                               {
                                   GalleryId = t.NodeId,
                                   Title = language == Language.En ? t.Name : t.DescriptionText,
                                   dt.Value.Date,
                                   //Date = dt.FormatMediumByLanguage(language)
                                   Photos = GetPhotoInfos(AssetTypeGallery, AssetTypeImage, t.NodeId, null, language.ToString())
                               }).FirstOrDefault();

                if (lResult == null)
                {
                    Logger.Warn($"Node not found: nodeId={nodeId}");
                    return null;
                }

                var result = Mapper.Map<CmsPhotoGalleryViewModel>(lResult);
                if (top > 0)
                {
                    result.Photos = result.Photos.Take(top).ToList();
                }

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public NationProfileWebViewModelOld GetCmsNationProfileWeb(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nationCode}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as NationProfileWebViewModelOld;
                    if (cacheValue != null)
                    {
                        return cacheValue;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var baseLineId = CmsRepo.Get<ITFNationBaseline>(x => x.NationCode == nationCode)?.NationId;
                if (baseLineId == null)
                {
                    Logger.Warn($"Nation not found: nationCode={nationCode}");
                    return null;
                }

                var lResult = (from con in CmsRepo.GetMany<ITFBaselineContent>(x => x.baselineId == baseLineId && x.AssetType.assetTypeName == AssetTypeNation && x.websiteCode == source.ToString())
                               group con by 1 into g
                               select new
                               {
                                   Captain = g.FirstOrDefault(x => x.contentType == "captainName")?.content,
                                   CaptainId = g.FirstOrDefault(x => x.contentType == "captainId")?.content,
                                   NextTieDate = g.FirstOrDefault((x => x.contentType == "nextTieDate" && x.cultureCode == language.ToString().ToLower()))?.content,
                                   NextTieDateEN = g.FirstOrDefault((x => x.contentType == "nextTieDate" && x.cultureCode == Language.En.ToString().ToLower()))?.content,
                                   NextTieDateES = g.FirstOrDefault((x => x.contentType == "nextTieDate" && x.cultureCode == Language.Es.ToString().ToLower()))?.content,
                                   NextTieId = g.FirstOrDefault((x => x.contentType == "nextTieId" && x.cultureCode == language.ToString().ToLower()))?.content,
                                   NextTieIdEN = g.FirstOrDefault((x => x.contentType == "nextTieId" && x.cultureCode == Language.En.ToString().ToLower()))?.content,
                                   NextTieIdES = g.FirstOrDefault((x => x.contentType == "nextTieId" && x.cultureCode == Language.Es.ToString().ToLower()))?.content,
                                   NextTieDesc = g.FirstOrDefault((x => x.contentType == "nextTieDesc" && x.cultureCode == language.ToString().ToLower()))?.content,
                                   NextTieDescEN = g.FirstOrDefault((x => x.contentType == "nextTieDesc" && x.cultureCode == Language.En.ToString().ToLower()))?.content,
                                   NextTieDescES = g.FirstOrDefault((x => x.contentType == "nextTieDesc" && x.cultureCode == Language.Es.ToString().ToLower()))?.content,
                                   LastTieDate = g.FirstOrDefault((x => x.contentType == "lastTieDate" && x.cultureCode == language.ToString().ToLower()))?.content,
                                   LastTieDateEN = g.FirstOrDefault((x => x.contentType == "lastTieDate" && x.cultureCode == Language.En.ToString().ToLower()))?.content,
                                   LastTieDateES = g.FirstOrDefault((x => x.contentType == "lastTieDate" && x.cultureCode == Language.Es.ToString().ToLower()))?.content,
                                   LastTieId = g.FirstOrDefault((x => x.contentType == "lastTieId" && x.cultureCode == language.ToString().ToLower()))?.content,
                                   LastTieIdEN = g.FirstOrDefault((x => x.contentType == "lastTieId" && x.cultureCode == Language.En.ToString().ToLower()))?.content,
                                   LastTieIdES = g.FirstOrDefault((x => x.contentType == "lastTieId" && x.cultureCode == Language.Es.ToString().ToLower()))?.content,
                                   LastTieDesc = g.FirstOrDefault((x => x.contentType == "lastTieDesc" && x.cultureCode == language.ToString().ToLower()))?.content,
                                   LastTieDescEN = g.FirstOrDefault((x => x.contentType == "lastTieDesc" && x.cultureCode == Language.En.ToString().ToLower()))?.content,
                                   LastTieDescES = g.FirstOrDefault((x => x.contentType == "lastTieDesc" && x.cultureCode == Language.Es.ToString().ToLower()))?.content,
                                   History = g.FirstOrDefault((x => x.contentType == "history" && x.cultureCode == language.ToString().ToLower()))?.content,
                                   HistoryEN = g.FirstOrDefault((x => x.contentType == "history" && x.cultureCode == Language.En.ToString().ToLower()))?.content,
                                   HistoryES = g.FirstOrDefault((x => x.contentType == "history" && x.cultureCode == Language.Es.ToString().ToLower()))?.content,
                                   Record = g.FirstOrDefault((x => x.contentType == "record" && x.cultureCode == language.ToString().ToLower()))?.content,
                                   RecordEN = g.FirstOrDefault((x => x.contentType == "record" && x.cultureCode == Language.En.ToString().ToLower()))?.content,
                                   RecordES = g.FirstOrDefault((x => x.contentType == "record" && x.cultureCode == Language.Es.ToString().ToLower()))?.content
                               }).FirstOrDefault();

                if (lResult == null)
                {
                    Logger.Warn($"Nation not found: nationCode={nationCode}");
                    return null;
                }

                var result = Mapper.Map<NationProfileWebViewModelOld>(lResult);

                var cupRepo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;

                result.NextTiePlayStatusCode = (from tie in cupRepo.GetMany<Tie>(x => x.PublicTieId == result.NextTieId)
                    select tie.PlayStatusCode).FirstOrDefault();

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public HtmlComponentViewModel GetCmsHtmlComponent(int nodeId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nodeId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as HtmlComponentViewModel;
                    if (cacheValue != null)
                    {
                        return cacheValue;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var lResult = (from t in CmsRepo.GetMany<ITFHtml>(x => x.NodeId == nodeId)
                               select new {t.Title, Html = t.HTML}).FirstOrDefault();

                if (lResult == null)
                {
                    Logger.Warn($"Node not found: nodeId={nodeId}");
                    return null;
                }

                var result = Mapper.Map<HtmlComponentViewModel>(lResult);

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public ICollection<SearchContentViewModel> GetSearchCmsContent(int nodeId, string search,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (string.IsNullOrEmpty(search)) throw new ArgumentException("search string must not be empty");
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nodeId}.{search}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<SearchContentViewModel>;
                    if (cacheValue != null)
                    {
                        return cacheValue;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var repo = CmsRepo;
                const int maxReturnedRows = 20;

                var contents = repo.GetMany<ITFContent>(x => x.published.Value && x.languageRootNodeId == nodeId && (x.title.Contains(search) || x.body.Contains(search)))
                    .OrderByDescending(x => x.dateStamp).Take(maxReturnedRows).ToList();
                var contentNodeIds = contents.Select(x => x.nodeId);
                var nodeIds = repo.GetMany<umbracoNode>(x => x.text.Contains(search)).Select(x => x.id).ToList();

                var lResultContent = (from t in contents //repo.GetMany<ITFContent>(x => x.published.Value && x.languageRootNodeId == nodeId && (x.title.Contains(search) || x.body.Contains(search)))
                                      join u in repo.GetMany<umbracoNode>(x => contentNodeIds.Contains(x.id)) on t.nodeId equals u.id
                                      orderby t.dateStamp descending
                                      select new
                                      {
                                          ContentId = t.nodeId,
                                          Title = t.title,
                                          Summary = t.summary,
                                          Date = t.dateStamp,
                                          ImageId = GetImageId(AssetTypeMatchReport, t.nodeId)
                                      }).Take(maxReturnedRows).ToList();
                var lResultNode = (from uId in nodeIds //repo.GetMany<umbracoNode>(x => x.text.Contains(search))
                                   join t in repo.GetMany<ITFContent>(x => x.published.Value && x.languageRootNodeId == nodeId && nodeIds.Contains(x.nodeId)) on uId equals t.nodeId
                                   orderby t.dateStamp descending
                                   select new
                                   {
                                       ContentId = t.nodeId,
                                       Title = t.title,
                                       Summary = t.summary,
                                       Date = t.dateStamp,
                                       ImageId = GetImageId(AssetTypeMatchReport, t.nodeId)
                                   }).Take(maxReturnedRows).ToList();

                var lResult = lResultContent.Union(lResultNode).OrderByDescending(x => x.Date).Take(maxReturnedRows).ToList();

                var result = Mapper.Map<ICollection<SearchContentViewModel>>(lResult);

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;

            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        #region Static methods

        private ICollection<CultureViewModel> GetArticles(int tieId, out IEnumerable<int> articleIds)
        {
            var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
            var cacheTimeout = GetCacheTimeout(cachePrefix);
            var cacheKey = $"{cachePrefix}.{tieId}";
            var cacheKeyOut = $"{cacheKey}.Out";
            var cacheValue = MemoryCache.Get(cacheKey) as ICollection<CultureViewModel>;
            if (cacheValue != null)
            {
                articleIds = MemoryCache.Get(cacheKeyOut) as IEnumerable<int>;
                return cacheValue;
            }

            var stopWatch = Stopwatch.StartNew();
            var ids = GetItfRelationShips(AssetTypeMatchReport, AssetTypeTie, null, tieId)
                    .Where(x => x.assetId.HasValue)
                    .OrderByDescending(x => x.relationshipId)
                    .Select(x => x.assetId.Value).ToList();
            articleIds = ids;

            var lResult = articleIds.Any() ?
                (from id in articleIds
                 join rs in GetItfRelationShipsByIds(AssetTypeMatchReport, AssetTypeImage, ids) on id equals rs.assetId into imgs
                 join ic in CmsRepo.GetMany<ITFContent>(x => x.published.Value && ids.Contains(x.nodeId)) on id equals ic.nodeId
                 group new
                 {
                     Articles = new ArticleViewModel
                     {
                         Id = id,
                         Title = ic.title,
                         Summary = ic.summary,
                         Body = ic.summary,
                         //ImageId = GetImageId(AssetTypeMatchReport, rsT.assetId)
                         ImageId = imgs.OrderBy(x => x.sortOrder).ThenByDescending(x => x.relationshipId).FirstOrDefault()?.relatedAssetId
                     }
                 } by ic.cultureCode into g
                 select new
                 {
                     CultureCode = g.Key,
                     Articles = from gL in g.ToList() select gL.Articles
                 }).ToList() : GetAllCultures().Select(x => new { CultureCode = x, Articles = (IEnumerable<ArticleViewModel>)null });

            var lResultWithCulture = (from c in GetAllCultures()
                                      join r in lResult on c equals r.CultureCode into rL
                                      from r in rL.DefaultIfEmpty() // LEFT JOIN
                                      select new
                                      {
                                          CultureCode = c,
                                          r?.Articles
                                      }).ToList();

            var result = Mapper.Map<ICollection<CultureViewModel>>(lResultWithCulture);

            stopWatch.Stop();
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
            }

            MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));
            MemoryCache.Set(cacheKeyOut, articleIds, DateTimeOffset.Now.AddSeconds(cacheTimeout));

            return result;
        }

        private ICollection<PhotoViewModel> GetPhotoInfos(string assetTypeName, string relatedAssetTypeName,
            int? assetId, int? relatedAssetId, string cultureCode)
        {
            var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
            var cacheTimeout = GetCacheTimeout(cachePrefix);
            var cacheKey = $"{cachePrefix}.{assetTypeName}.{relatedAssetTypeName}.{assetId}.{relatedAssetId}.{cultureCode}";
            var cacheValue = MemoryCache.Get(cacheKey) as ICollection<PhotoViewModel>;
            if (cacheValue != null)
            {
                return cacheValue;
            }

            var stopWatch = Stopwatch.StartNew();
            var photoIds = GetItfRelationShips(assetTypeName, relatedAssetTypeName, assetId, relatedAssetId)
                    .Where(x => x.relatedAssetId.HasValue).Select(x => x.relatedAssetId.Value).ToList();

            if (!photoIds.Any()) return null;

            var lResult = (from rsI in GetItfRelationShips(assetTypeName, relatedAssetTypeName, assetId, relatedAssetId)
                           join med in CmsRepo.GetMany<ITFMediaDescription>(x => x.cultureCode == cultureCode && photoIds.Contains(x.umbracoMediaNodeId.Value)) on rsI.relatedAssetId equals med.umbracoMediaNodeId
                           //from med in CmsRepo.GetMany<ITFMediaDescription>(x => x.cultureCode == cultureCode && x.umbracoMediaNodeId == rsI.relatedAssetId)
                           join rel in GetItfRelationShipsByIds(null, AssetTypePhotographer, photoIds) on rsI.relatedAssetId equals rel.assetId into relL from rel in relL.DefaultIfEmpty() // LEFT JOIN
                           join con in CmsRepo.GetAll<ITFContentProvider>(true) on rel?.relatedAssetId equals con.Id into conL from con in conL.DefaultIfEmpty() // LEFT JOIN
                           orderby rsI.sortOrder
                           select new
                           {
                               PhotoId = rsI.relatedAssetId,
                               Desc = med.altText,
                               Photographer = con?.ContentProvider
                           }).ToList();

            var result = Mapper.Map<ICollection<PhotoViewModel>>(lResult);

            stopWatch.Stop();
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
            }

            MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

            return result;
        }

        private ICollection<ImageViewModel> GetImageInfos(string assetTypeName, string relatedAssetTypeName,
            int? assetId, int? relatedAssetId, string cultureCode)
        {
            var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
            var cacheTimeout = GetCacheTimeout(cachePrefix);
            var cacheKey = $"{cachePrefix}.{assetTypeName}.{relatedAssetTypeName}.{assetId}.{relatedAssetId}.{cultureCode}";
            var cacheValue = MemoryCache.Get(cacheKey) as ICollection<ImageViewModel>;
            if (cacheValue != null)
            {
                return cacheValue;
            }

            var stopWatch = Stopwatch.StartNew();
            var imageIds = GetItfRelationShips(assetTypeName, relatedAssetTypeName, assetId, relatedAssetId)
                    .Where(x => x.assetId.HasValue).Select(x => x.assetId.Value).ToList();

            if (!imageIds.Any()) return null;

            var lResult = (from rsI in GetItfRelationShips(assetTypeName, relatedAssetTypeName, assetId, relatedAssetId)
                           join med in CmsRepo.GetMany<ITFMediaDescription>(x => x.cultureCode == cultureCode && imageIds.Contains(x.umbracoMediaNodeId.Value)) on rsI.assetId equals med.umbracoMediaNodeId
                           //from med in CmsRepo.GetMany<ITFMediaDescription>(x => x.cultureCode == cultureCode && x.umbracoMediaNodeId == rsI.assetId)
                           join rel in GetItfRelationShipsByIds(null, AssetTypePhotographer, imageIds) on rsI.assetId equals rel.assetId
                           join con in CmsRepo.GetAll<ITFContentProvider>(true) on rel.relatedAssetId equals con.Id
                           orderby rsI.relationshipId, rel.relationshipId descending
                           select new
                           {
                               ImageId = rsI.assetId,
                               Desc = med.altText,
                               Photographer = con.ContentProvider
                           }).ToList();

            var result = Mapper.Map<ICollection<ImageViewModel>>(lResult);

            stopWatch.Stop();
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
            }

            MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

            return result;
        }

        private int? GetImageId(string assetTypeName, int? assetId)
        {
            var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
            var cacheTimeout = GetCacheTimeout(cachePrefix);
            var cacheKey = $"{cachePrefix}.{assetTypeName}.{assetId}";
            var cacheValue = MemoryCache.Get(cacheKey) as int?;
            if (cacheValue != null)
            {
                return cacheValue;
            }

            var stopWatch = Stopwatch.StartNew();

            var result = GetItfRelationShips(assetTypeName, AssetTypeImage, assetId)
                .OrderBy(x => x.sortOrder).ThenByDescending(x => x.relationshipId).FirstOrDefault()?.relatedAssetId;

            stopWatch.Stop();
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}, result={result}");
            }

            if (result != null)
            {
                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));
            }

            return result;
        }

        private IEnumerable<ITFRelationships> GetItfRelationShips(
            string assetTypeName = null, string relatedAssetTypeName = null, int? assetId = null, int? relatedAssetId = null)
        {
            if (!assetId.HasValue && !relatedAssetId.HasValue)
            {
                Logger.Warn($"STOP DOING this heavy query: {assetTypeName}.{relatedAssetTypeName}");
            }
            var result = CmsRepo.GetMany<ITFRelationships>(
                x => (assetTypeName == null || x.AssetType.assetTypeName == assetTypeName)
                && (relatedAssetTypeName == null || x.RelatedAssetType.assetTypeName == relatedAssetTypeName)
                && (assetId == null || x.assetId == assetId) && (relatedAssetId == null || x.relatedAssetId == relatedAssetId));
            return result;
        }

        private IEnumerable<ITFRelationships> GetItfRelationShipsByIds(
            string assetTypeName = null, string relatedAssetTypeName = null, ICollection<int> assetIds = null, ICollection<int> relatedAssetIds = null)
        {
            if (assetIds == null)
            {
                assetIds = new List<int>();
            }
            var ignoreAssetIds = !assetIds.Any();
            if (relatedAssetIds == null)
            {
                relatedAssetIds = new List<int>();
            }
            var ignoreRelatedAssetIds = !relatedAssetIds.Any();
            if (ignoreAssetIds && ignoreRelatedAssetIds)
            {
                Logger.Warn($"STOP DOING this heavy query: {assetTypeName}.{relatedAssetTypeName}");
            }

            var result = CmsRepo.GetMany<ITFRelationships>(
                x => (assetTypeName == null || x.AssetType.assetTypeName == assetTypeName)
                && (relatedAssetTypeName == null || x.RelatedAssetType.assetTypeName == relatedAssetTypeName)
                && (ignoreAssetIds || assetIds.Contains(x.assetId.Value)) && (ignoreRelatedAssetIds || relatedAssetIds.Contains(x.relatedAssetId.Value)));
            return result;
        }

        public static string GetItfBaselineContents(IDataRepository cmsRepo, int baselineId, string assetTypeName, string contentType, string cultureCode)
        {
            //If there's no content for the specified CultureCode, take the content with no CultureCode
            var result = cmsRepo.Get<ITFBaselineContent>(
                             x => x.baselineId == baselineId && x.AssetType.assetTypeName == assetTypeName
                                  && x.contentType == contentType && x.cultureCode == cultureCode)?.content ??
                         cmsRepo.Get<ITFBaselineContent>(
                             x => x.baselineId == baselineId && x.AssetType.assetTypeName == assetTypeName
                                  && x.contentType == contentType)?.content;
            return result;
        }

        #endregion
    }
}
