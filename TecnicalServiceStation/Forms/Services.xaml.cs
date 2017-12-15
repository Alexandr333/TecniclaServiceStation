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
    /// Логика взаимодействия для Services.xaml
    /// </summary>
    public partial class Services : MetroWindow
    {
        private TSSEntities _TSS;
        //
        private bool _ModeEnabled_workWithServiceCategories
        {
            get
            {
                return this._Flyout_ForServiceCategories.IsOpen;
            }
            set
            {
                this._Flyout_ForServiceCategories.IsOpen = value;
            }
        }
        private bool _ModeEnabled_workWithServices
        {
            get
            {
                return this._Flyout_ForService.IsOpen;
            }
            set
            {
                this._Flyout_ForService.IsOpen = value;
            }
        }
        private WorkWithEntityMode _workModeCategory;
        private WorkWithEntityMode _Mode_workWithCategory
        {
            get
            {
                return this._workModeCategory;
            }
            set
            {
                this._workModeCategory = value;
                if (value == WorkWithEntityMode.Adding)
                {
                    this._Button_AddServiceCategory.Visibility = Visibility.Visible;
                    this._Button_SaveServiceCategory.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._Button_AddServiceCategory.Visibility = Visibility.Collapsed;
                    this._Button_SaveServiceCategory.Visibility = Visibility.Visible;
                }
            }
        }
        private WorkWithEntityMode _workModeService;
        private WorkWithEntityMode _Mode_workWithService
        {
            get
            {
                return this._workModeService;
            }
            set
            {
                this._workModeService = value;
                if (value == WorkWithEntityMode.Adding)
                {
                    this._Button_AddService.Visibility = Visibility.Visible;
                    this._Button_SaveService.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._Button_AddService.Visibility = Visibility.Collapsed;
                    this._Button_SaveService.Visibility = Visibility.Visible;
                }
            }
        }
        //
        private ObservableCollection<Service> _filtredServices;
        private ObservableCollection<Service> _FiltredServices
        {
            get
            {
                return this._filtredServices;
            }
            set
            {
                this._filtredServices = value;
                this._List_Services.ItemsSource = this._filtredServices;
                this._List_Services.Items.Refresh();
            }
        }
        private Service _serviceForWork;
        private Service _ServiceForWork
        {
            get
            {
                return this._serviceForWork;
            }
            set
            {
                this._serviceForWork = value;
                this._Flyout_ForService.DataContext = this._serviceForWork;
            }
        }
        private List<Service> _SelectedServices
        {
            get
            {
                List<Service> Services = new List<Service>();
                if (this._List_Services.SelectedItem != null)
                {
                    foreach (Service service in this._List_Services.SelectedItems)
                    {
                        Services.Add(service);
                    }
                }
                return Services;
            }
        }
        private Service _SelectedService
        {
            get
            {
                return (Service)this._List_Services.SelectedItem;
            }
        }
        //
        private ObservableCollection<ServiceCategory> _filtredServiceCategorys;
        private ObservableCollection<ServiceCategory> _FiltredServiceCategorys
        {
            get
            {
                return this._filtredServiceCategorys;
            }
            set
            {
                this._filtredServiceCategorys = value;
                this._List_ServiceCategorys.ItemsSource = this._filtredServiceCategorys;
                this._List_ServiceCategorys.Items.Refresh();
            }
        }
        private ServiceCategory _serviceCategoryForWork;
        private ServiceCategory _ServiceCategoryForWork
        {
            get
            {
                return this._serviceCategoryForWork;
            }
            set
            {
                this._serviceCategoryForWork = value;
                this._Flyout_ForServiceCategories.DataContext = this._serviceCategoryForWork;
            }
        }
        private List<ServiceCategory> _SelectedServiceCategorys
        {
            get
            {
                List<ServiceCategory> ServiceCategorys = new List<ServiceCategory>();
                if (this._List_ServiceCategorys.SelectedItem != null)
                {
                    foreach (ServiceCategory serviceCategory in this._List_ServiceCategorys.SelectedItems)
                    {
                        ServiceCategorys.Add(serviceCategory);
                    }
                }
                return ServiceCategorys;
            }
        }
        private ServiceCategory _SelectedServiceCategory
        {
            get
            {
                return (ServiceCategory)this._List_ServiceCategorys.SelectedItem;
            }
        }
        //
        public Services()
        {
            InitializeComponent();
            this._TSS = new TSSEntities();
            this._ModeEnabled_workWithServiceCategories = false;
            this._Flyout_ForServiceCategories.ClosingFinished += this.Clicked_CancelWorkWithEntity;
            this._FiltredServiceCategorys = this._FilterServiceCategorys();
        }
        public Services(bool forServiceSelect, TSSEntities entities)
        {
            InitializeComponent();
            this._Button_SelectServiceCategory.Visibility = Visibility.Visible;
            this._TSS = entities;
            this._ModeEnabled_workWithServiceCategories = false;
            this._Flyout_ForServiceCategories.ClosingFinished += this.Clicked_CancelWorkWithEntity;
            this._FiltredServiceCategorys = this._FilterServiceCategorys();
        }

        private void Clicked_AddServiceCategory(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Insert(this._serviceCategoryForWork, this._TSS);
                this._FiltredServiceCategorys = this._FilterServiceCategorys();
                this._ModeEnabled_workWithServiceCategories = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка добавления");
            }
        }

        private void Clicked_SaveServiceCategoryChanges(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Update(this._serviceCategoryForWork, this._TSS);
                this._ModeEnabled_workWithServiceCategories = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка изменения");
            }
        }

        private void Clicked_CancelWorkWithEntity(object sender, RoutedEventArgs e)
        {
            this._ModeEnabled_workWithServiceCategories = false;
            this._ModeEnabled_workWithServices = false;
            Repository.UndoingChangesDbContextLevel(this._TSS);
        }

        private void Clicked_Search(object sender, RoutedEventArgs e)
        {
            this._FiltredServiceCategorys = this._FilterServiceCategorys();
        }

        private void Clicked_ChangeCategory(object sender, RoutedEventArgs e)
        {
            if (this._SelectedServiceCategory != null)
            {
                this._ModeEnabled_workWithServiceCategories = true;
                this._Mode_workWithCategory = WorkWithEntityMode.Change;

                this._serviceCategoryForWork = _SelectedServiceCategory;
                this._Flyout_ForServiceCategories.DataContext = this._serviceCategoryForWork;
            }
            else
            {
                MessageBox.Show("Выберите категорию");
            }
        }

        private void Clicked_DeleteCategory(object sender, RoutedEventArgs e)
        {
            List<ServiceCategory> ServiceCategorysForDelete = this._SelectedServiceCategorys;
            if (ServiceCategorysForDelete.Count == 0)
            {
                MessageBox.Show("Выберите категорию");
            }
            else
            {
                try
                {
                    foreach (ServiceCategory serviceCategory in ServiceCategorysForDelete)
                    {
                        Repository.Delete(serviceCategory, this._TSS);
                        this._FiltredServiceCategorys.Remove(serviceCategory);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка удаления");
                }
            }
        }

        private void Clicked_NewCategory(object sender, RoutedEventArgs e)
        {
            this._ModeEnabled_workWithServiceCategories = true;
            this._Mode_workWithCategory = WorkWithEntityMode.Adding;
            this._serviceCategoryForWork = new ServiceCategory();
            this._Flyout_ForServiceCategories.DataContext = this._serviceCategoryForWork;
        }

        private ObservableCollection<ServiceCategory> _FilterServiceCategorys()
        {
            /*string Surname = this._Field_Search_Surname.Text.Trim();
            string Name = this._Field_Search_Name.Text.Trim();
            string PatronymicName = this._Field_Search_PatronymicName.Text.Trim();
            string ID = this._Field_Search_ID.Text.Trim();
            IQueryable<ServiceCategory> request;
            request = this._TSS.ServiceCategory.Where(
                serviceCategory => serviceCategory.ID.ToString().Contains(ID)
            );
            return new ObservableCollection<ServiceCategory>(request.ToList());*/
            return new ObservableCollection<ServiceCategory>(this._TSS.ServiceCategory.ToList());
        }

        //-- --//
        private void SelectionChanged_ServiceCategory(object sender, SelectionChangedEventArgs e)
        {
            if (this._SelectedServiceCategory != null)
            {
                this._FiltredServices = new ObservableCollection<Service>(this._SelectedServiceCategory.Service.ToList());
            }
        }

        private void Clicked_NewService(object sender, RoutedEventArgs e)
        {
            this._ModeEnabled_workWithServices = true;
            this._Mode_workWithService = WorkWithEntityMode.Adding;
            this._ServiceForWork = new Service();

            if (this._SelectedServiceCategory != null)
            {
                this._ServiceForWork = new Service()
                {
                    ServiceCategory = this._SelectedServiceCategory
                };
            }
            this._Flyout_ForService.DataContext = this._ServiceForWork;
            this._ComboBox_ServiceCategory.ItemsSource = this._TSS.ServiceCategory.ToList();
            
        }

        private void Clicked_DeleteService(object sender, RoutedEventArgs e)
        {
            List<Service> ServicesForDelete = this._SelectedServices;
            if (ServicesForDelete.Count == 0)
            {
                MessageBox.Show("Выберите услугу");
            }
            else
            {
                try
                {
                    foreach (Service service in ServicesForDelete)
                    {
                        Repository.Delete(service, this._TSS);
                        this._FiltredServices.Remove(service);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка удаления");
                }
            }
        }

        private void Clicked_ChangeService(object sender, RoutedEventArgs e)
        {
            if (this._SelectedService != null)
            {
                if (this._SelectedServiceCategory != null)
                {
                    this._ServiceForWork = new Service()
                    {
                        ServiceCategory = this._SelectedServiceCategory
                    };
                }

                this._ModeEnabled_workWithServices = true;
                this._Mode_workWithService = WorkWithEntityMode.Change;

                this._serviceForWork = _SelectedService;
                this._Flyout_ForService.DataContext = this._serviceForWork;
            }
            else
            {
                MessageBox.Show("Выберите услугу");
            }
        }

        private void Clicked_AddService(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Insert(this._serviceForWork, this._TSS);
                this._FiltredServices = this._FilterServices();
                this._ModeEnabled_workWithServices = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка добавления");
            }
        }

        private void _Button_SaveServiceChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Update(this._serviceCategoryForWork, this._TSS);
                this._ModeEnabled_workWithServices = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка изменения");
            }
        }

        private ObservableCollection<Service> _FilterServices()
        {
            return new ObservableCollection<Service>(this._SelectedServiceCategory.Service.ToList());
        }

        //-- --//
        public Service SelectedService 
        {
            get
            {
                return this._SelectedService;
            }
        }

        private void Clicked_SelectService(object sender, RoutedEventArgs e)
        {
            if (this._SelectedService != null)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Выберите услугу");
            }
        }
    }
}
