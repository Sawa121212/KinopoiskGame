using System.Net;
using System.Reflection.Metadata;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using Common.Core.Prism.Regions;
using Common.Core.Views;
using FilmsDB.Domain.Models;
using Game.Domain.Models;
using Prism.Commands;
using Prism.Regions;
using ReactiveUI;

namespace Game.Ui.Views.Pages;

public partial class GameViewModel : NavigationViewModelBase
{
    /// <inheritdoc />
    public GameViewModel(IRegionManager regionManager)
        : base(regionManager)
    {
        ShowRatingCommand = new DelegateCommand(OnShowRating);
        ShowNextFilmCommand = new DelegateCommand(async () => await OnShowNextFilm());
    }

    /// <summary>
    /// Фильм
    /// </summary>
    public Film? Film
    {
        get => _film;
        set => this.RaiseAndSetIfChanged(ref _film, value);
    }
    private Film? _film;

    /// <summary>
    ///  Постер
    /// </summary>
    public Bitmap? PosterBitmap
    {
        get => _posterBitmap;
        set => this.RaiseAndSetIfChanged(ref _posterBitmap, value);
    }
    private Bitmap? _posterBitmap;

    public bool HasShowRating
    {
        get => _hasShowRating;
        set => this.RaiseAndSetIfChanged(ref _hasShowRating, value);
    }
    private bool _hasShowRating;

    public ICommand ShowRatingCommand { get; }

    public ICommand ShowNextFilmCommand { get; }

    private async Task DownloadImage(string url)
    {
        try
        {
            byte[]? bytes = null;

            using (WebClient client = new())
            {
                bytes = await client.DownloadDataTaskAsync(new Uri(url));
            }

            Stream stream = new MemoryStream(bytes);

            PosterBitmap = new Bitmap(stream);
        }
        catch (Exception _)
        {
            PosterBitmap = new Bitmap("Error");
        }
    }

    private void OnShowRating()
    {
        HasShowRating = true;
    }

    private async Task OnShowNextFilm()
    {
        HasShowRating = false;

        if (_filmsEnumerator is null)
        {
            throw new Exception();
        }

        _filmsEnumerator.MoveNext();

        Film = _filmsEnumerator.Current;

        if (Film is null)
        {
            MoveBackCommand.Execute(default);
            return;
        }

        await DownloadImage(Film.PosterUrl).ConfigureAwait(true);
    }

    private void ClearAllParameters()
    {
        _gameMode = null;
        _filmsEnumerator = null;
    }

    private GameMode? _gameMode;
    private IEnumerator<Film>? _filmsEnumerator;
}