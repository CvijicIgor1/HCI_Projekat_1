using Sistem_za_upravljanje_sadrzajima.Helper;
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
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private Artist _artist;
        public event Action<Artist> ArtistUpdated;
        public EditWindow(Artist artist)
        {
            InitializeComponent();
            _artist = artist;
            FillForm();
            LoadSystemColors();
            LoadFontFamilies();
            LoadFontSizes();
        }
        private bool ValidateForm()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(EditArtistNameTextBox.Text))
            {
                isValid = false;
                EditArtistNameTextBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Please enter the artist’s name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                EditArtistNameTextBox.BorderBrush = Brushes.Gray;

            if (string.IsNullOrWhiteSpace(EditTitleOfSongTextBox.Text))
            {
                isValid = false;
                EditTitleOfSongTextBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Please enter the song title.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                EditTitleOfSongTextBox.BorderBrush = Brushes.Gray;

            if (!int.TryParse(EditGradeOfFanTextBox.Text.Trim(), out int grade) || grade < 1 || grade > 10)
            {
                isValid = false;
                EditGradeOfFanTextBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Please enter a valid fan grade between 1 and 10.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                EditGradeOfFanTextBox.BorderBrush = Brushes.Gray;

            if (EditPreviewImage.Source == null)
            {
                isValid = false;
                MessageBox.Show("Please select an image for the artist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            TextRange reviewText = new TextRange(EditDescriptionRichTextBox.Document.ContentStart, EditDescriptionRichTextBox.Document.ContentEnd);
            if (string.IsNullOrWhiteSpace(reviewText.Text.Trim()))
            {
                isValid = false;
                MessageBox.Show("Please write a review.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return isValid;
        }
        private void LoadRtf()
        {
            if (!File.Exists(_artist.RtfPath)) return;

            TextRange range = new TextRange(
                EditDescriptionRichTextBox.Document.ContentStart,
                EditDescriptionRichTextBox.Document.ContentEnd);

            using (FileStream fs = new FileStream(_artist.RtfPath, FileMode.Open))
            {
                range.Load(fs, DataFormats.Rtf);
            }
        }
        private void SaveRtf()
        {
            if (string.IsNullOrEmpty(_artist.RtfPath)) return;

            TextRange range = new TextRange(
                EditDescriptionRichTextBox.Document.ContentStart,
                EditDescriptionRichTextBox.Document.ContentEnd);

            using (FileStream fs = new FileStream(_artist.RtfPath, FileMode.Create))
            {
                range.Save(fs, DataFormats.Rtf);
            }
        }

        private void SaveToXml()
        {
            string path = "artists.xml";
            DataIO_User io = new DataIO_User();

            if (!File.Exists(path))
            {
                MessageBox.Show("artists.xml not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ArtistList list = io.DeSerializeObject<ArtistList>(path);

            Artist existing = list.Artists.FirstOrDefault(a => a.ArtistName == _artist.ArtistName);
            if (existing != null)
            {
                existing.ArtistName = EditArtistNameTextBox.Text;
                existing.TitleOfSong = EditTitleOfSongTextBox.Text;
                existing.GradeOfFan = int.TryParse(EditGradeOfFanTextBox.Text, out int grade) ? grade : existing.GradeOfFan;

                if (EditPreviewImage.Source is BitmapImage bitmap)
                {
                    string absoluteImagePath = bitmap.UriSource.LocalPath;
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    existing.ImagePath = System.IO.Path.GetRelativePath(basePath, absoluteImagePath);
                }

            }

            io.SerializeObject(list, path);
        }
        private void FillForm()
        {
            EditArtistNameTextBox.Text = _artist.ArtistName;
            EditTitleOfSongTextBox.Text = _artist.TitleOfSong;
            EditGradeOfFanTextBox.Text = _artist.GradeOfFan.ToString();

            if (!string.IsNullOrEmpty(_artist.ImagePath))
            {
                string absolutePath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    _artist.ImagePath
                );

                if (File.Exists(absolutePath))
                {
                    EditPreviewImage.Source =
                        new BitmapImage(new Uri(absolutePath));
                }
            }

            LoadRtf();
        }

        private void ButtonEditStudent_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            SaveRtf();

            _artist.ArtistName = EditArtistNameTextBox.Text;
            _artist.TitleOfSong = EditTitleOfSongTextBox.Text;
            _artist.GradeOfFan = int.TryParse(EditGradeOfFanTextBox.Text, out int grade) ? grade : _artist.GradeOfFan;

            if (EditPreviewImage.Source is BitmapImage bitmap)
            {
                string absoluteImagePath = bitmap.UriSource.LocalPath;
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                _artist.ImagePath = System.IO.Path.GetRelativePath(basePath, absoluteImagePath);
            }

            _artist.DateAdded = DateTime.Now;

            SaveToXml();

            ArtistUpdated?.Invoke(_artist);

            MessageBox.Show("Artist updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
        private void EditButtonLeavePage_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditDescriptionRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextRange textRange = new TextRange(EditDescriptionRichTextBox.Document.ContentStart, EditDescriptionRichTextBox.Document.ContentEnd);
            string text = textRange.Text.Trim();
            int wordCount = string.IsNullOrEmpty(text) ? 0 : text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            EditWordCountTextBlock.Text = $"Words: {wordCount}"; // brojanje reci
        }

        private void LoadSystemColors()
        {
            var properties = typeof(Colors).GetProperties();
            foreach (var prop in properties)
            {
                Color color = (Color)prop.GetValue(null);
                EditColorComboBox.Items.Add(new
                {
                    Name = prop.Name,
                    Brush = new SolidColorBrush(color)
                });
            }
        }

        private void LoadFontFamilies()
        {
            foreach (var font in Fonts.SystemFontFamilies)
                EditFontFamilyComboBox.Items.Add(font);
        }

        private void LoadFontSizes()
        {
            for (double i = 8; i <= 72; i += 2)
                EditFontSizeComboBox.Items.Add(i);
        }
        private void EditFontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EditFontFamilyComboBox.SelectedItem is FontFamily selectedFont)
            {
                EditDescriptionRichTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, selectedFont);
            }
        }

        private void EditFontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EditFontSizeComboBox.SelectedItem != null)
            {
                double size;
                if (double.TryParse(EditFontSizeComboBox.SelectedItem.ToString(), out size))
                {
                    EditDescriptionRichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, size);
                }
            }
        }

        private void EditColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EditColorComboBox.SelectedItem != null)
            {
                var item = EditColorComboBox.SelectedItem;
                var brushProperty = item.GetType().GetProperty("Brush");
                if (brushProperty != null)
                {
                    var brush = brushProperty.GetValue(item) as Brush;
                    if (brush != null)
                    {
                        EditDescriptionRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
                    }
                }
            }
        }
        private void EditDescriptionRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selection = EditDescriptionRichTextBox.Selection;

            object bold = selection.GetPropertyValue(TextElement.FontWeightProperty);
            EditBoldToggleButton.IsChecked = (bold != DependencyProperty.UnsetValue) && bold.Equals(FontWeights.Bold);

            object italic = selection.GetPropertyValue(TextElement.FontStyleProperty);
            EditItalicToggleButton.IsChecked = (italic != DependencyProperty.UnsetValue) && italic.Equals(FontStyles.Italic);

            object underline = selection.GetPropertyValue(Inline.TextDecorationsProperty);
            EditUnderlineToggleButton.IsChecked = (underline != DependencyProperty.UnsetValue) && underline.Equals(TextDecorations.Underline);

            object fontFamily = selection.GetPropertyValue(TextElement.FontFamilyProperty);
            if (fontFamily != DependencyProperty.UnsetValue)
                EditFontFamilyComboBox.SelectedItem = fontFamily;

            object fontSize = selection.GetPropertyValue(TextElement.FontSizeProperty);
            if (fontSize != DependencyProperty.UnsetValue)
                EditFontSizeComboBox.SelectedItem = fontSize;

            object foreground = selection.GetPropertyValue(TextElement.ForegroundProperty);
            if (foreground != DependencyProperty.UnsetValue)
            {
                Brush brush = foreground as Brush;
                if (brush != null)
                {
                    foreach (var item in EditColorComboBox.Items)
                    {
                        var brushProp = item.GetType().GetProperty("Brush");
                        if (brushProp != null)
                        {
                            var itemBrush = brushProp.GetValue(item) as Brush;
                            if (itemBrush != null && itemBrush.ToString() == brush.ToString())
                            {
                                EditColorComboBox.SelectedItem = item;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void EditPreviewImageButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;";

            bool? result = dlg.ShowDialog();// upamti true/false/null 

            if (result == true)
            {
                try
                {
                    string source = dlg.FileName;

                    string imagesFolder = "Images";
                    Directory.CreateDirectory(imagesFolder);

                    string fileName = System.IO.Path.GetFileName(source);
                    string dest = System.IO.Path.Combine(imagesFolder, fileName);

                    if (!System.IO.Path.GetFullPath(source).Equals(System.IO.Path.GetFullPath(dest), StringComparison.OrdinalIgnoreCase))
                    {
                        File.Copy(source, dest, true);
                    }

                    EditPreviewImage.Source = new BitmapImage(
                        new Uri(System.IO.Path.GetFullPath(dest))
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
        }
    }
}
