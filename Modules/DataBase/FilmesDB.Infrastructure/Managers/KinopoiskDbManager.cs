using FilmsDB.Domain;
using FilmsDB.Infrastructure.Interfaces.Managers;

namespace FilmsDB.Infrastructure.Managers
{
    public class KinopoiskDbManager : IKinopoiskDbManager
    {
        public KinopoiskDbManager()
        {
            _kinopoiskDbContext = new KinopoiskDbContext();
        }

        /// <inheritdoc />
        public KinopoiskDbContext DbContext => _kinopoiskDbContext;

        /// <inheritdoc />
        public bool IsConnected()
        {
            return _kinopoiskDbContext.Database.CanConnect();
        }

        private readonly KinopoiskDbContext _kinopoiskDbContext;
    }
}
