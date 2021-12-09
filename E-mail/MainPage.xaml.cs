using AE.Net.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace E_mail
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    [Serializable]
    public class Zapis
    {
        public string mail { get; set; }
        public string login { get; set; }
        public string passwordText { get; set; }
        public Zapis(string mail, string login, string passwordText)
        {
            this.mail = mail;
            this.login = login;
            this.passwordText = passwordText;
        }
        public Zapis()
        {

        }
    }
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.InitializeComponent();
        }
        private async void InPut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ImapClient ic = new ImapClient("imap.mail.ru", login.Text.ToString(), password.Password.ToString(), AuthMethods.Login, 993, true);

                StorageFolder folder = ApplicationData.Current.LocalFolder;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Zapis));
                StorageFile file = await folder.CreateFileAsync("DataBaseMailPassword.xml", CreationCollisionOption.ReplaceExisting);
                Zapis zapis = new Zapis("mail.ru", login.Text.ToString(), password.Password.ToString());
                Stream write = await file.OpenStreamForWriteAsync();
                xmlSerializer.Serialize(write, zapis);
                write.Close();

                this.Frame.Navigate(typeof(Home));
            }
            catch
            {
                var rep = await new MessageDialog("Ошибка").ShowAsync();
            }
        }
    }
}
