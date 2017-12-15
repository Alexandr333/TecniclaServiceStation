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
    public partial class Cars : MetroWindow
    {
        private TSSEntities _TSS = new TSSEntities();
        //
        private bool _ModeEnabled_workWithCarModelCategories
        {
            get
            {
                return this._Flyout_ForCarModelCategories.IsOpen;
            }
            set
            {
                this._Flyout_ForCarModelCategories.IsOpen = value;
            }
        }
        private bool _ModeEnabled_workWithCarModels
        {
            get
            {
                return this._Flyout_ForCarModel.IsOpen;
            }
            set
            {
                this._Flyout_ForCarModel.IsOpen = value;
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
                    this._Button_AddCarMark.Visibility = Visibility.Visible;
                    this._Button_SaveCarMark.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._Button_AddCarMark.Visibility = Visibility.Collapsed;
                    this._Button_SaveCarMark.Visibility = Visibility.Visible;
                }
            }
        }
        private WorkWithEntityMode _workModeCarModel;
        private WorkWithEntityMode _Mode_workWithCarModel
        {
            get
            {
                return this._workModeCarModel;
            }
            set
            {
                this._workModeCarModel = value;
                if (value == WorkWithEntityMode.Adding)
                {
                    this._Button_AddCarModel.Visibility = Visibility.Visible;
                    this._Button_SaveCarModel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._Button_AddCarModel.Visibility = Visibility.Collapsed;
                    this._Button_SaveCarModel.Visibility = Visibility.Visible;
                }
            }
        }
        //
        private ObservableCollection<CarModel> _filtredCarModels;
        private ObservableCollection<CarModel> _FiltredCarModels
        {
            get
            {
                return this._filtredCarModels;
            }
            set
            {
                this._filtredCarModels = value;
                this._List_CarModels.ItemsSource = this._filtredCarModels;
                this._List_CarModels.Items.Refresh();
            }
        }
        private CarModel _carModelForWork;
        private CarModel _CarModelForWork
        {
            get
            {
                return this._carModelForWork;
            }
            set
            {
                this._carModelForWork = value;
                this._Flyout_ForCarModel.DataContext = this._carModelForWork;
            }
        }
        private List<CarModel> _SelectedCarModels
        {
            get
            {
                List<CarModel> CarModels = new List<CarModel>();
                if (this._List_CarModels.SelectedItem != null)
                {
                    foreach (CarModel service in this._List_CarModels.SelectedItems)
                    {
                        CarModels.Add(service);
                    }
                }
                return CarModels;
            }
        }
        private CarModel _SelectedCarModel
        {
            get
            {
                return (CarModel)this._List_CarModels.SelectedItem;
            }
        }
        public CarModel SelectedCarModel
        {
            get
            {
                return (CarModel)this._List_CarModels.SelectedItem;
            }
        }
        //
        private ObservableCollection<CarMark> _filtredCarMarks;
        private ObservableCollection<CarMark> _FiltredCarMarks
        {
            get
            {
                return this._filtredCarMarks;
            }
            set
            {
                this._filtredCarMarks = value;
                this._List_CarMarks.ItemsSource = this._filtredCarMarks;
                this._List_CarMarks.Items.Refresh();
            }
        }
        private CarMark _carMarkForWork;
        private CarMark _CarMarkForWork
        {
            get
            {
                return this._carMarkForWork;
            }
            set
            {
                this._carMarkForWork = value;
                this._Flyout_ForCarModelCategories.DataContext = this._carMarkForWork;
            }
        }
        private List<CarMark> _SelectedCarMarks
        {
            get
            {
                List<CarMark> CarMarks = new List<CarMark>();
                if (this._List_CarMarks.SelectedItem != null)
                {
                    foreach (CarMark carMark in this._List_CarMarks.SelectedItems)
                    {
                        CarMarks.Add(carMark);
                    }
                }
                return CarMarks;
            }
        }
        private CarMark _SelectedCarMark
        {
            get
            {
                return (CarMark)this._List_CarMarks.SelectedItem;
            }
        }
        //
        public Cars()
        {
            InitializeComponent();
            this._ModeEnabled_workWithCarModelCategories = false;
            this._Flyout_ForCarModelCategories.ClosingFinished += this.Clicked_CancelWorkWithEntity;
            this._FiltredCarMarks = this._FilterCarMarks();
        }
        public Cars(bool selectCarMode): this()
        {
            this._Button_SelectModel.Visibility = Visibility.Visible;
        }

        private void Clicked_AddCarMark(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Insert(this._carMarkForWork, this._TSS);
                this._FiltredCarMarks = this._FilterCarMarks();
                this._ModeEnabled_workWithCarModelCategories = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка добавления");
            }
        }

        private void Clicked_SaveCarMarkChanges(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Update(this._carMarkForWork, this._TSS);
                this._ModeEnabled_workWithCarModelCategories = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка изменения");
            }
        }

        private void Clicked_CancelWorkWithEntity(object sender, RoutedEventArgs e)
        {
            this._ModeEnabled_workWithCarModelCategories = false;
            this._ModeEnabled_workWithCarModels = false;
            Repository.UndoingChangesDbContextLevel(this._TSS);
        }

        private void Clicked_Search(object sender, RoutedEventArgs e)
        {
            this._FiltredCarMarks = this._FilterCarMarks();
        }

        private void Clicked_ChangeCategory(object sender, RoutedEventArgs e)
        {
            if (this._SelectedCarMark != null)
            {
                this._ModeEnabled_workWithCarModelCategories = true;
                this._Mode_workWithCategory = WorkWithEntityMode.Change;

                this._carMarkForWork = _SelectedCarMark;
                this._Flyout_ForCarModelCategories.DataContext = this._carMarkForWork;
            }
            else
            {
                MessageBox.Show("Выберите категорию");
            }
        }

        private void Clicked_DeleteCategory(object sender, RoutedEventArgs e)
        {
            List<CarMark> CarMarksForDelete = this._SelectedCarMarks;
            if (CarMarksForDelete.Count == 0)
            {
                MessageBox.Show("Выберите категорию");
            }
            else
            {
                try
                {
                    foreach (CarMark carMark in CarMarksForDelete)
                    {
                        Repository.Delete(carMark, this._TSS);
                        this._FiltredCarMarks.Remove(carMark);
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
            this._ModeEnabled_workWithCarModelCategories = true;
            this._Mode_workWithCategory = WorkWithEntityMode.Adding;
            this._carMarkForWork = new CarMark();
            this._Flyout_ForCarModelCategories.DataContext = this._carMarkForWork;
        }

        private ObservableCollection<CarMark> _FilterCarMarks()
        {
            /*string Surname = this._Field_Search_Surname.Text.Trim();
            string Name = this._Field_Search_Name.Text.Trim();
            string PatronymicName = this._Field_Search_PatronymicName.Text.Trim();
            string ID = this._Field_Search_ID.Text.Trim();
            IQueryable<CarMark> request;
            request = this._TSS.CarMark.Where(
                carMark => carMark.ID.ToString().Contains(ID)
            );
            return new ObservableCollection<CarMark>(request.ToList());*/
            return new ObservableCollection<CarMark>(this._TSS.CarMark.ToList());
        }
        //-- --//
        private void SelectionChanged_CarMark(object sender, SelectionChangedEventArgs e)
        {
            if (this._SelectedCarMark != null)
            {
                this._FiltredCarModels = new ObservableCollection<CarModel>(this._SelectedCarMark.CarModel.ToList());
            }
        }

        private void Clicked_NewCarModel(object sender, RoutedEventArgs e)
        {
            this._ModeEnabled_workWithCarModels = true;
            this._Mode_workWithCarModel = WorkWithEntityMode.Adding;
            this._CarModelForWork = new CarModel();

            if (this._SelectedCarMark != null)
            {
                this._CarModelForWork = new CarModel()
                {
                    CarMark = this._SelectedCarMark
                };
            }
            this._Flyout_ForCarModel.DataContext = this._CarModelForWork;
            this._ComboBox_CarMark.ItemsSource = this._TSS.CarMark.ToList();
        }

        private void Clicked_DeleteCarModel(object sender, RoutedEventArgs e)
        {
            List<CarModel> CarModelsForDelete = this._SelectedCarModels;
            if (CarModelsForDelete.Count == 0)
            {
                MessageBox.Show("Выберите деталь");
            }
            else
            {
                try
                {
                    foreach (CarModel service in CarModelsForDelete)
                    {
                        Repository.Delete(service, this._TSS);
                        this._FiltredCarModels.Remove(service);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка удаления");
                }
            }
        }

        private void Clicked_ChangeCarModel(object sender, RoutedEventArgs e)
        {
            if (this._SelectedCarModel != null)
            {
                if (this._SelectedCarMark != null)
                {
                    this._CarModelForWork = new CarModel()
                    {
                        CarMark = this._SelectedCarMark
                    };
                }
                this._ComboBox_CarMark.ItemsSource = this._TSS.CarMark.ToList();
                this._ModeEnabled_workWithCarModels = true;
                this._Mode_workWithCarModel = WorkWithEntityMode.Change;

                this._carModelForWork = _SelectedCarModel;
                this._Flyout_ForCarModel.DataContext = this._carModelForWork;
            }
            else
            {
                MessageBox.Show("Выберите деталь");
            }
        }

        private void Clicked_AddCarModel(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Insert(this._carModelForWork, this._TSS);
                this._FiltredCarModels = this._FilterCarModels();
                this._ModeEnabled_workWithCarModels = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка добавления");
            }
        }

        private void _Button_SaveCarModelChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Update(this._carMarkForWork, this._TSS);
                this._ModeEnabled_workWithCarModels = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка изменения");
            }
        }

        private ObservableCollection<CarModel> _FilterCarModels()
        {
            return new ObservableCollection<CarModel>(this._SelectedCarMark.CarModel.ToList());
        }

        private void Clicked_OpenMenu(object sender, RoutedEventArgs e)
        {
            this._Flyout_ForMenu.IsOpen = true;
        }

        private void Clicked_SelectModel(object sender, RoutedEventArgs e)
        {
            if (this._SelectedCarModel != null)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Необходимо выбрать модель");
            }
        }
    }
}
