using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using MauiMovies.Models;

namespace MauiMovies;
 
public partial class GenreListPopup : Popup
{
    public ObservableCollection<UserGenre> Genres { get; set; }
    
    private bool _selectionHasChanged = false;
 
    public GenreListPopup(List<UserGenre> genres)
    {
        BindingContext = this;
        Genres = new ObservableCollection<UserGenre>(genres);
        ResultWhenUserTapsOutsideOfPopup = _selectionHasChanged;
 
        InitializeComponent();
    }
 
    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectionHasChanged = true;
 
        var selectedItems = e.CurrentSelection;
 
        foreach (var genre in Genres)
        {
            if (selectedItems.Contains(genre))
            {
                genre.Selected = true;
            }
            else
            {
                genre.Selected = false;
            }
        }
    }
    
    private void Button_Clicked(object sender, EventArgs e) => Close(_selectionHasChanged);

}