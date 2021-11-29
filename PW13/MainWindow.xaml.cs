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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using LibMas;
using VisualTable;
using FindCountMoreAvgColumnLibrary;
using System.Windows.Threading;
//Практическая работа №13. Лопаткин Сергей ИСП-31
//Задание №8. Дана матрица размера M * N. В каждом ее столбце найти количество элементов, 
//больших среднего арифметического всех элементов этого столбца
namespace PW13
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer cell = new DispatcherTimer();
            cell.Tick += Cell_Tick;
            cell.Interval = new TimeSpan(0,0,0,0,200);
            cell.IsEnabled = true;            
        }        
        private void Cell_Tick(object sender, EventArgs e)
        {
            string _defaultcurrentcell = "Ячейка не выбрана";
            if (WorkMas._dmas != null)
            {
                string _linelength = "";
                if (VisualTable.SelectedItem == null) CurrentCell.Text = _defaultcurrentcell;
                else CurrentCell.Text = (VisualTable.CurrentCell.Column.DisplayIndex + 1).ToString() + " столбец / " + (VisualTable.SelectedIndex + 1) + " строка";

                if (WorkMas._dmas.GetLength(1) <= 4)
                    _linelength = WorkMas._dmas.GetLength(1).ToString() + " столбца / ";
                else
                    TableLength.Text = WorkMas._dmas.GetLength(1).ToString() + " столбцов / ";
                if (WorkMas._dmas.GetLength(0) <= 4)
                    _linelength += WorkMas._dmas.GetLength(0).ToString() + " строки";
                else
                    _linelength += WorkMas._dmas.GetLength(0).ToString() + " строк";
                TableLength.Text = _linelength;
             }
            else
            {
                CurrentCell.Text = _defaultcurrentcell;
                TableLength.Text = "Таблица не создана";
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {            
            OpenFileDialog openfile = new OpenFileDialog
            {
                Title = "Открытие таблицы",
                Filter = "Все файлы (*.*) | *.* | Текстовые файлы | *.txt",
                FilterIndex = 2,
                DefaultExt = "*.txt",                
            };
            
            if (openfile.ShowDialog() == true) 
            {                                
                WorkMas.Open_File(openfile.FileName); //Обращение к функции с параметром (название текстового файла, в котором хранятся данные)
                VisualTable.ItemsSource = VisualArray.ToDataTable(WorkMas._dmas).DefaultView; //Отображение данных, считанных с файла
                ClearResults();
                VisualArray.ClearUndoAndCancelUndo();
                DynamicActionsOnOrOff(e);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {            
            SaveFileDialog savefile = new SaveFileDialog
            {
                Title = "Сохранение таблицы",
                Filter = "Все файлы (*.*) | *.* | Текстовые файлы | *.txt",
                FilterIndex = 2,
                DefaultExt = "*.txt",
            };

            if (savefile.ShowDialog() == true)
            {                
                WorkMas._twomas = true;                
                WorkMas.Save_File(savefile.FileName); //Обращение к функции с параметром (аналогично предыдущему) 
                VisualArray.ClearUndoAndCancelUndo();
            }
        }

        private void ClearTable_Click(object sender, RoutedEventArgs e)
        {
            VisualTable.ItemsSource = WorkMas.ClearTable(); //Обращение к функции "очистки" массива и возвращение null для DataGrid(Очистка таблицы)
            VisualArray.ClearUndoAndCancelUndo();
            DynamicActionsOnOrOff(e);
            ClearResults();
        }

        private void CreateMas_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            bool prv_columns = int.TryParse(CountColumns.Text, out int columns);
            bool prv_rows = int.TryParse(CountRows.Text, out int rows);
            if (prv_columns == true && prv_rows == true)
            {
                WorkMas.CreateMas(in rows, in columns);
                VisualArray.ClearUndoAndCancelUndo();
                VisualTable.ItemsSource = VisualArray.ToDataTable(WorkMas._dmas).DefaultView;
                DynamicActionsOnOrOff(e);                
            }           
        }

        private void Exit_Click(object sender, RoutedEventArgs e) //Закрытие программы
        {
            Close();
        }

        private void AboutProgram_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Практическая работа №13. Лопаткин Сергей Михайлович. " +
                "Задание №8. Дана матрица размера M * N. В каждом ее столбце найти количество элементов, " +
                "больших среднего арифметического всех элементов этого столбца",
                "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void Fill_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            bool prv_range = int.TryParse(Range.Text, out int range);
            if (prv_range == true && WorkMas._dmas != null) //2-ое условие - проверка на заполнение без скелета
            {                
                WorkMas.FillDMas(in range);//Обращение с передачей информации об диапазоне
                VisualTable.ItemsSource = VisualArray.ToDataTable(WorkMas._dmas).DefaultView; //Отображение таблицы с заполненными значениями
            }
            else MessageBox.Show("Введен некорректно диапазон значений",
                "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("1) В программе нельзя вводить более трехзначных чисел для диапазона и двухзначных для столбцов и строк.\n" +
                "2)Заполнение происходит от 0 до указанного вами значения\n" +
                "3)Для включения кнопок \"Выполнить\" и \"Заполнить\" необходимо создать таблицу.", "Справка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void VisualTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {                                 
            object cell = VisualTable.SelectedItem;
            int iRow = e.Row.GetIndex();
            int iColumn = e.Column.DisplayIndex;
            bool tryedit = int.TryParse(((TextBox)e.EditingElement).Text, out int value);
            if (tryedit)
            {
                WorkMas._dmas[iRow, iColumn] = value;
                VisualArray.ReserveTable(WorkMas._dmas);
            }
            else 
            {
                VisualTable.SelectedItem = cell;
            }
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                VisualTable.SelectedItem = cell;
            }
        }

        private void CountColumns_GotFocus(object sender, RoutedEventArgs e)
        {
            CreateMas.IsDefault = true;
            Fill.IsDefault = false;
        }

        private void Range_GotFocus(object sender, RoutedEventArgs e)
        {
            CreateMas.IsDefault = false;
            Fill.IsDefault = true;
        }

        private void MainWin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.O) Open_Click(sender, e);
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.S) Save_Click(sender, e);
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt && e.Key == Key.F4) Exit_Click(sender, e);
            if (e.Key == Key.F1) Support_Click(sender, e);
            if (e.Key == Key.F12) AboutProgram_Click(sender, e);
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Z) 
            {
                VisualTable.ItemsSource = VisualArray.CancelChanges().DefaultView;
                WorkMas._dmas = VisualArray.SyncData();
            }
            
            if (((e.KeyboardDevice.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == 
                (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.Z) ^
                (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Y))              
            {
                VisualTable.ItemsSource = VisualArray.CancelUndo().DefaultView;
                WorkMas._dmas = VisualArray.SyncData();
            }
        }

        private void VisualTable_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {            
            e.Handled = "-1234567890".IndexOf(e.Text) < 0;
        }
        private void AddColumn_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();            
            VisualTable.ItemsSource = VisualArray.AddNewColumn(ref WorkMas._dmas).DefaultView;            
        }
        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();            
            VisualTable.ItemsSource = VisualArray.AddNewRow(ref WorkMas._dmas).DefaultView;            
        }
        private void DeleteColumn_Click(object sender, RoutedEventArgs e)
        {
            if (VisualTable.CurrentCell.Column.DisplayIndex != -1)
            {
                ClearResults();                
                VisualTable.ItemsSource = VisualArray.DeleteColumn(ref WorkMas._dmas, VisualTable.CurrentCell.Column.DisplayIndex).DefaultView;
            }
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (VisualTable.SelectedIndex != -1)
            {
                ClearResults();                
                VisualTable.ItemsSource = VisualArray.DeleteRow(ref WorkMas._dmas, VisualTable.SelectedIndex).DefaultView;
            }
        }
        private void DynamicActionsOnOrOff(RoutedEventArgs e)
        {
            if (e.Source == CreateMas || e.Source == CreateMasMenu 
                || e.Source == OpenMenu)
            {
                DynamicActions.IsEnabled = true;
                AddColumnContextMenu.IsEnabled = true;
                AddRowContextMenu.IsEnabled = true;
                DeleteColumnContextMenu.IsEnabled = true;
                DeleteRowContextMenu.IsEnabled = true;                
                Find.IsEnabled = true;
                FindMenu.IsEnabled = true;
                Fill.IsEnabled = true;
                FillMenu.IsEnabled = true;
                FillToolBar.IsEnabled = true;
                ClearTable.IsEnabled = true;
                ClearTableMenu.IsEnabled = true;
                ClearTableToolBar.IsEnabled = true;
            }
            else
            {
                DynamicActions.IsEnabled = false;
                AddColumnContextMenu.IsEnabled = false;
                AddRowContextMenu.IsEnabled = false;
                DeleteColumnContextMenu.IsEnabled = false;
                DeleteRowContextMenu.IsEnabled = false;                
                Find.IsEnabled = false;
                FindMenu.IsEnabled = false;
                Fill.IsEnabled = false;
                FillMenu.IsEnabled = false;
                FillToolBar.IsEnabled = false;
                ClearTable.IsEnabled = false;
                ClearTableMenu.IsEnabled = false;
                ClearTableToolBar.IsEnabled = false;
            }
        }
        private void ClearResults()
        {
            AvgOfColumns.ItemsSource = null;
            CountMoreAvgOfColumns.ItemsSource = null;
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {            
            VisualArray.ClearUndoAndCancelUndo();
            int [][] result = FindCountMoreAvgColumnClass.FindCountMoreAvgColumn(WorkMas._dmas);
            AvgOfColumns.ItemsSource = VisualArray.ToDataTable(result[0]).DefaultView;
            CountMoreAvgOfColumns.ItemsSource = VisualArray.ToDataTable(result[1]).DefaultView;
        }
    }
}
