using HW2_WPF_MyNotes__Client_Server_.ViewModel;
using System.Windows;

namespace HW2_WPF_MyNotes__Client_Server_
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainVM();
        }
    }
}
