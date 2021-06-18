using ITF.DataServices.SDK.Models.Media;

namespace ITF.DataServices.SDK.Interfaces
{
    public interface IInstagramService
    {
        Instagram GetProfileData(string profile, bool useCache = true);
    }
}
