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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TecnicalServiceStation.Forms;

namespace TecnicalServiceStation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private TSSEntities _TSS = new TSSEntities();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void _Authorize_Clicked(object sender, RoutedEventArgs e)
        {
            
            //MetroWindow window = new Clients();
            //MetroWindow window = new Positions();
            //MetroWindow window = new Workers();
            //MetroWindow window = new Services();
            //MetroWindow window = new Details();
            //MetroWindow window = new Units();
            //MetroWindow window = new Cars();
            //MetroWindow window = new CarCopies();
            //MetroWindow window = new OrganizationForm();
            //OrderDetails window = new OrderDetails();
            
            Worker Worker = this._TSS.Worker.Where(
                    worker => worker.Surname == this._TextBox_Login.Text &&
                        worker.ID + worker.Surname == this._TextBox_Password.Text
                    ).FirstOrDefault();
            if (Worker != null)
            {

                Orders window = new Orders(Worker.ID);
                window.Show();
            }
            else
            {
                MessageBox.Show("Ошибка авторизации");
            }
           
        }
    }
}
