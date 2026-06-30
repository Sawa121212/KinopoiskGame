namespace Game.Ui.Views;

using Common.Core.Views;
using FilmsDB.Domain.Models;
using Prism.Regions;
using ReactiveUI;
using System.Net;
using Avalonia.Media.Imaging;
using System.Windows.Input;
using Prism.Commands;

public class TestFilmViewModel : NavigationViewModelBase
{
    /// <inheritdoc />
    public TestFilmViewModel(IRegionManager regionManager)
        : base(regionManager)
    {
        ShowRatingCommand = new DelegateCommand(OnShowRating);
        ShowNextFilmCommand = new DelegateCommand(async () => await OnShowNextFilm());

        Film = new Film()
        {
            Name = "Артур, ты король",
            Year = 2024,
            PosterUrl = "https://image.openmoviedb.com/kinopoisk-images/10809116/02f5c9fa-8ed2-455b-9861-1a01252d73f4/orig",
            Rating = 8.301
        };

        //Task.Run(async () => await DownloadImage(Film.PosterUrl));
    }

    /// <summary>
    ///  Постер
    /// </summary>
    public Bitmap? PosterBitmap
    {
        get => _posterBitmap;
        set => this.RaiseAndSetIfChanged(ref _posterBitmap, value);
    }

    private Bitmap? _posterBitmap;

    /// <summary>
    /// Фильм
    /// </summary>
    public Film Film
    {
        get => _film;
        set => this.RaiseAndSetIfChanged(ref _film, value);
    }
    private Film _film;

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
        catch (Exception exception)
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

        // test
        Random rnd = new();
        int value = rnd.Next(0, 10);

        Film = new Film()
        {
            Name = $"Test 'Артур, ты король'",
            Year = value,
            PosterUrl = "https://image.openmoviedb.com/kinopoisk-images/10809116/02f5c9fa-8ed2-455b-9861-1a01252d73f4/orig",
            Rating = value
        };

        await DownloadImage(Film.PosterUrl).ConfigureAwait(true);
    }
}