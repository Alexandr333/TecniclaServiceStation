using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
    /// Логика взаимодействия для Orders.xaml
    /// </summary>
    public partial class Orders : MetroWindow
    {
        #region variable declaration
        private TSSEntities _TSS = new TSSEntities();
        //
        private bool _ModeEnabled_workWithOrders
        {
            get
            {
                return this._Flyout_ForWorkWithOrder.IsOpen;
            }
            set
            {
                this._Flyout_ForWorkWithOrder.IsOpen = value;
            }
        }
        private WorkWithEntityMode _workModeOrder;
        private WorkWithEntityMode _Mode_workWithOrder
        {
            get
            {
                return this._workModeOrder;
            }
            set
            {
                this._workModeOrder = value;
                if (value == WorkWithEntityMode.Adding)
                {
                    this._Button_AddOrder.Visibility = Visibility.Visible;
                    this._Button_SaveOrder.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._Button_AddOrder.Visibility = Visibility.Collapsed;
                    this._Button_SaveOrder.Visibility = Visibility.Visible;
                }
            }
        }
        //
        private ObservableCollection<Order> _filtredOrders;
        private ObservableCollection<Order> _FiltredOrders
        {
            get
            {
                return this._filtredOrders;
            }
            set
            {
                this._filtredOrders = value;
                this._List_Orders.ItemsSource = this._filtredOrders;
                this._List_Orders.Items.Refresh();
            }
        }
        private Order _orderForWork;
        private Order _OrderForWork
        {
            get
            {
                return this._orderForWork;
            }
            set
            {
                this._orderForWork = value;
                this._Flyout_ForWorkWithOrder.DataContext = this._orderForWork;
            }
        }
        private List<Order> _SelectedOrders
        {
            get
            {
                List<Order> Orders = new List<Order>();
                if (this._List_Orders.SelectedItem != null)
                {
                    foreach (Order service in this._List_Orders.SelectedItems)
                    {
                        Orders.Add(service);
                    }
                }
                return Orders;
            }
        }
        private Order _SelectedOrder
        {
            get
            {
                return (Order)this._List_Orders.SelectedItem;
            }
        }
        public Order SelectedOrder
        {
            get
            {
                return (Order)this._List_Orders.SelectedItem;
            }
        }

        private Worker _CurrentWorker;
        //
        #endregion variable declaration

        public Orders()
        {
            InitializeComponent();
            this._FiltredOrders = this._FilterOrders();
        }
        public Orders(int workerID): this()
        {
            this._CurrentWorker = this._TSS.Worker.Where(
                worker => worker.ID == workerID
                ).First();
        }

        private ObservableCollection<Order> _FilterOrders()
        {
            IQueryable<Order> result;
            result = this._TSS.Order.Where(
                order => order.ID.ToString().Contains(this._TextBox_ID.Text)
            );
            if (this._CheckBox_UnfulfilledOnly.IsChecked == true)
            {
                result.Where(
                     order => order.Status == false
                );
            }
            if (this._CheckBox_OldFirst.IsChecked == true)
            {
                result.OrderBy(order => order.ReceiptDate);
            }
            return new ObservableCollection<Order>(result.ToList());
        }

        #region OrderActions
        private void Clicked_NewOrder(object sender, RoutedEventArgs e)
        {
            this._Mode_workWithOrder = WorkWithEntityMode.Adding;
            this._ModeEnabled_workWithOrders = true;
            Order order = new Order();
            order.OrderService = new List<OrderService>();
            order.OrderSpareParts = new List<OrderSpareParts>();
            this._OrderForWork = order;
            this._Flyout_ForWorkWithOrder.DataContext = null;
            this._Flyout_ForWorkWithOrder.DataContext = this._OrderForWork;
        }

        private void Clicked_DeleteOrder(object sender, RoutedEventArgs e)
        {
            List<Order> orderForDelete = this._SelectedOrders;
            if (orderForDelete.Count == 0)
            {
                MessageBox.Show("Необходимо выбрать заказ для удаления");
            }
            else
            {
                try
                {
                    foreach (Order order in orderForDelete)
                    {
                        Repository.Delete(order, this._TSS);
                        this._FiltredOrders.Remove(order);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка удаления");
                }
            }
        }

        private void Clicked_ChangeOrder(object sender, RoutedEventArgs e)
        {
            if (this._SelectedOrder == null)
            {
                MessageBox.Show("Необходимо выбрать заказ");
            }
            else
            {
                this._Mode_workWithOrder = WorkWithEntityMode.Change;
                this._ModeEnabled_workWithOrders = true;
                this._OrderForWork = this._SelectedOrder;
            }
        }

        private void Clicked_AddOrder(object sender, RoutedEventArgs e)
        {
            try
            {
                this._OrderForWork.Worker = this._CurrentWorker;
                this._OrderForWork.ReceiptDate = DateTime.Now;
                Repository.Insert(this._OrderForWork, this._TSS);
                this._FiltredOrders = this._FilterOrders();
                this._ModeEnabled_workWithOrders = false;
                
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка добавления");
            }
        }

        private void Clicked_SaveOrder(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Update(this._OrderForWork, this._TSS);
                this._ModeEnabled_workWithOrders = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка изменения");
            }
        }

        private void Clicked_CancelWorkWithEntity(object sender, RoutedEventArgs e)
        {
            this._ModeEnabled_workWithOrders = false;
            Repository.UndoingChangesDbContextLevel(this._TSS);
            this._List_Orders.Items.Refresh();
        }
        #endregion OrderActions

        #region WorkWithEntityActions

        #region ServiceActions
        private void Clicked_SelectService(object sender, RoutedEventArgs e)
        {
            Services window = new Services(true,this._TSS);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                OrderService orderService = new OrderService();
                orderService.Service = window.SelectedService;
                this._OrderForWork.OrderService.Add(orderService);
                this._Flyout_ForWorkWithOrder.DataContext = null;
                this._Flyout_ForWorkWithOrder.DataContext = this._orderForWork;
            }
        }

        private void Clicked_DeleteService(object sender, RoutedEventArgs e)
        {
            OrderService orderService = (OrderService)this._List_OrderServices.SelectedItem;
            if (orderService != null)
            {
                this._TSS.Entry(orderService).State = EntityState.Deleted;
                this._OrderForWork.OrderService.Remove(orderService);
                this._Flyout_ForWorkWithOrder.DataContext = null;
                this._Flyout_ForWorkWithOrder.DataContext = this._orderForWork;
            }
            else
            {
                MessageBox.Show("Выьерите услугу");
            }
        }
        #endregion ServiceActions

        #region SparePartActions
        private void Clicked_SelectSparePart(object sender, RoutedEventArgs e)
        {
            OrderDetails window = new OrderDetails(this._TSS);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                OrderSpareParts orderSparePart = window.OrderSparePart;
                this._OrderForWork.OrderSpareParts.Add(orderSparePart);
                this._Flyout_ForWorkWithOrder.DataContext = null;
                this._Flyout_ForWorkWithOrder.DataContext = this._orderForWork;
            }
        }

        private void Clicked_DeleteSparePart(object sender, RoutedEventArgs e)
        {
            OrderSpareParts orderSparePart = (OrderSpareParts)this._List_OrderSpareParts.SelectedItem;
            if (orderSparePart != null)
            {
                this._TSS.Entry(orderSparePart).State = EntityState.Deleted;
                this._OrderForWork.OrderSpareParts.Remove(orderSparePart);
                this._Flyout_ForWorkWithOrder.DataContext = null;
                this._Flyout_ForWorkWithOrder.DataContext = this._orderForWork;
            }
            else
            {
                MessageBox.Show("Выьерите деталь");
            }
        }

        private void Clicked_ChangeSparePart(object sender, RoutedEventArgs e)
        {
            OrderSpareParts orderSparePart = (OrderSpareParts)this._List_OrderSpareParts.SelectedItem;
            if (orderSparePart != null)
            {
                OrderDetails window = new OrderDetails(orderSparePart, this._TSS);
                window.ShowDialog();
                if (window.DialogResult == true)
                {
                    orderSparePart = window.OrderSparePart;
                    this._List_OrderSpareParts.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Выберите деталь");
            }
            
        }
        #endregion SparePartActions

        #region ClientActions
        private void Clicked_SelectClient(object sender, RoutedEventArgs e)
        {
            Clients window = new Clients(true, this._TSS);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                if (window.SelectedClient != null)
                {
                    this._OrderForWork.Client = window.SelectedClient;
                    this._Flyout_ForWorkWithOrder.DataContext = null;
                    this._Flyout_ForWorkWithOrder.DataContext = this._orderForWork;
                }
            }
        }
        #endregion ClientActions

        #region CarActions
        private void Clicked_SelectCar(object sender, RoutedEventArgs e)
        {
            CarCopies window = new CarCopies(true, this._TSS);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                if (window.SelectedCar != null)
                {
                    this._OrderForWork.Car = window.SelectedCar;
                    this._Flyout_ForWorkWithOrder.DataContext = null;
                    this._Flyout_ForWorkWithOrder.DataContext = this._orderForWork;
                }
            }
        }
        #endregion CarActions

        private void Clicked_PrintSelectedDocument(object sender, RoutedEventArgs e)
        {
            if (this._SelectedOrder != null)
            {
                ReportsCreator.createOrder(this._SelectedOrder.ID);
            }
            else
            {
                MessageBox.Show("Необходимо выбрать заказ");
            }
        }

        #endregion WorkWithEntityActions
    }
}
