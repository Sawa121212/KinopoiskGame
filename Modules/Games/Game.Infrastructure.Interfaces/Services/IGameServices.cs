using Game.Domain.Models;

namespace Game.Infrastructure.Interfaces.Services
{
    public interface IGameServices
    {
        public IEnumerable<GameMode> CreateGameModes();
    }
}