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
using TecnicalServiceStation;
using TecnicalServiceStation.ClassModels;

namespace TecnicalServiceStation.Forms
{
    /// <summary>
    /// Логика взаимодействия для Details.xaml
    /// </summary>
    public partial class Details : MetroWindow
    {
        //
        public SparePart SelectedSparePart
        {
            get
            {
                return this._SelectedSparePart;
            }
        }
        //
        private TSSEntities _TSS = new TSSEntities();
        //
        private bool _ModeEnabled_workWithSparePartCategories
        {
            get
            {
                return this._Flyout_ForSparePartCategories.IsOpen;
            }
            set
            {
                this._Flyout_ForSparePartCategories.IsOpen = value;
            }
        }
        private bool _ModeEnabled_workWithSpareParts
        {
            get
            {
                return this._Flyout_ForSparePart.IsOpen;
            }
            set
            {
                this._Flyout_ForSparePart.IsOpen = value;
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
                    this._Button_AddSparePartCategory.Visibility = Visibility.Visible;
                    this._Button_SaveSparePartCategory.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._Button_AddSparePartCategory.Visibility = Visibility.Collapsed;
                    this._Button_SaveSparePartCategory.Visibility = Visibility.Visible;
                }
            }
        }
        private WorkWithEntityMode _workModeSparePart;
        private WorkWithEntityMode _Mode_workWithSparePart
        {
            get
            {
                return this._workModeSparePart;
            }
            set
            {
                this._workModeSparePart = value;
                if (value == WorkWithEntityMode.Adding)
                {
                    this._Button_AddSparePart.Visibility = Visibility.Visible;
                    this._Button_SaveSparePart.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._Button_AddSparePart.Visibility = Visibility.Collapsed;
                    this._Button_SaveSparePart.Visibility = Visibility.Visible;
                }
            }
        }
        //
        private ObservableCollection<SparePart> _filtredSpareParts;
        private ObservableCollection<SparePart> _FiltredSpareParts
        {
            get
            {
                return this._filtredSpareParts;
            }
            set
            {
                this._filtredSpareParts = value;
                this._List_SpareParts.ItemsSource = this._filtredSpareParts;
                this._List_SpareParts.Items.Refresh();
            }
        }
        private SparePart _sparePartForWork;
        private SparePart _SparePartForWork
        {
            get
            {
                return this._sparePartForWork;
            }
            set
            {
                this._sparePartForWork = value;
                this._Flyout_ForSparePart.DataContext = this._sparePartForWork;
            }
        }
        private List<SparePart> _SelectedSpareParts
        {
            get
            {
                List<SparePart> SpareParts = new List<SparePart>();
                if (this._List_SpareParts.SelectedItem != null)
                {
                    foreach (SparePart service in this._List_SpareParts.SelectedItems)
                    {
                        SpareParts.Add(service);
                    }
                }
                return SpareParts;
            }
        }
        private SparePart _SelectedSparePart
        {
            get
            {
                return (SparePart)this._List_SpareParts.SelectedItem;
            }
        }
        //
        private ObservableCollection<SparePartCategory> _filtredSparePartCategorys;
        private ObservableCollection<SparePartCategory> _FiltredSparePartCategorys
        {
            get
            {
                return this._filtredSparePartCategorys;
            }
            set
            {
                this._filtredSparePartCategorys = value;
                this._List_SparePartCategorys.ItemsSource = this._filtredSparePartCategorys;
                this._List_SparePartCategorys.Items.Refresh();
            }
        }
        private SparePartCategory _sparePartCategoryForWork;
        private SparePartCategory _SparePartCategoryForWork
        {
            get
            {
                return this._sparePartCategoryForWork;
            }
            set
            {
                this._sparePartCategoryForWork = value;
                this._Flyout_ForSparePartCategories.DataContext = this._sparePartCategoryForWork;
            }
        }
        private List<SparePartCategory> _SelectedSparePartCategorys
        {
            get
            {
                List<SparePartCategory> SparePartCategorys = new List<SparePartCategory>();
                if (this._List_SparePartCategorys.SelectedItem != null)
                {
                    foreach (SparePartCategory sparePartCategory in this._List_SparePartCategorys.SelectedItems)
                    {
                        SparePartCategorys.Add(sparePartCategory);
                    }
                }
                return SparePartCategorys;
            }
        }
        private SparePartCategory _SelectedSparePartCategory
        {
            get
            {
                return (SparePartCategory)this._List_SparePartCategorys.SelectedItem;
            }
        }
        //
        public Details()
        {
            InitializeComponent();
            this._ModeEnabled_workWithSparePartCategories = false;
            this._Flyout_ForSparePartCategories.ClosingFinished += this.Clicked_CancelWorkWithEntity;
            this._FiltredSparePartCategorys = this._FilterSparePartCategorys();
            this._TextBox_Search.TextChanged +=_TextBox_Search_TextChanged;  
        }
        public Details(bool ForSelectSparePart,TSSEntities entities):this()
        {
            InitializeComponent();
            this._Button_AddSelectedDetail.Visibility = Visibility.Visible;
            this._TSS = entities;
        }
        //

