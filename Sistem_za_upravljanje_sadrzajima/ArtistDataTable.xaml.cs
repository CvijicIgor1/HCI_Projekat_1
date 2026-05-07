using Sistem_za_upravljanje_sadrzajima.Enumeracije;
using Sistem_za_upravljanje_sadrzajima.Helper;
using Sistem_za_upravljanje_sadrzajima.Modeli;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Sistem_za_upravljanje_sadrzajima
{
    public partial class ArtistDataTable : Window
    {
        private UserRole _user;
        public ObservableCollection<Artist> ArtistsCollection { get; set; } = new ObservableCollection<Artist>();

        public ArtistDataTable(UserRole user)
        {
            InitializeComponent();
            _user = user;
            this.DataContext = this; 

            if (_user == UserRole.Visitor)
            {
                AddButton.Visibility = Visibility.Collapsed;
                DeleteArtistButton.Visibility = Visibility.Collapsed;
            }

            LoadData();
        }

        private void LoadData()
        {
            DataIO_User io = new DataIO_User();

            if (File.Exists("artists.xml"))
            {
                ArtistList list = io.DeSerializeObject<ArtistList>("artists.xml");
                ArtistsCollection = list.Artists ?? new ObservableCollection<Artist>();
                ArtistsDataGrid.ItemsSource = ArtistsCollection; 
            }
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow mainWindow = new MainWindow();
            mainWindow.ShowDialog();
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddArtistInTable newWindow = new AddArtistInTable(ArtistsCollection);
            newWindow.ShowDialog();
            SaveToXml();
        }

        private void DeleteArtistButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = ArtistsCollection.Where(a => a.isSelected).ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show("Please select at least one artist to delete.");
                return;
            }

            foreach (var artist in selected)
            {
                ArtistsCollection.Remove(artist);
            }

            SaveToXml();
        }

        private void SaveToXml()
        {
            DataIO_User io = new DataIO_User();
            ArtistList list = new ArtistList { Artists = ArtistsCollection };
            io.SerializeObject(list, "artists.xml");
        }

        private void ArtistHyperLinkNewWindow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hyperlink && hyperlink.DataContext is Artist artist)
            {
                if (_user == UserRole.Visitor)
                {
                    VisitWindow readOnlyWindow = new VisitWindow(artist);
                    readOnlyWindow.ShowDialog();
                }
                else
                {
                    EditWindow adminWindow = new EditWindow(artist);
                    adminWindow.ArtistUpdated += (updatedArtist) =>
                    {
                        ArtistsDataGrid.Items.Refresh();
                        updatedArtist.DateAdded = DateTime.Now;
                    };

                    adminWindow.ShowDialog();
                }
            }
        }
    }
}