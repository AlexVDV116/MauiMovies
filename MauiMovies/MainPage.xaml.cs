using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using MauiMovies.Models;

namespace MauiMovies;

public partial class MainPage : ContentPage
{
    // https://www.themoviedb.org/settings/api
    string _apiKey = "9b30d99cee6e2b6b79e2c0e24825b837";
    string _baseUri = "https://api.themoviedb.org/3/";

    string _imageBaseUrl = "https://image.tmdb.org/t/p/w500";

    private TrendingMovies _movieList;

    private GenreList _genres;

    public ObservableCollection<Genre> Genres { get; set; } = new();

    public ObservableCollection<MovieResult> Movies { get; set; } = new();

    public ICommand ChooseGenres => new Command(async () => await ShowGenreList());

    public ICommand ShowMovie => new Command<MovieResult>((movie) => ShowMovieDetails(movie));

    public bool IsLoading { get; set; }

    private readonly HttpClient _httpClient;

    List<UserGenre> _genreList { get; set; } = new();

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
        _httpClient = new HttpClient { BaseAddress = new Uri(_baseUri) };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        IsLoading = true;
        OnPropertyChanged(nameof(IsLoading));
        
        _genres = await _httpClient.GetFromJsonAsync<GenreList>($"genre/movie/list?api_key={_apiKey}&language=en-US");

        _movieList = await _httpClient.GetFromJsonAsync<TrendingMovies>($"trending/movie/week?api_key={_apiKey}&language=en-US");

        foreach (var movie in _movieList.results)
        {
            movie.poster_path = $"{_imageBaseUrl}{movie.poster_path}";
        }

        foreach (var genre in _genres.genres)
        {
            _genreList.Add(new UserGenre
            {
                id = genre.id,
                name = genre.name,
                Selected = false
            });
        }

        LoadFilteredMovies();

        IsLoading = false;
        OnPropertyChanged(nameof(IsLoading));
    }

    private void LoadFilteredMovies()
    {
        Movies.Clear();

        if (_genreList.Any(g => g.Selected))
        {
            var selectedGenreIds = _genreList.Where(g => g.Selected).Select(g => g.id);

            foreach (var movie in _movieList.results)
            {
                if (movie.genre_ids.Any(id => selectedGenreIds.Contains(id)))
                {
                    Movies.Add(movie);
                }
            }
        }
        else
        {
            foreach (var movie in _movieList.results)
            {
                Movies.Add(movie);
            }
        }

        OnPropertyChanged(nameof(Movies));
    }

    private async Task ShowGenreList()
    {
        var genrePopup = new GenreListPopup(_genreList);

        var selected = await this.ShowPopupAsync(genrePopup);
        if ((bool)selected)
        {
            Genres.Clear();
            foreach (var genre in _genreList)
            {
                if (genre.Selected)
                {
                    Genres.Add(new Genre
                    {
                        name = genre.name
                    });
                }
            }

            LoadFilteredMovies();
        }
    }
    
    private void ShowMovieDetails(MovieResult movie)
    {
        var moviePopup = new MovieDetailsPopup(movie, _genres.genres);
 
        this.ShowPopup(moviePopup);
    }
}
