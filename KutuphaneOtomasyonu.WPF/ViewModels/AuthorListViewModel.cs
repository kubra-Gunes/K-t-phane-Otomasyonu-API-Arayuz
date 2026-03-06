// KutuphaneOtomasyonu.WPF.ViewModels/AuthorListViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // ICommand için
using KutuphaneOtomasyonu.WPF.Commands; // AsyncRelayCommand için
using KutuphaneOtomasyonu.WPF.Helpers; // ObservableObject için
using KutuphaneOtomasyonu.WPF.Models; // AuthorDto için
using KutuphaneOtomasyonu.WPF.Services; // AuthorService için


namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class AuthorListViewModel : ObservableObject
    {
        private readonly AuthorService _authorService;
        private ObservableCollection<AuthorDto> _authors;
        private readonly Action<BookDto> _showBookDetailsCallback;
        public ObservableCollection<AuthorDto> Authors
        {
            get => _authors;
            set => SetProperty(ref _authors, value);
        }

        private AuthorDto _selectedAuthor;
        public AuthorDto SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                if (SetProperty(ref _selectedAuthor, value))
                {
                    if (_selectedAuthor != null)
                    {
                       LoadBooksBySelectedAuthorCommand.Execute(_selectedAuthor.YazarId);
                    }
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    LoadAuthorsCommand.Execute(null);
                }
            }
        }


        public ICommand LoadAuthorsCommand { get; }
        public ICommand LoadBooksBySelectedAuthorCommand { get; } // Yeni komut




        // Constructor güncellendi
        private readonly Func<int, Action<BookDto>, Task> _onAuthorSelected; // Tipini güncelledik

        // Constructor güncellendi: Artık ShowBookDetails callback'ini de almalı
        public AuthorListViewModel(Func<int, Action<BookDto>, Task> onAuthorSelectedCallback, Action<BookDto> showBookDetailsCallback)
        {
            _authorService = new AuthorService();
            Authors = new ObservableCollection<AuthorDto>();
            LoadAuthorsCommand = new AsyncRelayCommand(LoadAuthors);

            // Yeni callback'leri ata
            _onAuthorSelected = onAuthorSelectedCallback;
            _showBookDetailsCallback = showBookDetailsCallback;

            LoadBooksBySelectedAuthorCommand = new AsyncRelayCommand<int>(async (authorId) =>
                // MainViewModel'a hem yazar ID'sini hem de detay açma callback'ini iletiyoruz
                await _onAuthorSelected(authorId, _showBookDetailsCallback)
            );

            LoadAuthorsCommand.Execute(null);
        }

        public async Task LoadAuthors()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var authors = await _authorService.GetAuthorsAsync(SearchText);
                var sortedAuthors = authors.OrderBy(a => a.TamAd).ToList();
                Authors.Clear();
                foreach (var author in sortedAuthors)
                {
                    Authors.Add(author);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Yazarlar yüklenirken hata oluştu: {ex.Message}";
                Console.WriteLine($"Yazarlar yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}