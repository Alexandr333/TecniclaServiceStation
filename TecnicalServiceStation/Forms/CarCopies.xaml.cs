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
    /// Логика взаимодействия для CarCopies.xaml
    /// </summary>
    public partial class CarCopies : MetroWindow
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
                    this._Button_AddCar.Visibility = Visibility.Visible;
                    this._Button_SaveCar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._Button_AddCar.Visibility = Visibility.Collapsed;
                    this._Button_SaveCar.Visibility = Visibility.Visible;
                }
            }
        }
        //
        private TSSEntities _TSS;
        private ObservableCollection<Car> _filtredCars;
        private ObservableCollection<Car> _FiltredCars
        {
            get
            {
                return this._filtredCars;
            }
            set
            {
                this._filtredCars = value;
                this._List_Cars.ItemsSource = this._filtredCars;
                this._List_Cars.Items.Refresh();
            }
        }
        private Car _carForWork;
        private Car _CarForWork
        {
            get
            {
                return this._carForWork;
            }
            set
            {
                this._carForWork = value;
                this._Flyout_ForEntity.DataContext = this._carForWork;
            }
        }
        private List<Car> _SelectedCars
        {
            get
            {
                List<Car> Cars = new List<Car>();
                if (this._List_Cars.SelectedItem != null)
                {
                    foreach (Car car in this._List_Cars.SelectedItems)
                    {
                        Cars.Add(car);
                    }
                }
                return Cars;
            }
        }
        private Car _SelectedCar
        {
            get
            {
                return (Car)this._List_Cars.SelectedItem;
            }
        }
        //
        public CarCopies()
        {
            InitializeComponent();
            this._ModeEnabled_workWithEntity = false;
            this._Flyout_ForEntity.ClosingFinished += this.Clicked_CancelWorkWithEntity;
            this._TSS = new TSSEntities();
            this._FiltredCars = this._FilterCars();
        }
        public CarCopies(bool ForCarSelection,TSSEntities entities)
        {
            InitializeComponent();
            this._TSS = entities;
            this._Button_SelectCarCopy.Visibility = Visibility.Visible;
            this._ModeEnabled_workWithEntity = false;
            this._Flyout_ForEntity.ClosingFinished += this.Clicked_CancelWorkWithEntity;
            this._FiltredCars = this._FilterCars();
        }

        private void Clicked_AddCar(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Insert(this._carForWork, this._TSS);
                this._FiltredCars = this._FilterCars();
                this._ModeEnabled_workWithEntity = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка добавления");
            }
        }

        private void Clicked_SaveCarChanges(object sender, RoutedEventArgs e)
        {

            try
            {
                Repository.Update(this._carForWork, this._TSS);
                this._ModeEnabled_workWithEntity = false;
                this._List_Cars.Items.Refresh();
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
            this._carForWork = new Car();
            this._Flyout_ForEntity.DataContext = this._carForWork;
        }

        private void Clicked_Delete(object sender, RoutedEventArgs e)
        {
            List<Car> CarsForDelete = this._SelectedCars;
            if (CarsForDelete.Count == 0)
            {
                MessageBox.Show("Выберите клиента");
            }
            else
            {
                try
                {
                    foreach (Car car in CarsForDelete)
                    {
                        Repository.Delete(car, this._TSS);
                        this._FiltredCars.Remove(car);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка удаления");
                }
            }
        }

        private void Clicked_Change(object sender, RoutedEventArgs e)
        {
            if (this._SelectedCar != null)
            {
                this._ModeEnabled_workWithEntity = true;
                this._Mode_workWithEntity = WorkWithEntityMode.Change;

                this._carForWork = _SelectedCar;
                this._Flyout_ForEntity.DataContext = this._carForWork;
            }
            else
            {
                MessageBox.Show("Выберите клиента");
            }
        }

        private void Clicked_Search(object sender, RoutedEventArgs e)
        {
            this._FiltredCars = this._FilterCars();
        }

        private ObservableCollection<Car> _FilterCars()
        {
            string ID = this._Field_Search_VIN.Text.Trim();
            string owner = this._Field_Search_Owner.Text.Trim();
            IQueryable<Car> request;
            request = this._TSS.Car.Where(
                car => car.VIN.ToString().Contains(ID)
            ).
            Where(
                car => car.Owner.ToString().Contains(owner)
            );
            return new ObservableCollection<Car>(request.ToList());
        }

        private void Clicked_SelectCarModel(object sender, RoutedEventArgs e)
        {
            Cars window = new Cars(true);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                this._CarForWork.CarModel = this._TSS.CarModel.Where(
                    carModel => carModel.ID == window.SelectedCarModel.ID
                ).FirstOrDefault();
                this._CarForWork = this._CarForWork;
                this._Flyout_ForEntity.DataContext = null;
                this._Flyout_ForEntity.DataContext = this._CarForWork;
            }
            
        }

        //-- --//
        public Car SelectedCar
        {
            get
            {
                return this._SelectedCar;
            }
        }
        private void Clicked_SelectCarCopy(object sender, RoutedEventArgs e)
        {
            if (this._SelectedCar != null)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Выберите автомобиль");
            }
        }
    }
}
