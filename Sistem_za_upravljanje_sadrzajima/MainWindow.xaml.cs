using Sistem_za_upravljanje_sadrzajima.Enumeracije;
using Sistem_za_upravljanje_sadrzajima.Helper;
using Sistem_za_upravljanje_sadrzajima.Modeli;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sistem_za_upravljanje_sadrzajima
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      
        private List<User> users = new List<User>();
        private DataIO_User serializer = new DataIO_User();
        private string filePath = "users.xml";
        public MainWindow()
        {
            InitializeComponent();
            LoadUsers();
        }
        private void LoadUsers()
        {
            if (!System.IO.File.Exists(filePath))
            {
                users = new List<User>
                {
                    new User("ADMIN", "1234", UserRole.Admin),
                    new User("VISITOR", "visitor", UserRole.Visitor)
                };

                serializer.SerializeObject(users, filePath);
            }
            else
            {
                users = serializer.DeSerializeObject<List<User>>(filePath);
                if (users == null)
                    users = new List<User>();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /*MessageBoxResult messageBoxResult = MessageBox.Show("Are you really sure you want to exit this masterpiece?", "Confirmation Window", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
            if (messageBoxResult == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                  Application.Current.Shutdown();
            }*/
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            string enteredUsername = UsernameTextBoxLogInTab.Text.Trim();
            string enteredPassword = PasswordBoxPasswordLogInTab.Password.Trim();

            User foundUser = users.FirstOrDefault(u => u.Username == enteredUsername && u.Password == enteredPassword);

            if (foundUser == null)
            {
                MessageBox.Show("Username or password is incorrect!","Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (foundUser.Role == UserRole.Admin || foundUser.Role==UserRole.Visitor)
            {
                this.Hide();
                ArtistDataTable newWindow = new ArtistDataTable(foundUser.Role);
                newWindow.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Something went wrong try again!", "Incorrect!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}