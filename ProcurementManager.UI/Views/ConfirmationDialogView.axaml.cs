using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ProcurementManager.UI.Views
{
    public partial class ConfirmationDialogView : UserControl
    {
        public ConfirmationDialogView()
        {
            InitializeComponent();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            var window = (Window)this.VisualRoot;
            window.Close(true);
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            var window = (Window)this.VisualRoot;
            window.Close(false);
        }
    }
}
