using System.Collections.Generic;
using ITF.DataServices.SDK.Models.ViewModels;
using ITF.DataServices.SDK.Models.ViewModels.Cms;

namespace ITF.DataServices.SDK.Interfaces
{
    public interface ICmsService
    {
        ICollection<TicketsInfoViewModel> GetCmsTicketsInfo(Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        TieRelatedAssetsViewModel GetCmsTieRelatedAssets(string publicTieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<CmsPhotoGalleriesViewModel> GetCmsPhotoGalleries(int nodeId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        NationProfileWebViewModelOld GetCmsNationProfileWeb(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        CmsPhotoGalleryViewModel GetCmsPhotoGallery(int nodeId, int top = 0,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        HtmlComponentViewModel GetCmsHtmlComponent(int nodeId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<SearchContentViewModel> GetSearchCmsContent(int nodeId, string search,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
    }
}
