using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Sistem_za_upravljanje_sadrzajima.Modeli
{
    [Serializable]
    public class Artist
    {
        public bool isSelected { get; set; }   // ovo za checkbox
        public string ArtistName { get; set; } // hyperlink
        public string TitleOfSong { get; set; } // tekst
        public int GradeOfFan { get; set; } // broj-OcenaFana (koji korisnik unosi)
        public string ImagePath { get; set; }  // slika
        public string RtfPath { get; set; }    // putanja do .rtf
        public DateTime DateAdded { get; set; } // datum trenutni

        public BitmapImage ImageSource
        {
            get
            {
                if (string.IsNullOrEmpty(ImagePath)) return null;
                string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ImagePath);
                if (!File.Exists(absolutePath)) return null;
                return new BitmapImage(new Uri(absolutePath));
            }
        }
        public Artist()
        {
            DateAdded = DateTime.Now;
            isSelected = false;
        }

        public Artist(string artistName,string titleOfSong,int grade, string imagePath,string rtfPath)
        {
            ArtistName = artistName;
            TitleOfSong = titleOfSong;
            GradeOfFan = grade;
            ImagePath = imagePath;
            RtfPath = rtfPath;
            DateAdded = DateTime.Now;
            isSelected = false;
        }
    }
}