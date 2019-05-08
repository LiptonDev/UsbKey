using DevExpress.Mvvm;
using MaterialDesignXaml.DialogsHelper;
using MaterialDesignXaml.DialogsHelper.Enums;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using UsbKeyManager.Model;
using UsbKeyManager.View;

namespace UsbKeyManager.ViewModel
{
    /// <summary>
    /// Main ViewModel.
    /// </summary>
    class MainVM : ViewModelBase, IDialogIdentifier
    {
        public string Identifier => "RootDialog";

        private Driver selectedDriver;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainVM()
        {
            Drivers = new ObservableCollection<Driver>();
            UpdateDriversCommand = new DelegateCommand(LoadDrivers);
            CreateKeyCommand = new DelegateCommand(CreateKey);
            RemoveKeyCommand = new DelegateCommand(RemoveKey);
            CopyKeyCommand = new DelegateCommand(CopyKey);
            AddToStartupCommand = new DelegateCommand(AddToStartup);

            LoadDrivers();
        }

        /// <summary>
        /// Load all removable drivers.
        /// </summary>
        public ICommand UpdateDriversCommand { get; }

        /// <summary>
        /// Command for creating keys in drivers.
        /// </summary>
        public ICommand CreateKeyCommand { get; }

        /// <summary>
        /// Command for removing keys from drivers.
        /// </summary>
        public ICommand RemoveKeyCommand { get; }

        /// <summary>
        /// Command for copying key from driver to UsbKey.
        /// </summary>
        public ICommand CopyKeyCommand { get; }

        /// <summary>
        /// Command for adding to startup.
        /// </summary>
        public ICommand AddToStartupCommand { get; }

        /// <summary>
        /// All removable drivers.
        /// </summary>
        public ObservableCollection<Driver> Drivers { get; }

        /// <summary>
        /// Selected driver.
        /// </summary>
        public Driver SelectedDriver
        {
            get => selectedDriver;
            set
            {
                selectedDriver = value;
                RaisePropertyChanged();

                value?.LoadInfo();
            }
        }

        /// <summary>
        /// Add to startup.
        /// </summary>
        async void AddToStartup()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Select UsbKey.exe"
            };

            if (ofd.ShowDialog() == false)
                return;

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            rk.SetValue("UsbKey", ofd.FileName);

            await this.ShowMessageBoxAsync("UsbKey added to startup!", MaterialMessageBoxButtons.Ok);
        }

        /// <summary>
        /// Copy key.
        /// </summary>
        async void CopyKey()
        {
            if (SelectedDriver == null)
            {
                await this.ShowMessageBoxAsync("Driver not selected.", MaterialMessageBoxButtons.Ok);
                return;
            }

            if (!SelectedDriver.ContainsKeyFile)
            {
                var ans = await this.ShowMessageBoxAsync("Key not found!\nCreate key?", MaterialMessageBoxButtons.YesNo);

                if (ans == MaterialMessageBoxButtons.Yes)
                    CreateKey();

                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName = Driver.KeyFileName,
                Title = "Don't replace the file name!"
            };

            if (sfd.ShowDialog() == true)
            {
                File.Copy(SelectedDriver.FullPath, sfd.FileName, true);

                await this.ShowMessageBoxAsync("Key copied", MaterialMessageBoxButtons.Ok);
            }
        }

        /// <summary>
        /// Remove key.
        /// </summary>
        async void RemoveKey()
        {
            if (SelectedDriver == null)
            {
                await this.ShowMessageBoxAsync("Driver not selected.", MaterialMessageBoxButtons.Ok);
                return;
            }

            if (!SelectedDriver.ContainsKeyFile)
            {
                var ans = await this.ShowMessageBoxAsync("Key already removed!\nCreate key?", MaterialMessageBoxButtons.YesNo);

                if (ans == MaterialMessageBoxButtons.Yes)
                    CreateKey();

                return;
            }

            if ((await this.ShowMessageBoxAsync("Are you sure you want to delete the key?", MaterialMessageBoxButtons.YesNo)) == MaterialMessageBoxButtons.Yes)
            {
                SelectedDriver.RemoveKey();

                await this.ShowMessageBoxAsync("Key removed.", MaterialMessageBoxButtons.Ok);
            }
        }

        /// <summary>
        /// Create key.
        /// </summary>
        async void CreateKey()
        {
            if (SelectedDriver == null)
            {
                await this.ShowMessageBoxAsync("Driver not selected.", MaterialMessageBoxButtons.Ok);
                return;
            }

            if (SelectedDriver.ContainsKeyFile)
            {
                var ans = await this.ShowMessageBoxAsync("Key already created!\nRemove key?", MaterialMessageBoxButtons.YesNo);

                if (ans == MaterialMessageBoxButtons.Yes)
                    RemoveKey();

                return;
            }

            var res = await this.ShowAsync<byte[]>(new NewKeyDialog(this));

            SelectedDriver.CreateKey(res.Concat(Guid.NewGuid().ToByteArray()).ToArray());

            await this.ShowMessageBoxAsync("Key created.", MaterialMessageBoxButtons.Ok);
        }

        /// <summary>
        /// Load all removable drivers.
        /// </summary>
        void LoadDrivers()
        {
            Drivers.Clear();
            var drivers = DriveInfo.GetDrives().Where(drive => drive.IsReady && drive.DriveType == DriveType.Removable);

            foreach (var item in drivers)
            {
                Drivers.Add(new Driver(item.VolumeLabel, item.Name));
            }
        }
    }
}
