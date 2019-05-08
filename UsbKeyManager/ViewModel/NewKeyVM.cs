using DevExpress.Mvvm;
using MaterialDesignXaml.DialogsHelper;
using System;
using System.Security.Cryptography;
using System.Windows.Input;

namespace UsbKeyManager.ViewModel
{
    class NewKeyVM : IClosableDialog
    {
        public NewKeyVM(IDialogIdentifier identifier)
        {
            Identifier = identifier;
            CloseDialogCommand = new DelegateCommand(CloseDialog);
        }

        /// <summary>
        /// Key data.
        /// </summary>
        public string KeyData { get; set; } = "";

        /// <summary>
        /// Close dialog and return key data.
        /// </summary>
        void CloseDialog()
        {
            this.Close(KeyData.GetHash(MD5.Create()));
        }

        /// <summary>
        /// Command for closing dialog.
        /// </summary>
        public ICommand CloseDialogCommand { get; }

        /// <summary>
        /// Dialog identifier.
        /// </summary>
        public IDialogIdentifier Identifier { get; }
    }
}
