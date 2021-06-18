using ITF.SDK.DavisCup.Models;

namespace ITF.SDK.DavisCup.Interfaces
{
    public interface IPlayerService
    {
        PlayerModel GetPlayer(int playerId, string token = null);
    }
}
