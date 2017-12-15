using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TecnicalServiceStation.ClassModels;

namespace TecnicalServiceStation.Forms
{
    /// <summary>
    /// Логика взаимодействия для Organization.xaml
    /// </summary>
    public partial class OrganizationForm : MetroWindow
    {
        private TSSEntities _TSS = new TSSEntities();
        private Organization _organization;
        public OrganizationForm()
        {
            InitializeComponent();
            this._organization = _TSS.Organization.First();
            this.DataContext = _organization;
        }

        private void Clicked_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Clicked_Save(object sender, RoutedEventArgs e)
        {
            Repository.Update(this._organization, this._TSS);
            this.DataContext = null;
            this.DataContext = this._organization;
            this._CheckBox_RedactionEnabled.IsChecked = false;
        }

        private void Clicked_CancelChanges(object sender, RoutedEventArgs e)
        {
            Repository.UndoingChangesDbContextLevel(this._TSS);
            this.DataContext = null;
            this.DataContext = this._organization;
        }
    }
}
