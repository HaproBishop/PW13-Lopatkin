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
            DispatcherTimer cell = new DispatcherTimer();//Таймер для StatusBar
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
                if (VisualTable.SelectedIndex == -1 || VisualTable.CurrentCell.Column == null) CurrentCell.Text = _defaultcurrentcell;
                else CurrentCell.Text = (VisualTable.CurrentCell.Column.DisplayIndex + 1).ToString() + " столбец / " + (VisualTable.SelectedIndex + 1) + " строка";

                if (WorkMas._dmas.GetLength(1) <= 4)
                    _linelength = WorkMas._dmas.GetLength(1).ToString() + " столбца / ";
                else
                    _linelength = WorkMas._dmas.GetLength(1).ToString() + " столбцов / ";
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
        /// <summary>
        /// Шаблонное открытие таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            }
        }
        /// <summary>
        /// Шаблонное сохранение таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                if(e.Source == SaveMenu || e.Source == SaveToolBar) WorkMas._twomas = true;   
                else WorkMas._twomas = false;
                WorkMas.Save_File(savefile.FileName); //Обращение к функции с параметром (аналогично предыдущему) 
                VisualArray.ClearUndoAndCancelUndo();
            }
        }
        /// <summary>
        /// Очистка таблицы;
        /// Также производится очистка undo and cancelundo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearTable_Click(object sender, RoutedEventArgs e)
        {
            VisualTable.ItemsSource = WorkMas.ClearTable(); //Обращение к функции "очистки" массива и возвращение null для DataGrid(Очистка таблицы)
            VisualArray.ClearUndoAndCancelUndo();//Обращение к методу очистки undo and cancelundo            
            ClearResults();
        }
        /// <summary>
        /// Создание пустой таблицы, заполненной нулями
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateMas_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            bool prv_columns = int.TryParse(CountColumns.Text, out int columns);
            bool prv_rows = int.TryParse(CountRows.Text, out int rows);
            if (prv_columns == true && prv_rows == true && columns > 0 && rows > 0)
            {
                WorkMas.CreateMas(in rows, in columns);
                VisualArray.ClearUndoAndCancelUndo();
                VisualTable.ItemsSource = VisualArray.ToDataTable(WorkMas._dmas).DefaultView;                
            }           
        }
        /// <summary>
        /// Выход (Закрытие программы)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e) //Закрытие программы
        {
            Close();
        }
        /// <summary>
        /// Сообщение пользователю о работе программы, а также о разработчике
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutProgram_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Практическая работа №13. Лопаткин Сергей Михайлович. " +
                "Задание №8. Дана матрица размера M * N. В каждом ее столбце найти количество элементов, " +
                "больших среднего арифметического всех элементов этого столбца",
                "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        /// <summary>
        /// Заполнение массива
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fill_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            bool prv_range = int.TryParse(Range.Text, out int range);
            if (prv_range == true && WorkMas._dmas != null && range > 0) //2-ое условие - проверка на заполнение без скелета(В нашем случае - проверка на скелет не нужна)
            {                
                WorkMas.FillDMas(in range);//Обращение с передачей информации об диапазоне
                VisualTable.ItemsSource = VisualArray.ToDataTable(WorkMas._dmas).DefaultView; //Отображение таблицы с заполненными значениями
            }
            else MessageBox.Show("Введен некорректно диапазон значений, необходимо больше 0",
                "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        /// <summary>
        /// Справка пользователю об особенностях программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Support_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("1) В программе нельзя вводить более трехзначных чисел для диапазона и двухзначных для столбцов и строк.\n" +
                "2)Заполнение происходит от 0 до указанного вами значения\n" +
                "3)Для включения кнопок \"Выполнить\" и \"Заполнить\" необходимо создать таблицу.", "Справка", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        /// <summary>
        /// Событие окончания изменения значения ячейки 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisualTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {                                 
            object cell = VisualTable.SelectedItem;//Зарезервированное значение ячейки до изменения
            int iRow = e.Row.GetIndex();
            int iColumn = e.Column.DisplayIndex;
            bool tryedit = int.TryParse(((TextBox)e.EditingElement).Text, out int value);
            if (tryedit)
            {
                WorkMas._dmas[iRow, iColumn] = value;//Занесение значения в двумерный массив(указанная ячейка)
                VisualArray.ReserveTable(WorkMas._dmas);//Резервирование текущей таблицы при успешном изменении значения
                ClearResults();
            }
            else 
            {
                VisualTable.SelectedItem = cell;//Возвращение значения ячейки при неверном вводе
            }
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                VisualTable.SelectedItem = cell;//Возвращение значения перед изменениями, если была произведена отмена
            }
        }
        //Переключение дефолта относительно полученного фокуса
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
        /// <summary>
        /// Событие нажатия клавиш для окна (В данном случае используется для горячих клавиш)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                WorkMas._dmas = VisualArray.SyncData();//Обязательная синхронизация
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
            MessageBoxResult saveresult = MessageBox.Show("Вы хотите сохранить результаты среднего арифметического столбцов?", "Сохранение результатов среднего арифметического",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (saveresult == MessageBoxResult.Yes)
            {
                WorkMas._mas = result[0];
                Save_Click(sender, e);
            }
            saveresult = MessageBox.Show("Вы хотите сохранить результаты количества значений ячеек, больших среднего" +
                " арифметического?", "Сохранение результатов количества",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (saveresult == MessageBoxResult.Yes)
            {
                WorkMas._mas = result[1];
                Save_Click(sender, e);
            }
        }
    }
}
