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
    /// Логика взаимодействия для Workers.xaml
    /// </summary>
    public partial class Workers : MetroWindow
    {
        private bool _ModeEnabled_workWithEntity
        {
            get
            {
                return this._Flyout_ForEntity.IsOpen;
            }
            set
            {
                this._Select_Positions.ItemsSource = this._TSS.Positon.ToList();
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
                    this._Button_AddWorker.Visibility = Visibility.Visible;
                    this._Button_SaveWorker.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._Button_AddWorker.Visibility = Visibility.Collapsed;
                    this._Button_SaveWorker.Visibility = Visibility.Visible;
                }
            }
        }
        //
        private TSSEntities _TSS = new TSSEntities();
        private ObservableCollection<Worker> _filtredWorkers;
        private ObservableCollection<Worker> _FiltredWorkers
        {
            get
            {
                return this._filtredWorkers;
            }
            set
            {
                this._filtredWorkers = value;
                this._List_Workers.ItemsSource = this._filtredWorkers;
                this._List_Workers.Items.Refresh();
            }
        }
        private Worker _workerForWork;
        private Worker _WorkerForWork
        {
            get
            {
                return this._workerForWork;
            }
            set
            {
                this._workerForWork = value;
                this._Flyout_ForEntity.DataContext = this._workerForWork;
            }
        }
        private List<Worker> _SelectedWorkers
        {
            get
            {
                List<Worker> Workers = new List<Worker>();
                if (this._List_Workers.SelectedItem != null)
                {
                    foreach (Worker worker in this._List_Workers.SelectedItems)
                    {
                        Workers.Add(worker);
                    }
                }
                return Workers;
            }
        }
        private Worker _SelectedWorker
        {
            get
            {
                return (Worker)this._List_Workers.SelectedItem;
            }
        }
        //
        public Workers()
        {
            InitializeComponent();
            this._ModeEnabled_workWithEntity = false;
            this._Flyout_ForEntity.ClosingFinished += this.Clicked_CancelWorkWithEntity;
            this._FiltredWorkers = this._FilterWorkers();
        }

        private void Clicked_AddWorker(object sender, RoutedEventArgs e)
        {
            try
            {
                Repository.Insert(this._workerForWork, this._TSS);
                this._FiltredWorkers = this._FilterWorkers();
                this._ModeEnabled_workWithEntity = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка добавления");
            }
        }

        private void Clicked_SaveWorkerChanges(object sender, RoutedEventArgs e)
        {

            try
            {
                Repository.Update(this._workerForWork, this._TSS);
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
            this._workerForWork = new Worker();
            this._Flyout_ForEntity.DataContext = this._workerForWork;
        }

        private void Clicked_Delete(object sender, RoutedEventArgs e)
        {
            List<Worker> WorkersForDelete = this._SelectedWorkers;
            if (WorkersForDelete.Count == 0)
            {
                MessageBox.Show("Выберите клиента");
            }
            else
            {
                try
                {
                    foreach (Worker worker in WorkersForDelete)
                    {
                        Repository.Delete(worker, this._TSS);
                        this._FiltredWorkers.Remove(worker);
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
            if (this._SelectedWorker != null)
            {
                this._ModeEnabled_workWithEntity = true;
                this._Mode_workWithEntity = WorkWithEntityMode.Change;

                this._workerForWork = _SelectedWorker;
                this._Flyout_ForEntity.DataContext = this._workerForWork;
            }
            else
            {
                MessageBox.Show("Выберите клиента");
            }
        }

        private void Clicked_Search(object sender, RoutedEventArgs e)
        {
            this._FiltredWorkers = this._FilterWorkers();
        }

        private ObservableCollection<Worker> _FilterWorkers()
        {
            string Surname = this._Field_Search_Surname.Text.Trim();
            string Name = this._Field_Search_Name.Text.Trim();
            string PatronymicName = this._Field_Search_PatronymicName.Text.Trim();
            string ID = this._Field_Search_ID.Text.Trim();
            IQueryable<Worker> request;
            request = this._TSS.Worker.Where(
                worker => worker.Surname.Contains(Surname)
            ).Where(
                worker => worker.Name.Contains(Name)
            ).Where(
                worker => worker.PatronymicName.Contains(PatronymicName)
            ).Where(
                worker => worker.ID.ToString().Contains(ID)
            );
            return new ObservableCollection<Worker>(request.ToList());
        }
    }
}
