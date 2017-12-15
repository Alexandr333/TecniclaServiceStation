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
    /// Логика взаимодействия для Units.xaml
    /// </summary>
    public partial class Units : MetroWindow
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
                    this._Button_AddUnit.Visibility = Visibility.Visible;
                    this._Button_SaveUnit.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._Button_AddUnit.Visibility = Visibility.Collapsed;
                    this._Button_SaveUnit.Visibility = Visibility.Visible;
                }
            }
        }
        //
        private TSSEntities _TSS = new TSSEntities();
        private ObservableCollection<Unit> _filtredUnits;
        private ObservableCollection<Unit> _FiltredUnits
        {
            get
            {
                return this._filtredUnits;
            }
            set
            {
                this._filtredUnits = value;
                this._List_Units.ItemsSource = this._filtredUnits;
                this._List_Units.Items.Refresh();
            }
        }
        private Unit _unitForWork;
        private Unit _UnitForWork
        {
            get
            {
                return this._unitForWork;
            }
            set
            {
                this._unitForWork = value;
                this._Flyout_ForEntity.DataContext = this._unitForWork;
            }
        }
        private List<Unit> _SelectedUnits
        {
            get
            {
                List<Unit> units = new List<Unit>();
                if (this._List_Units.SelectedItem != null)
                {
                    foreach (Unit unit in this._List_Units.SelectedItems)
                    {
                        units.Add(unit);
                    }
                }
                return units;
            }
        }
        private Unit _SelectedUnit
        {
            get
            {
                return (Unit)this._List_Units.SelectedItem;
            }
        }
        //
        public Units()
        {
            InitializeComponent();
            this._ModeEnabled_workWithEntity = false;
            this._Flyout_ForEntity.ClosingFinished += this.Clicked_CancelWorkWithEntity;
            this._FiltredUnits = this._FilterUnits();
        }

        private void Clicked_AddUnit(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Insert(this._UnitForWork, this._TSS);
                this._FiltredUnits=this._FilterUnits();
                this._ModeEnabled_workWithEntity = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка добавления");
            }
        }

        private void Clicked_SaveUnitChanges(object sender, RoutedEventArgs e)
        {
            
            try
            {
                Repository.Update(this._UnitForWork, this._TSS);
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
            this._unitForWork = new Unit();
            this._Flyout_ForEntity.DataContext = this._unitForWork;
        }

        private void Clicked_Delete(object sender, RoutedEventArgs e)
        {
            List<Unit> unitsForDelete = this._SelectedUnits;
            if (unitsForDelete.Count == 0)
            {
                MessageBox.Show("Выберите клиента");
            }
            else
            {
                try
                {
                    foreach (Unit unit in unitsForDelete)
                    {
                        Repository.Delete(unit, this._TSS);
                        this._FiltredUnits.Remove(unit);
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
            if (this._SelectedUnit != null)
            {
                this._ModeEnabled_workWithEntity = true;
                this._Mode_workWithEntity = WorkWithEntityMode.Change;

                this._UnitForWork = _SelectedUnit;
            }
            else
            {
                MessageBox.Show("Выберите клиента");
            }
        }

        private ObservableCollection<Unit> _FilterUnits()
        {
            string UnitTitle = this._Field_Search_Title.Text;
            IQueryable<Unit> request;
            request = this._TSS.Unit.Where(
                unit => unit.Title.Contains(UnitTitle)
            );
            return new ObservableCollection<Unit>(request.ToList());

        }

        private void Changed_Search(object sender, TextChangedEventArgs e)
        {
            this._FiltredUnits = this._FilterUnits();
        }
    }
}
