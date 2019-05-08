using MaterialDesignXaml.DialogsHelper;
using System.Windows.Controls;
using UsbKeyManager.ViewModel;

namespace UsbKeyManager.View
{
    /// <summary>
    /// Return: byte[]
    /// </summary>
    public partial class NewKeyDialog : UserControl
    {
        public NewKeyDialog(IDialogIdentifier identifier)
        {
            InitializeComponent();

            DataContext = new NewKeyVM(identifier);
        }
    }
}
