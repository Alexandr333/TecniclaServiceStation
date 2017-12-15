using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для Form_Clients.xaml
    /// </summary>
    public partial class Clients : MetroWindow
    {
        private bool _ModeEnabled_workWithEntity
        {
            get
            {
                return this._Flyout_ForEntity.IsOpen;
            }
            set
            {
                this._Flyout_ForEntity.IsOpen = value;
            }
        }
        private WorkWithEntityMode _workMode;
        private WorkWithEntityMode _Mode_workWithEntity
        {
            get
            {
                return this._workMode;
            }
            set
            {
                this._workMode = value;
                if (value == WorkWithEntityMode.Adding)
                {
                    this._Button_AddClient.Visibility = Visibility.Visible;
                    this._Button_SaveClient.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._Button_AddClient.Visibility = Visibility.Collapsed;
                    this._Button_SaveClient.Visibility = Visibility.Visible;
                }
            }
        }
        //
        private TSSEntities _TSS;
        private ObservableCollection<Client> _filtredClients;
        private ObservableCollection<Client> _FiltredClients
        {
            get
            {
                return this._filtredClients;
            }
            set
            {
                this._filtredClients = value;
                this._List_Clients.ItemsSource = this._filtredClients;
                this._List_Clients.Items.Refresh();
            }
        }
        private Client _clientForWork;
        private Client _ClientForWork
        {
            get
            {
                return this._clientForWork;
            }
            set
            {
                this._clientForWork = value;
                this._Flyout_ForEntity.DataContext = this._clientForWork;
            }
        }
        private List<Client> _SelectedClients
        {
            get
            {
                List<Client> clients = new List<Client>();
                if (this._List_Clients.SelectedItem != null)
                {
                    foreach(Client client in this._List_Clients.SelectedItems)
                    {
                        clients.Add(client);
                    }
                }
                return clients;
            }
        }
        private Client _SelectedClient
        {
            get
            {
                return (Client)this._List_Clients.SelectedItem;
            }
        }
        //
        public Clients()
        {
            InitializeComponent();
            this._ModeEnabled_workWithEntity = false;
            this._Flyout_ForEntity.ClosingFinished += this.Clicked_CancelWorkWithEntity;
            this._TSS = new TSSEntities();
            this._FiltredClients = this._FilterClients();
        }
        public Clients(bool ForCarSelection, TSSEntities entities)
            : this()
        {
            InitializeComponent();
            this._ModeEnabled_workWithEntity = false;
            this._Flyout_ForEntity.ClosingFinished += this.Clicked_CancelWorkWithEntity;
            this._Button_SelectClient.Visibility = Visibility.Visible;
            this._TSS = entities;
            this._FiltredClients = this._FilterClients();
        }

        private void Clicked_AddClient(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Insert(this._clientForWork, this._TSS);
                this._FiltredClients.Add(this._clientForWork);
                this._ModeEnabled_workWithEntity = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка добавления");
            }
        }

        private void Clicked_SaveClientChanges(object sender, RoutedEventArgs e)
        {
            
            try
            {
                Repository.Update(this._clientForWork, this._TSS);
                this._ModeEnabled_workWithEntity = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка изменения");
            }
        }

        private void Clicked_CancelWorkWithEntity(object sender, RoutedEventArgs e)
        {
            this._ModeEnabled_workWithEntity = false;
            Repository.UndoingChangesDbContextLevel(this._TSS);
        }

        private void Clicked_New(object sender, RoutedEventArgs e)
        {
            this._ModeEnabled_workWithEntity = true;
            this._Mode_workWithEntity = WorkWithEntityMode.Adding;
            this._clientForWork = new Client();
            this._Flyout_ForEntity.DataContext = this._clientForWork;
        }

        private void Clicked_Delete(object sender, RoutedEventArgs e)
        {
            List<Client> clientsForDelete = this._SelectedClients;
            if (clientsForDelete.Count == 0)
            {
                MessageBox.Show("Выберите клиента");
            }
            else
            {
                try
                {
                    foreach (Client client in clientsForDelete)
                    {
                        Repository.Delete(client, this._TSS);
                        this._filtredClients.Remove(client);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка удаления");
                }
            }
            this._List_Clients.Items.Refresh();
        }

        private void Clicked_Change(object sender, RoutedEventArgs e)
        {
            if (this._SelectedClient != null)
            {
                this._ModeEnabled_workWithEntity = true;
                this._Mode_workWithEntity = WorkWithEntityMode.Change;

                this._clientForWork = _SelectedClient;
                this._Flyout_ForEntity.DataContext = this._clientForWork;
            }
            else
            {
                MessageBox.Show("Выберите клиента");
            }
        }

        private void Clicked_Search(object sender, RoutedEventArgs e)
        {
            this._FiltredClients = this._FilterClients();
        }

        private ObservableCollection<Client> _FilterClients()
        {
            string Surname = this._Field_Search_Surname.Text.Trim();
            string Name = this._Field_Search_Name.Text.Trim();
            string PatronymicName = this._Field_Search_PatronymicName.Text.Trim();
            string PassportID = this._Field_Search_PasspornID.Text.Trim();
            IQueryable<Client> request;
            request = this._TSS.Client.Where(
                client => client.Surname.Contains(Surname)
            ).Where(
                client => client.Name.Contains(Name)
            ).Where(
                client => client.PatronymicName.Contains(PatronymicName)
            ).Where(
                client => client.PassportNumber.Contains(PassportID)
            );
            return new ObservableCollection<Client>(request.ToList());
        }

        public Client SelectedClient 
        {
            get
            {
                return this._SelectedClient;
            }
        }

        private void Clicked_SelectClient(object sender, RoutedEventArgs e)
        {
            if (this._SelectedClient != null)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Выберите клмента");
            }
        }


    }

}