        private void _TextBox_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            this._FiltredSpareParts = this._FilterSpareParts(); 
        }

        private void Clicked_AddSparePartCategory(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Insert(this._sparePartCategoryForWork, this._TSS);
                this._FiltredSparePartCategorys = this._FilterSparePartCategorys();
                this._ModeEnabled_workWithSparePartCategories = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка добавления");
            }
        }

        private void Clicked_SaveSparePartCategoryChanges(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Update(this._sparePartCategoryForWork, this._TSS);
                this._ModeEnabled_workWithSparePartCategories = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка изменения");
            }
        }

        private void Clicked_CancelWorkWithEntity(object sender, RoutedEventArgs e)
        {
            this._ModeEnabled_workWithSparePartCategories = false;
            this._ModeEnabled_workWithSpareParts = false;
            Repository.UndoingChangesDbContextLevel(this._TSS);
        }

        private void Clicked_Search(object sender, RoutedEventArgs e)
        {
            this._FiltredSparePartCategorys = this._FilterSparePartCategorys();
        }

        private void Clicked_ChangeCategory(object sender, RoutedEventArgs e)
        {
            if (this._SelectedSparePartCategory != null)
            {
                this._ModeEnabled_workWithSparePartCategories = true;
                this._Mode_workWithCategory = WorkWithEntityMode.Change;

                this._sparePartCategoryForWork = _SelectedSparePartCategory;
                this._Flyout_ForSparePartCategories.DataContext = this._sparePartCategoryForWork;
            }
            else
            {
                MessageBox.Show("Выберите категорию");
            }
        }

        private void Clicked_DeleteCategory(object sender, RoutedEventArgs e)
        {
            List<SparePartCategory> SparePartCategorysForDelete = this._SelectedSparePartCategorys;
            if (SparePartCategorysForDelete.Count == 0)
            {
                MessageBox.Show("Выберите категорию");
            }
            else
            {
                try
                {
                    foreach (SparePartCategory sparePartCategory in SparePartCategorysForDelete)
                    {
                        Repository.Delete(sparePartCategory, this._TSS);
                        this._FiltredSparePartCategorys.Remove(sparePartCategory);
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
            this._ModeEnabled_workWithSparePartCategories = true;
            this._Mode_workWithCategory = WorkWithEntityMode.Adding;
            this._sparePartCategoryForWork = new SparePartCategory();
            this._Flyout_ForSparePartCategories.DataContext = this._sparePartCategoryForWork;
        }

        private ObservableCollection<SparePartCategory> _FilterSparePartCategorys()
        {
            /*string Surname = this._Field_Search_Surname.Text.Trim();
            string Name = this._Field_Search_Name.Text.Trim();
            string PatronymicName = this._Field_Search_PatronymicName.Text.Trim();
            string ID = this._Field_Search_ID.Text.Trim();
            IQueryable<SparePartCategory> request;
            request = this._TSS.SparePartCategory.Where(
                sparePartCategory => sparePartCategory.ID.ToString().Contains(ID)
            );
            return new ObservableCollection<SparePartCategory>(request.ToList());*/
            return new ObservableCollection<SparePartCategory>(this._TSS.SparePartCategory.ToList());
        }

        //-- --//
        private void SelectionChanged_SparePartCategory(object sender, SelectionChangedEventArgs e)
        {
            if (this._SelectedSparePartCategory != null)
            {
                this._FiltredSpareParts = new ObservableCollection<SparePart>(this._SelectedSparePartCategory.SparePart.ToList());
            }
        }

        private void Clicked_NewSparePart(object sender, RoutedEventArgs e)
        {
            this._ModeEnabled_workWithSpareParts = true;
            this._Mode_workWithSparePart = WorkWithEntityMode.Adding;
            this._SparePartForWork = new SparePart();

            if (this._SelectedSparePartCategory != null)
            {
                this._SparePartForWork = new SparePart()
                {
                    SparePartCategory = this._SelectedSparePartCategory
                };
            }
            this._Flyout_ForSparePart.DataContext = this._SparePartForWork;
            this._ComboBox_SparePartCategory.ItemsSource = this._TSS.SparePartCategory.ToList();
            this._ComboBox_Unit.ItemsSource = this._TSS.Unit.ToList();
        }

        private void Clicked_DeleteSparePart(object sender, RoutedEventArgs e)
        {
            List<SparePart> SparePartsForDelete = this._SelectedSpareParts;
            if (SparePartsForDelete.Count == 0)
            {
                MessageBox.Show("Выберите деталь");
            }
            else
            {
                try
                {
                    foreach (SparePart service in SparePartsForDelete)
                    {
                        Repository.Delete(service, this._TSS);
                        this._FiltredSpareParts.Remove(service);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка удаления");
                }
            }
        }

        private void Clicked_ChangeSparePart(object sender, RoutedEventArgs e)
        {
            if (this._SelectedSparePart != null)
            {
                if (this._SelectedSparePartCategory != null)
                {
                    this._SparePartForWork = new SparePart()
                    {
                        SparePartCategory = this._SelectedSparePartCategory
                    };
                }
                this._ComboBox_SparePartCategory.ItemsSource = this._TSS.SparePartCategory.ToList();
                this._ComboBox_Unit.ItemsSource = this._TSS.Unit.ToList();
                this._ModeEnabled_workWithSpareParts = true;
                this._Mode_workWithSparePart = WorkWithEntityMode.Change;
                
                this._SparePartForWork = _SelectedSparePart;
            }
            else
            {
                MessageBox.Show("Выберите деталь");
            }
        }

        private void Clicked_AddSparePart(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Insert(this._sparePartForWork, this._TSS);
                this._FiltredSpareParts = this._FilterSpareParts();
                this._ModeEnabled_workWithSpareParts = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка добавления");
            }
        }

        private void _Button_SaveSparePartChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Update(this._sparePartForWork, this._TSS);
                this._ModeEnabled_workWithSpareParts = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка изменения");
            }
        }

        private ObservableCollection<SparePart> _FilterSpareParts()
        {
            List<SparePart> result = new List<SparePart>();
            if (this._SelectedSparePartCategory != null)
            {
                result = this._TSS.SparePart.Where(
                    sparePart => sparePart.Title.Contains(this._TextBox_Search.Text)
                ).Where(
                    sparePart => sparePart.SparePartsCategoryID == this._SelectedSparePartCategory.ID
                ).ToList();
            }
            else
            {
                MessageBox.Show("Выберите категорию для поиска");
            }
            return new ObservableCollection<SparePart>(result);
        }

        private void Clicked_OpenMenu(object sender, RoutedEventArgs e)
        {
            this._Flyout_ForMenu.IsOpen = true;
        }

        //-- --//
        private void Clicked_AddSelectedSparePart(object sender, RoutedEventArgs e)
        {
            if (this._SelectedSparePart != null)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Выберите деталь");
            }
        }
    }
}
