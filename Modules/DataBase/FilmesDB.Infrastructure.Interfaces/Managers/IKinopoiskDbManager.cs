using FilmsDB.Domain;

namespace FilmsDB.Infrastructure.Interfaces.Managers
{
    public interface IKinopoiskDbManager
    {
        KinopoiskDbContext DbContext { get; }

        bool IsConnected();
    }
}
