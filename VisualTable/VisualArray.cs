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
        public static DataTable res;
        private static Stack<DataTable> reservedtable = new Stack<DataTable>();
        private static Stack<DataTable> cancelledchanges = new Stack<DataTable>();
        public static Stack<DataTable> ReservedTable { get => reservedtable; }
        public static Stack<DataTable> CancelledChanges { get => cancelledchanges; }
        public static DataTable AddNewRow()
        {
            DataRow row;
            row = res.NewRow();
            for (int i = 0; i < res.Columns.Count; i++)
            {
                row[i] = 0;
            }
            res.Rows.Add(row);
            return res;
        }
        public static DataTable AddNewRow<T>(T[] mas)
        {
            res.Rows.Add(mas);
            return res;
        }
        public static DataTable DeleteRow(int index)
        {
            DataRow row = res.Rows[index];
            res.Rows.Remove(row);
            return res;
        }
        public static DataTable AddNewColumn(int [,] dmas)
        {            
            dmas = SyncData();
            dmas = AddNewColumnIntoMas(dmas);                       
            res = ToDataTable(dmas);
            return res;
        }
        public static DataTable DeleteColumn(int[,] dmas, int index)
        {            
            dmas = DeleteColumnIntoMas(dmas, index);
            res = ToDataTable(dmas);
            return res;
        }
        //Метод для двухмерного массива
        public static DataTable ToDataTable<T>(T[,] matrix)
        {            
            res = new DataTable();
             for (int i = 0; i < matrix.GetLength(1); i++)
            {
                res.Columns.Add("Column" + (i+1), typeof(T));
            }

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                DataRow row = res.NewRow();
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    row[j] = matrix[i, j];
                }
                res.Rows.Add(row);
            }
            ReserveTable();
            return res;
        }
        public static int[,] SyncData()
        {            
            int[,] dmas = new int[res.Rows.Count, res.Columns.Count];
            for (int i = 0; i < dmas.GetLength(0); i++)
            {
                DataRow row = res.Rows[i];
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
            res.Clear();            
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
        public static void ReserveTable()
        {
            reservedtable.Push(res);
            cancelledchanges.Clear();
        }
        public static DataTable CancelChanges()
        {
            if (reservedtable.Count != 0)
            {
                cancelledchanges.Push(reservedtable.Peek());
                res = reservedtable.Pop();
            }
            return res;
        }
        public static DataTable CancelReturn()
        {
            if (cancelledchanges.Count != 0)
            {
                reservedtable.Push(cancelledchanges.Peek());
                res = cancelledchanges.Pop();
            }
            return res;
        }
    }
}
