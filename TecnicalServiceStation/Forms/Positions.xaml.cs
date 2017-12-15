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
    /// Логика взаимодействия для Positions.xaml
    /// </summary>
    public partial class Positions : MetroWindow
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
                    this._Button_AddPosition.Visibility = Visibility.Visible;
                    this._Button_SavePosition.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._Button_AddPosition.Visibility = Visibility.Collapsed;
                    this._Button_SavePosition.Visibility = Visibility.Visible;
                }
            }
        }
        //
        private TSSEntities _TSS = new TSSEntities();
        private ObservableCollection<Positon> _filtredPositions;
        private ObservableCollection<Positon> _FiltredPositions
        {
            get
            {
                return this._filtredPositions;
            }
            set
            {
                this._filtredPositions = value;
                this._List_Positions.ItemsSource = this._filtredPositions;
                this._List_Positions.Items.Refresh();
            }
        }
        private Positon _positionForWork;
        private Positon _PositionForWork
        {
            get
            {
                return this._positionForWork;
            }
            set
            {
                this._positionForWork = value;
                this._Flyout_ForEntity.DataContext = this._positionForWork;
            }
        }
        private List<Positon> _SelectedPositions
        {
            get
            {
                List<Positon> positions = new List<Positon>();
                if (this._List_Positions.SelectedItem != null)
                {
                    foreach (Positon position in this._List_Positions.SelectedItems)
                    {
                        positions.Add(position);
                    }
                }
                return positions;
            }
        }
        private Positon _SelectedPosition
        {
            get
            {
                return (Positon)this._List_Positions.SelectedItem;
            }
        }
        //
        public Positions()
        {
            InitializeComponent();
            this._ModeEnabled_workWithEntity = false;
            this._Flyout_ForEntity.ClosingFinished += this.Clicked_CancelWorkWithEntity;
            this._FiltredPositions = this._FilterPositions();
        }

        private void Clicked_AddPosition(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Insert(this._positionForWork, this._TSS);
                this._FiltredPositions=this._FilterPositions();
                this._ModeEnabled_workWithEntity = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка добавления");
            }
        }

        private void Clicked_SavePositionChanges(object sender, RoutedEventArgs e)
        {
            
            try
            {
                Repository.Update(this._positionForWork, this._TSS);
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
            this._positionForWork = new Positon();
            this._Flyout_ForEntity.DataContext = this._positionForWork;
        }

        private void Clicked_Delete(object sender, RoutedEventArgs e)
        {
            List<Positon> positionsForDelete = this._SelectedPositions;
            if (positionsForDelete.Count == 0)
            {
                MessageBox.Show("Выберите клиента");
            }
            else
            {
                try
                {
                    foreach (Positon position in positionsForDelete)
                    {
                        Repository.Delete(position, this._TSS);
                        this._FiltredPositions.Remove(position);
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
            if (this._SelectedPosition != null)
            {
                this._ModeEnabled_workWithEntity = true;
                this._Mode_workWithEntity = WorkWithEntityMode.Change;

                this._positionForWork = _SelectedPosition;
                this._Flyout_ForEntity.DataContext = this._positionForWork;
            }
            else
            {
                MessageBox.Show("Выберите клиента");
            }
        }

        private void Clicked_Search(object sender, RoutedEventArgs e)
        {
            this._FiltredPositions = this._FilterPositions();
        }

        private ObservableCollection<Positon> _FilterPositions()
        {
            string ID = this._Field_Search_ID.Text;
            string PositionTitle = this._Field_Search_Title.Text;
            IQueryable<Positon> request;
            request = this._TSS.Positon.Where(
                position => position.ID.ToString().Contains(ID)
            ).Where(
                position => position.Title.Contains(PositionTitle)
            );
            return new ObservableCollection<Positon>(request.ToList());

        }
    }
}
