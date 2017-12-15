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
    /// Логика взаимодействия для OrderDetails.xaml
    /// </summary>
    public partial class OrderDetails : MetroWindow
    {
        private TSSEntities _TSS = new TSSEntities();
        private OrderSpareParts _OrderDetails;
        private WorkWithEntityMode _WorkMode
        {
            set
            {
                if (value == WorkWithEntityMode.Adding)
                {
                    this._Button_AddOrderSparePart.Visibility = Visibility.Visible;
                    this._Button_SaveOrderSparePart.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._Button_AddOrderSparePart.Visibility = Visibility.Collapsed;
                    this._Button_SaveOrderSparePart.Visibility = Visibility.Visible;
                }
            }
        }
        public OrderSpareParts OrderSparePart
        {
            get
            {
                return this._OrderDetails;
            }
        }

        public OrderDetails(TSSEntities entities)
        {
            InitializeComponent();
            _WorkMode = WorkWithEntityMode.Adding;
            _OrderDetails = new OrderSpareParts();
            _OrderDetails.Number = 1;
            _OrderDetails.Own = true;
            this.DataContext = _OrderDetails;
            this._TSS = entities;
        }
        public OrderDetails(OrderSpareParts details, TSSEntities entities)
        {
            InitializeComponent();
            _WorkMode = WorkWithEntityMode.Change;
            this._OrderDetails = details;
            this.DataContext = _OrderDetails;
            this._ContentControl_SparePart.Content = _OrderDetails.SparePart;
            this._TSS = entities;
        }

        private void Clicked_SelectSparePart(object sender, RoutedEventArgs e)
        {
            Details window = new Details(true,this._TSS);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                if (window.SelectedSparePart != null)
                {
                    this._OrderDetails.SparePart = this._TSS.SparePart.Where(
                        sparePart => sparePart.ID == window.SelectedSparePart.ID
                    ).First();
                    this._ContentControl_SparePart.Content = this._OrderDetails.SparePart;
                }
            }
        }

        private void Clicked_Add(object sender, RoutedEventArgs e)
        {
            string message =this._CheckEntity();
            if (message == "")
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(message);
            }
            
        }

        private void Clicked_Save(object sender, RoutedEventArgs e)
        {
            string message = this._CheckEntity();
            if (message == "")
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(message);
            }
        }

        private string _CheckEntity()
        {
            string Message = "";
            if (this._OrderDetails.SparePart == null)
            {
                Message += "Необходимо выбрать деталь";
            } 
            if (this._OrderDetails.Number <= 0)
            {
                Message += "Количество деталей должно быть большим нуля";
            }
            return Message;
        }
    }
}
