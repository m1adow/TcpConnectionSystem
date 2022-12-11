using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows.Input;
using TcpConnectionSystem.Models;
using TcpConnectionSystem.Core;
using System.Collections.ObjectModel;

namespace TcpConnectionSystem.ViewModels
{
	public sealed class MainViewModel : ViewModelBase
	{
        #region Commands

        public ICommand ConnectCommand { get; private set; }
        public ICommand CreateServerCommand { get; private set; }

        #endregion

        #region Fields

        private NetworkAccess networkAccess;

        #endregion

        #region Main user

        private User user = new User();

		public User User
		{
			get => user;
			set => Set(ref user, value);
		}

        #endregion

        #region Visibilities

        private bool isServersVisible;

        public bool IsServersVisible
        {
            get => isServersVisible;
            set => Set(ref isServersVisible, value);
        }

        private bool isUsersVisible;

        public bool IsUsersVisible
        {
            get => isUsersVisible;
            set => Set(ref isUsersVisible, value);
        }

        #endregion

        public ObservableCollection<User> ConnectedUsers { get; private set; } = new ObservableCollection<User>();

        public ObservableCollection<KeyValuePair<string, string>> ConnectedServers { get; private set; } = new ObservableCollection<KeyValuePair<string, string>>();

        public MainViewModel()
		{
            ConnectCommand = new RelayCommand(ConnectExecute);
            CreateServerCommand = new RelayCommand(CreateServerExecute);

            ConnectedUsers.CollectionChanged += OnCollectionChanged;
            ConnectedServers.CollectionChanged += OnCollectionChanged;

            InitializeUser();
		}

        #region Collections changed

        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (sender is ObservableCollection<User>)
            {
                bool isUsersAdded = (sender as ObservableCollection<User>).Count > 0;
                IsUsersVisible = isUsersAdded;

                if (isUsersAdded)
                {
                    (sender as ObservableCollection<User>).CollectionChanged -= OnCollectionChanged;
                }
            }
            else if (sender is ObservableCollection<KeyValuePair<string, string>>)
            {
                bool isServersAdded = (sender as ObservableCollection<KeyValuePair<string, string>>).Count > 0;
                IsServersVisible = isServersAdded;

                if (isServersAdded)
                {
                    (sender as ObservableCollection<KeyValuePair<string, string>>).CollectionChanged -= OnCollectionChanged;
                }
            }
        }

        #endregion

        private async void InitializeUser()
        {
            this.networkAccess = Connectivity.Current.NetworkAccess;
            Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;

            if (this.networkAccess == NetworkAccess.Internet)
            {
                User = new User(MainViewModel.GetUserName(), MainViewModel.GetIPAddress());
            }
            else
            {
                await MainViewModel.ShowInternetAlertAsync();
            }
        }

        private async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            this.networkAccess = e.NetworkAccess;

            if (this.networkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("Internet", "We have lost internet connection", "Ok");
            }
        }

        #region Connect

        private async void ConnectExecute(object parameter)
        {
            if (Application.Current.MainPage is Page mainPage)
            {
                if (this.networkAccess == NetworkAccess.Internet)
                {
                    string ipInput = await mainPage.DisplayPromptAsync("Ip", "Enter ip");
                    await ConnectToServerAsync(ipInput);
                }
                else
                {
                    await MainViewModel.ShowInternetAlertAsync();
                }
            }
        }

        private async Task ConnectToServerAsync(string ip)
        {
            const string cancel = "Cancel";
            const string error = "Error";
            const string wrong = "{0} is wrong";

            if (!(ip is { Length: > 0 } && ip != this.User.IPAddress.ToString() && Regex.Match(ip, @"\b(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\b").Success))
            {
                await Application.Current.MainPage.DisplayAlert(error, string.Format(wrong, "Ip"), cancel);
                return;
            }

            await this.User.Client.ConnectAsync(ip, 8888);
            this.ConnectedServers.Add(new KeyValuePair<string, string>(ip, "8888"));
        }

        #endregion

        #region Create server

        private async void CreateServerExecute(object parameter)
        {
            if (this.networkAccess == NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("Success", "Server was created", "Ok");
                this.User.Listener.Start();
                await AcceptConnectionRequestsAsync(this.User.Listener);
            }
            else
            {
                await MainViewModel.ShowInternetAlertAsync();
            }
        }

        private async Task AcceptConnectionRequestsAsync(TcpListener listener)
        {
            while (this.networkAccess == NetworkAccess.Internet)
            {
                var client = await listener.AcceptTcpClientAsync();
                var ipAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
                var hostEntry = Dns.GetHostEntry(ipAddress);
                string hostName = hostEntry.HostName;
                var user = new User(hostName, ipAddress);
                this.ConnectedUsers.Add(user);
            }

            await MainViewModel.ShowInternetAlertAsync();
        }

        #endregion

        private static async Task ShowInternetAlertAsync() => await Application.Current.MainPage.DisplayAlert("Internet", "Check your internet connection and try again", "Ok");

        private static IPAddress GetIPAddress()
        {
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            var externalIp = IPAddress.Parse(externalIpString);
            return externalIp;
        }

		private static string GetUserName() => Environment.MachineName;
    }
}

