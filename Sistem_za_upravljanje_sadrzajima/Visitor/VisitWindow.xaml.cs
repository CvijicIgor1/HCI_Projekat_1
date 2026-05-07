using Sistem_za_upravljanje_sadrzajima.Modeli;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sistem_za_upravljanje_sadrzajima
{
    /// <summary>
    /// Interaction logic for VisitWindow.xaml
    /// </summary>
    public partial class VisitWindow : Window
    {
        private Artist _artist;
        public VisitWindow(Artist artist)
        {
            InitializeComponent();
            _artist = artist;
            FillFields();
        }
        private void FillFields()
        {
            ArtistNameLabel.Content = _artist.ArtistName;
            SongTitleLabel.Content = _artist.TitleOfSong;
            GradeOfFansLabel.Content = _artist.GradeOfFan.ToString();

            if (_artist.ImageSource != null)
            {
                PreviewImageImage.Source = _artist.ImageSource;
            }

            if (!string.IsNullOrEmpty(_artist.RtfPath) && File.Exists(_artist.RtfPath))
            {
                TextRange range = new TextRange(ReviewDocumentViewer.Document.ContentStart, ReviewDocumentViewer.Document.ContentEnd);
                using (FileStream fs = new FileStream(_artist.RtfPath, FileMode.Open))
                {
                    range.Load(fs, DataFormats.Rtf);
                }
            }
        }

    }
}
