using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace VisualTable
{
    //Класс для связывания массива с элементом DataGrid
    //для визуализации данных 
    public static class VisualArray
    {
        public static DataTable _res;
        private static Stack<int[,]> _reservedtable = new Stack<int[,]>();
        private static Stack<int[,]> _cancelledchanges = new Stack<int[,]>();
        private static bool _needreserve = true;
        public static bool NeedReserve { get => _needreserve; set => _needreserve = value; }
        public static Stack<int[,]> ReservedTable { get => _reservedtable; }
        public static Stack<int[,]> CancelledChanges { get => _cancelledchanges; }
        public static DataTable AddNewRow()
        {
            DataRow row;
            row = _res.NewRow();
            for (int i = 0; i < _res.Columns.Count; i++)
            {
                row[i] = 0;
            }
            _res.Rows.Add(row);            
            return _res;
        }
        public static DataTable AddNewRow(int[] mas)
        {
            _res.Rows.Add(mas);            
            return _res;
        }
        public static DataTable DeleteRow(int index)
        {
            DataRow row = _res.Rows[index];
            _res.Rows.Remove(row);            
            return _res;
        }
        public static DataTable AddNewColumn()
        {            
            int [,]dmas = SyncData();
            dmas = AddNewColumnIntoMas(dmas);
            _needreserve = false;
            _res = ToDataTable(dmas);            
            return _res;
        }
        public static DataTable DeleteColumn(int index)
        {
            int[,] dmas = SyncData();
            dmas = DeleteColumnIntoMas(dmas, index);
            _needreserve = false;
            _res = ToDataTable(dmas);
            return _res;
        }
        //Метод для двухмерного массива
        public static DataTable ToDataTable(int[,] matrix)
        {            
            _res = new DataTable();
             for (int i = 0; i < matrix.GetLength(1); i++)
            {
                _res.Columns.Add("Column" + (i+1), typeof(string));
            }

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                DataRow row = _res.NewRow();
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    row[j] = matrix[i, j];
                }
                _res.Rows.Add(row);
            }
            if(_needreserve)ReserveTable(matrix);
            _needreserve = true;
            return _res;
        }
        public static int[,] SyncData()
        {            
            int[,] dmas = new int[_res.Rows.Count, _res.Columns.Count];
            for (int i = 0; i < dmas.GetLength(0); i++)
            {
                DataRow row = _res.Rows[i];
                for (int j = 0; j < dmas.GetLength(1); j++)
                {
                    bool prove = int.TryParse(row[j].ToString(), out dmas[i, j]);
                    if (!prove) dmas[i, j] = 0;
                }
            }            
            return dmas;
        }
        public static int[,] AddNewColumnIntoMas(int [,] dmas)
        {
            int[,] newdmas = new int[dmas.GetLength(0), dmas.GetLength(1) + 1];
            for (int i = 0; i < dmas.GetLength(0); i++)
            {
                for (int j = 0; j < dmas.GetLength(1); j++)
                {
                    newdmas[i, j] = dmas[i, j];
                }
            }
            return newdmas;
        }
        public static int[,] AddNewRowIntoMas(int[,] dmas)
        {
            int[,] newdmas = new int[dmas.GetLength(0) + 1, dmas.GetLength(1)];
            for (int i = 0; i < dmas.GetLength(0); i++)
            {
                for (int j = 0; j < dmas.GetLength(1); j++)
                {
                    newdmas[i, j] = dmas[i, j];
                }
            }
            return newdmas;
        }
        public static void DataTableClear()
        {
            _res.Clear();            
        }
        public static int[,] DeleteColumnIntoMas(int[,] dmas, int index)
        {
            int[,] newdmas = new int[dmas.GetLength(0), dmas.GetLength(1) - 1];           
            for (int i = 0; i < dmas.GetLength(0); i++)
            {
                int nj = -1;
                for (int j = 0; j < dmas.GetLength(1); j++)
                {
                    nj++;
                    if (j == index)
                    {
                        nj--;
                        continue;
                    }
                    newdmas[i, nj] = dmas[i, j];
                }
            }
            return newdmas;
        }
        public static void ReserveTable(int[,] dmas)
        {
            _reservedtable.Push(dmas);            
        }
        public static DataTable CancelChanges()
        {
            if (_reservedtable.Count > 0)
            {
                ReserveTable(SyncData());
                _cancelledchanges.Push(_reservedtable.Pop());
                try
                {
                    _needreserve = false;
                    return ToDataTable(_reservedtable.Pop());
                }
                catch
                {
                    return _res;
                }
            }
            return _res;
        }
        public static DataTable CancelUndo()
        {
            if (_cancelledchanges.Count > 0)
            {
                ReserveTable(SyncData());
                _needreserve = false;
                return ToDataTable(_cancelledchanges.Pop());                           
            }
            return _res;
        }
    }
}
