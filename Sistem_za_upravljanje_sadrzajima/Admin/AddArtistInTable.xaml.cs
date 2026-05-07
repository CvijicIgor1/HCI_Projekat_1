using Notification.Wpf;
using Notification.Wpf.Controls;
using Sistem_za_upravljanje_sadrzajima.Helper;
using Sistem_za_upravljanje_sadrzajima.Modeli;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sistem_za_upravljanje_sadrzajima
{
    public partial class AddArtistInTable : Window
    {
        private string _ArtistNamePlaceHolder = "Input Artist name";
        private string _TitleOfSongPlaceHolder = "Input Title of song";
        private string _FanGradePlaceHolder = "Input grade here (1-10)";
        private NotificationManager notificationManager;

        public ObservableCollection<Artist> ArtistsCollection { get; set; }

        public AddArtistInTable(ObservableCollection<Artist> existingCollection)
        {
            InitializeComponent();
            ArtistsCollection = existingCollection;
            this.DataContext = this;

            LoadSystemColors();
            LoadFontFamilies();
            LoadFontSizes();
            notificationManager = new NotificationManager();

        }

        public void ShowToastNotification(string title, string message, NotificationType type)
        {
            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = message,
                Type = type
            });
        }

        private void LoadSystemColors()
        {
            var properties = typeof(Colors).GetProperties();
            foreach (var prop in properties)
            {
                Color color = (Color)prop.GetValue(null);
                ColorComboBox.Items.Add(new
                {
                    Name = prop.Name,
                    Brush = new SolidColorBrush(color)
                });
            }

            ArtistNameTextBox.Text = _ArtistNamePlaceHolder;
            ArtistNameTextBox.Foreground = Brushes.LightSlateGray;

            TitleOfSongTextBox.Text = _TitleOfSongPlaceHolder;
            TitleOfSongTextBox.Foreground = Brushes.LightSlateGray;

            GradeOfFanTextBox.Text = _FanGradePlaceHolder;
            GradeOfFanTextBox.Foreground = Brushes.LightSlateGray;
        }

        private void LoadFontFamilies()
        {
            foreach (var font in Fonts.SystemFontFamilies)
                FontFamilyComboBox.Items.Add(font);
        }

        private void LoadFontSizes()
        {
            for (double i = 8; i <= 72; i += 2)
                FontSizeComboBox.Items.Add(i);
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(ArtistNameTextBox.Text) || ArtistNameTextBox.Text == _ArtistNamePlaceHolder)
            {
                isValid = false;
                ArtistNameTextBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Please enter the artist’s name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                ArtistNameTextBox.BorderBrush = Brushes.Gray;

            if (string.IsNullOrWhiteSpace(TitleOfSongTextBox.Text) || TitleOfSongTextBox.Text == _TitleOfSongPlaceHolder)
            {
                isValid = false;
                TitleOfSongTextBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Please enter the song title.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                TitleOfSongTextBox.BorderBrush = Brushes.Gray;

            if (GradeOfFanTextBox.Text == _FanGradePlaceHolder || !int.TryParse(GradeOfFanTextBox.Text.Trim(), out int grade) || grade < 1 || grade > 10)
            {
                isValid = false;
                GradeOfFanTextBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Please enter a valid fan grade between 1 and 10.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                GradeOfFanTextBox.BorderBrush = Brushes.Gray;

            if (PreviewImage.Source == null)
            {
                isValid = false;
                MessageBox.Show("Please select an image for the artist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            TextRange reviewText = new TextRange(DescriptionRichTextBox.Document.ContentStart, DescriptionRichTextBox.Document.ContentEnd);
            if (string.IsNullOrWhiteSpace(reviewText.Text.Trim()))
            {
                isValid = false;
                MessageBox.Show("Please write a review.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return isValid;
        }

        private string SaveRtf()
        {
            string folder = "RTF";
            Directory.CreateDirectory(folder);

            string file = $"{folder}/{Guid.NewGuid()}.rtf";

            TextRange range = new TextRange(
                DescriptionRichTextBox.Document.ContentStart,
                DescriptionRichTextBox.Document.ContentEnd);

            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                range.Save(fs, DataFormats.Rtf);
            }

            return file;
        }

        private void ClearForm()
        {
            ArtistNameTextBox.Text = _ArtistNamePlaceHolder;
            ArtistNameTextBox.Foreground = Brushes.LightSlateGray;

            TitleOfSongTextBox.Text = _TitleOfSongPlaceHolder;
            TitleOfSongTextBox.Foreground = Brushes.LightSlateGray;

            GradeOfFanTextBox.Text = _FanGradePlaceHolder;
            GradeOfFanTextBox.Foreground = Brushes.LightSlateGray;

            PreviewImage.Source = null;
            DescriptionRichTextBox.Document.Blocks.Clear();
            WordCountTextBlock.Text = "Words: 0";
        }

        private void ButtonAddStudent_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            string rtfPath = SaveRtf();

            string absoluteImagePath = ((BitmapImage)PreviewImage.Source).UriSource.LocalPath;
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string relativeImagePath = Path.GetRelativePath(basePath, absoluteImagePath);

            Artist artist = new Artist(
                ArtistNameTextBox.Text,
                TitleOfSongTextBox.Text,
                int.Parse(GradeOfFanTextBox.Text),
                relativeImagePath,
                rtfPath
            );

            ArtistsCollection.Add(artist);
            SaveToXml(artist);
            ClearForm();

            ShowToastNotification("Success", "Artist added successfully!", NotificationType.Success);
        }

        private void SaveToXml(Artist artist)
        {
            DataIO_User io = new DataIO_User();
            string path = "artists.xml";

            ArtistList list;

            if (File.Exists(path))
                list = io.DeSerializeObject<ArtistList>(path);
            else
                list = new ArtistList(); // ArtistList je wrapper nije lista 

            list.Artists.Add(artist);
            io.SerializeObject(list, path);
        }

        private void ButtonLeavePage_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GradeOfFanTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void PreviewImageButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;";

            bool? result = dlg.ShowDialog(); // true ili false a moze i null 

            if (result == true)
            {
                try
                {
                    string source = dlg.FileName;

                    string imagesFolder = "Images";
                    Directory.CreateDirectory(imagesFolder);

                    string fileName = Path.GetFileName(source);
                    string dest = Path.Combine(imagesFolder, fileName);

                    if (!Path.GetFullPath(source).Equals(Path.GetFullPath(dest), StringComparison.OrdinalIgnoreCase))
                    {
                        File.Copy(source, dest, true);
                    }

                    PreviewImage.Source = new BitmapImage(
                        new Uri(Path.GetFullPath(dest))
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
        }

        private void DescriptionRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextRange textRange = new TextRange(DescriptionRichTextBox.Document.ContentStart, DescriptionRichTextBox.Document.ContentEnd);
            string text = textRange.Text.Trim();
            int wordCount = string.IsNullOrEmpty(text) ? 0 : text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            WordCountTextBlock.Text = $"Words: {wordCount}";
        }

        private void ArtistNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(ArtistNameTextBox.Text == "")
            {
                ArtistNameTextBox.Text = _ArtistNamePlaceHolder;
                ArtistNameTextBox.Foreground = Brushes.LightSlateGray;
            }
        }

        private void ArtistNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(ArtistNameTextBox.Text == _ArtistNamePlaceHolder)
            {
                ArtistNameTextBox.Text = "";
                ArtistNameTextBox.Foreground = Brushes.Black;
            }
        }

        private void TitleOfSongTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TitleOfSongTextBox.Text == _TitleOfSongPlaceHolder)
            {
                TitleOfSongTextBox.Text = "";
                TitleOfSongTextBox.Foreground = Brushes.Black;
            }
        }


        private void TitleOfSongTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TitleOfSongTextBox.Text == "")
            {
                TitleOfSongTextBox.Text = _TitleOfSongPlaceHolder;
                TitleOfSongTextBox.Foreground = Brushes.LightSlateGray;
            }
        }

        private void GradeOfFanTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GradeOfFanTextBox.Text == "")
            {
                GradeOfFanTextBox.Text = _FanGradePlaceHolder;
                GradeOfFanTextBox.Foreground = Brushes.LightSlateGray;
            }
        }

        private void GradeOfFanTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(GradeOfFanTextBox.Text == _FanGradePlaceHolder)
            {
                GradeOfFanTextBox.Text = "";
                GradeOfFanTextBox.Foreground = Brushes.Black;
            }
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem is FontFamily selectedFont)
            {
                DescriptionRichTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, selectedFont);
            }
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeComboBox.SelectedItem != null)
            {
                double size;
                if (double.TryParse(FontSizeComboBox.SelectedItem.ToString(), out size))
                {
                    DescriptionRichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, size);
                }
            }
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorComboBox.SelectedItem != null)
            {
                var item = ColorComboBox.SelectedItem;
                var brushProperty = item.GetType().GetProperty("Brush");
                if (brushProperty != null)
                {
                    var brush = brushProperty.GetValue(item) as Brush;
                    if (brush != null)
                    {
                        DescriptionRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
                    }
                }
            }
        }

        private void DescriptionRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selection = DescriptionRichTextBox.Selection;

            object bold = selection.GetPropertyValue(TextElement.FontWeightProperty);
            BoldToggleButton.IsChecked = (bold != DependencyProperty.UnsetValue) && bold.Equals(FontWeights.Bold);

            object italic = selection.GetPropertyValue(TextElement.FontStyleProperty);
            ItalicToggleButton.IsChecked = (italic != DependencyProperty.UnsetValue) && italic.Equals(FontStyles.Italic);

            object underline = selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineToggleButton.IsChecked = (underline != DependencyProperty.UnsetValue) && underline.Equals(TextDecorations.Underline);

            object fontFamily = selection.GetPropertyValue(TextElement.FontFamilyProperty);
            if (fontFamily != DependencyProperty.UnsetValue)
                FontFamilyComboBox.SelectedItem = fontFamily;

            object fontSize = selection.GetPropertyValue(TextElement.FontSizeProperty);
            if (fontSize != DependencyProperty.UnsetValue)
                FontSizeComboBox.SelectedItem = fontSize;

            object foreground = selection.GetPropertyValue(TextElement.ForegroundProperty);
            if (foreground != DependencyProperty.UnsetValue)
            {
                Brush brush = foreground as Brush;
                if (brush != null)
                {
                    foreach (var item in ColorComboBox.Items)
                    {
                        var brushProp = item.GetType().GetProperty("Brush");
                        if (brushProp != null)
                        {
                            var itemBrush = brushProp.GetValue(item) as Brush;
                            if (itemBrush != null && itemBrush.ToString() == brush.ToString())
                            {
                                ColorComboBox.SelectedItem = item;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}