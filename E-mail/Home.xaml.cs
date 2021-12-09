using AE.Net.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace E_mail
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class Home : Page
    {
        public AE.Net.Mail.Imap.Mailbox[] listMailboxes;
        public ImapClient ic;
        public AE.Net.Mail.MailMessage[] mm;
        public Home()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.InitializeComponent();
        }

        public Zapis zapis = new Zapis();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Zapis));
            StorageFile storageFile = await storageFolder.CreateFileAsync("DataBaseMailPassword.xml", CreationCollisionOption.OpenIfExists);
            Stream read = await storageFile.OpenStreamForReadAsync();
            try
            {
                zapis = (Zapis)xmlSerializer.Deserialize(read);
            }
            catch { }
            read.Close();

            ic = new ImapClient("imap." + zapis.mail, zapis.login, zapis.passwordText, AuthMethods.Login, 993, true);
            mm = ic.GetMessages(0, ic.GetMessageCount(), false);

            foreach (AE.Net.Mail.MailMessage m in mm)
            {
                view.Items.Add(m.Subject);
            }
            listMailboxes = ic.ListMailboxes(string.Empty, "*");
            for (int i = 0; i < listMailboxes.Length; i++)
            {
                Menu.Items.Add(listMailboxes[i].Name);
            }
        }

        private void Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ic.SelectMailbox(listMailboxes[Menu.SelectedIndex].Name);
            view.Items.Clear();
            mm = ic.GetMessages(0, ic.GetMessageCount(), false);

            foreach (AE.Net.Mail.MailMessage m in mm)
            {
                view.Items.Add(m.Subject);
            }
        }

        private void latter_SelectionChanged(object sender, RoutedEventArgs e)
        {
            latter.Text = "";
            if (view.SelectedIndex != -1)
            {
                latter.Text = mm[view.SelectedIndex].Body;
            }

        }
        public bool click = false;
        private void WriteLatter_Click(object sender, RoutedEventArgs e)
        {
            latter.Text = "";
            if (click == true)
            {
                GridLatterView.Visibility = Visibility.Visible;
                GridLatterWrite.Visibility = Visibility.Collapsed;
                click = false;
            }
            else
            {
                click = true;
                GridLatterWrite.Visibility = Visibility.Visible;
                GridLatterView.Visibility = Visibility.Collapsed;
            }
        }

        private void AddLatterToSend_Click(object sender, RoutedEventArgs e)
        {
            if (user.Text != "" && teg.Text != "" && LatterText.Text != "")
            {
                MailAddress from = new MailAddress(zapis.login);
                // кому отправляем
                MailAddress to = new MailAddress(user.Text);
                // создаем объект сообщения
                System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(from, to);
                // тема письма
                m.Subject = teg.Text;
                // текст письма
                m.Body = LatterText.Text;
                // письмо представляет код html
                m.IsBodyHtml = true;
                // адрес smtp-сервера и порт, с которого будем отправлять письмо
                SmtpClient smtp = new SmtpClient("smtp." + zapis.mail, 587);
                // логин и пароль
                smtp.Credentials = new NetworkCredential(zapis.login, zapis.passwordText);
                smtp.EnableSsl = true;
                smtp.Send(m);

                user.Text = "";

                teg.Text = "";

                LatterText.Text = "";

                GridLatterWrite.Visibility = Visibility.Collapsed;
                GridLatterView.Visibility = Visibility.Visible;
            }
        }
    }
}
