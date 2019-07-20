using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;

namespace ImapNotes
{
    public partial class MainWindow : Window
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/gmail-dotnet-quickstart.json
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "Gmail API .NET Quickstart";
        public ObservableCollection<Message> Messages { get; set; }

        private GmailService GmailService;

        Message selectedMessage;

        public MainWindow()
        {
            Messages = new ObservableCollection<Message>();
            InitializeComponent();
            SetupGmailService();
       
            UsersResource.MessagesResource.ListRequest request = GmailService.Users.Messages.List("me");
            request.Q = "label: notes";

            bool mainMessageSet = false;
            IList<Message> messages = request.Execute().Messages;

            if (messages != null && messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    var messageRequest = GmailService.Users.Messages.Get("me", message.Id);
                    var fetchedMessage = messageRequest.Execute();

                    Messages.Add(fetchedMessage);

                    if (!mainMessageSet)
                    {
                        this.updateSelectedMessage(fetchedMessage);
                    }
                }
            }

            DataContext = this;
        }

        private void SetupGmailService()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            GmailService = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        private void OnSelected(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbi = e.Source as ListBoxItem;
        }

        void updateSelectedMessage(Message message)
        {
            this.selectedMessage = message;
            Console.WriteLine(message.Payload.Body.Data);
            this.mainNote.Text = this.selectedMessage.Payload.Body.Data;
        }

        private void noteListItemClick(object sender, RoutedEventArgs e)
        {
            // do something
        }
    }
}
