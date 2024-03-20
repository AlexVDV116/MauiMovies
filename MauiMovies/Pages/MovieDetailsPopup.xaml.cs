using CommunityToolkit.Maui.Views;
using MauiMovies.Models;

namespace MauiMovies;
 
public partial class MovieDetailsPopup : Popup
{
    public string Title { get; set; }
 
    public string Description { get; set; }
 
    public string PosterUrl { get; set; }
 
    public List<string> Genres { get; set; } = new();
 
    public double Rating { get; set; }
    
    public MovieDetailsPopup(MovieResult movie, List<Genre> genres)
    {
        Size = new Size(600, 600);
 
        Title = movie.title;
        
        Description = movie.overview;
        
        PosterUrl = movie.poster_path;
 
        Rating = movie.vote_average;
 
        foreach (var id in movie.genre_ids)
        {
            Genres.Add(genres.Where(
                g => g.id == id).Select(g => g.name).FirstOrDefault());
        }
 
        BindingContext = this;
 
        InitializeComponent();
    }
}