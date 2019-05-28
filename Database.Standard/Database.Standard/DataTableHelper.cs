using System;
using System.Collections.Generic;
using System.Data;

namespace Interstates.Control.Database
{
    public class DataTableHelper
    {
        public DataTableHelper()
        {
        }

        /// <summary>
        /// In summary the code works as follows:
        /// 		Create new table
        /// 		Add Distinct columns and prepare sort expression
        /// 		Select all sorted rows
        /// 		Loop over rows and check against previous row
        /// 		Add only unique rows
        /// 		Return table
        /// </summary>
        public static DataTable Distinct(DataTable srcTable, DataColumn[] distinctColumns)
        {
            //Empty table
            DataTable distinctDT = new DataTable("Distinct");

            //Sort variable
            string sort = string.Empty;

            //Add DistinctCols & Build Sort expression
            for (int i = 0; i < distinctColumns.Length; i++)
            {
                distinctDT.Columns.Add(distinctColumns[i].ColumnName, distinctColumns[i].DataType);
                sort += distinctColumns[i].ColumnName + ",";
            }

            //Select all rows and strSort
            DataRow[] sortedDR = srcTable.Select(string.Empty, sort.Substring(0, sort.Length - 1));
            object[] currentRow = null;
            object[] prevRow = null;

            distinctDT.BeginLoadData();
            foreach (DataRow clsRow in sortedDR)
            {
                //Current Row
                currentRow = new object[distinctColumns.Length];
                for (int i = 0; i < distinctColumns.Length; i++)
                {
                    currentRow[i] = clsRow[distinctColumns[i].ColumnName];
                }

                //Match Current clsRow to previous clsRow
                if (!DataTableHelper.RowEqual(prevRow, currentRow))
                    distinctDT.LoadDataRow(currentRow, true);

                //Previous clsRow
                prevRow = new object[distinctColumns.Length];
                for (int i = 0; i < distinctColumns.Length; i++)
                {
                    prevRow[i] = clsRow[distinctColumns[i].ColumnName];
                }
            }
            distinctDT.EndLoadData();
            return distinctDT;
        }

        public static DataTable Distinct(DataTable srcTable, DataColumn distinctColumns)
        {
            return Distinct(srcTable, new DataColumn[] { distinctColumns });
        }

        public static DataTable Distinct(DataTable srcTable, string distinctColumns)
        {
            return Distinct(srcTable, srcTable.Columns[distinctColumns]);
        }

        public static DataTable Distinct(DataTable srcTable, params string[] distinctColumns)
        {
            DataColumn[] clsNewCols = new DataColumn[distinctColumns.Length];
            for (int i = 0; i < distinctColumns.Length; i++)
            {
                clsNewCols[i] = srcTable.Columns[distinctColumns[i]];
            }

            return Distinct(srcTable, clsNewCols);
        }

        public static DataTable Distinct(DataTable srcTable)
        {
            DataColumn[] clsNewCols = new DataColumn[srcTable.Columns.Count];
            for (int i = 0; i < srcTable.Columns.Count; i++)
            {
                clsNewCols[i] = srcTable.Columns[i];
            }

            return Distinct(srcTable, clsNewCols);
        }

        /// <summary>
        /// Transforms the original datatable into a new datatable that has as many columns as rows and as many rows as columns contained in the source datatable.
        /// </summary>
        /// <param name="srcTable"></param>
        /// <param name="pivotColumn"></param>
        /// <returns></returns>
        public static DataTable Pivot(DataTable srcTable, string pivotColumn)
        {
            DataTable newTable = new DataTable();
            DataColumn pivotCol = null;
            List<object> values = new List<object>();
            List<int> duplicates = new List<int>();

            // Add rows as columns to new Table
            pivotCol = srcTable.Columns[pivotColumn];
            newTable.Columns.Add(new DataColumn(pivotColumn, typeof(String)));
            for (int i = 0; i < srcTable.Rows.Count; i++)
            {
                try
                {
                    newTable.Columns.Add(new DataColumn(srcTable.Rows[i][pivotCol.Ordinal].ToString(), typeof(String)));
                }
                catch (System.Data.DuplicateNameException)
                {
                    duplicates.Add(i);
                }
            }

            //Execute the Pivot Method
            for (int cols = 0; cols < srcTable.Columns.Count; cols++)
            {
                // The pivot column is already the column name of the result
                if (pivotCol.Ordinal != cols)
                {
                    values.Clear();
                    values.Add(srcTable.Columns[cols].ColumnName);
                    for (int row = 0; row < srcTable.Rows.Count; row++)
                    {
                        if (duplicates.Count > 0 && !duplicates.Contains(row))
                            values.Add(srcTable.Rows[row][cols]);
                    }
                    newTable.LoadDataRow(values.ToArray(), true);
                }
            }
            return newTable;
        }

        /// <summary>
        /// The source DataTable is pivoted around the pivot columns. The name column is used as the column heading. The Value column will be used for the value of the name column.
        /// </summary>
        /// <param name="srcTable"></param>
        /// <param name="name">The column that will be used as the column name.</param>
        /// <param name="value">The column that will be used as the value.</param>
        /// <param name="pivotColumns">The unique identifier of the rows being pivoted</param>
        /// <returns></returns>
        public static DataTable Pivot(DataTable srcTable, DataColumn name, DataColumn value, DataColumn[] pivotColumns)
        {
            // Empty table
            DataTable pivotDT = new DataTable("Pivot");
            List<DataColumn> primaryKey = new List<DataColumn>(pivotColumns.Length);

            // Sort variable
            string strSort = string.Empty;

            // Build Sort expression and primary key
            for (int i = 0; i < pivotColumns.Length; i++)
            {
                strSort += pivotColumns[i].ColumnName + ",";
                primaryKey.Add(new DataColumn(pivotColumns[i].ColumnName, pivotColumns[i].DataType, pivotColumns[i].Expression, pivotColumns[i].ColumnMapping));
                pivotDT.Columns.Add(primaryKey[i]);
            }

            // Add columns
            for (int i = 0; i < srcTable.Columns.Count; i++)
            {
                DataColumn current = srcTable.Columns[i];
                if (i != name.Ordinal && i != value.Ordinal && !primaryKey.Exists(delegate(DataColumn match) { return (match.ColumnName == current.ColumnName); }))
                {
                    pivotDT.Columns.Add(new DataColumn(current.ColumnName, current.DataType, current.Expression, current.ColumnMapping));
                }
            }
            pivotDT.PrimaryKey = primaryKey.ToArray();

            // Select all rows and Sort
            DataRow[] sortedRows = srcTable.Select(string.Empty, strSort.Substring(0, strSort.Length - 1));
            object[] currentKey = null;
            object[] previousKey = null;
            List<object> values = new List<object>();
            DataColumn[] newCols = new DataColumn[pivotDT.Columns.Count];

            pivotDT.Columns.CopyTo(newCols, 0);
            pivotDT.BeginLoadData();
            foreach (DataRow clsRow in sortedRows)
            {
                // Get the Key of the current row
                currentKey = new object[pivotColumns.Length];
                for (int i = 0; i < pivotColumns.Length; i++)
                {
                    currentKey[i] = clsRow[pivotColumns[i].ColumnName];
                }

                // Match Current Row to previous Row
                if (!RowEqual(previousKey, currentKey))
                {
                    // The key is not the same so save the previous key values in the new row
                    if (previousKey != null)
                    {
                        pivotDT.LoadDataRow(values.ToArray(), true);
                    }

                    // Start a new row
                    values = new List<object>();
                    previousKey = new object[pivotColumns.Length];
                    for (int i = 0; i < pivotColumns.Length; i++)
                    {
                        previousKey[i] = clsRow[pivotColumns[i].ColumnName];
                    }
                    int counter = 0;
                    foreach (DataColumn existingCol in newCols)
                    {
                        if (primaryKey.Exists(delegate(DataColumn match) { return (match.ColumnName == existingCol.ColumnName); }))
                            values.Insert(counter++, previousKey[existingCol.Ordinal]);
                        else
                            values.Insert(counter++, clsRow[existingCol.ColumnName]);
                    }
                }

                // Save the column value
                string colName = clsRow[name].ToString();
                if (!pivotDT.Columns.Contains(colName))
                {
                    pivotDT.Columns.Add(new DataColumn(colName, name.DataType, name.Expression, name.ColumnMapping));
                    // Add missing values in the columns that have been added up to this point
                    int ordinal = pivotDT.Columns[colName].Ordinal;
                    for (int i = values.Count; i < ordinal; i++)
                        values.Add(DBNull.Value);
                    values.Add(clsRow[value]);
                }
                else
                {
                    int ordinal = pivotDT.Columns[colName].Ordinal;
                    for (int i = values.Count; i <= ordinal; i++)
                        values.Add(DBNull.Value);
                    // Update the value
                    values[ordinal] = clsRow[value];
                }
            }

            // Add the last row
            if (values != null && values.Count > 0)
            {
                pivotDT.LoadDataRow(values.ToArray(), true);
            }

            // Complete the load
            pivotDT.Constraints.Clear();
            pivotDT.EndLoadData();
            return pivotDT;
        }

        public static DataTable Pivot(DataTable srcTable, DataColumn name, DataColumn value, DataColumn pivotColumns)
        {
            return Pivot(srcTable, name, value, new DataColumn[] { pivotColumns });
        }

        public static DataTable Pivot(DataTable srcTable, string nameColumn, string valueColumn, string pivotColumn)
        {
            return Pivot(srcTable, srcTable.Columns[nameColumn], srcTable.Columns[valueColumn], srcTable.Columns[pivotColumn]);
        }

        public static DataTable Pivot(DataTable srcTable, string nameColumn, string valueColumn, params string[] pivotColumns)
        {
            DataColumn[] keyCols = new DataColumn[pivotColumns.Length];
            for (int i = 0; i < pivotColumns.Length; i++)
            {
                keyCols[i] = srcTable.Columns[pivotColumns[i]];
            }

            return Pivot(srcTable, srcTable.Columns[nameColumn], srcTable.Columns[valueColumn], keyCols);
        }

        private static bool RowEqual(object[] leftColumns, object[] rightColumns)
        {
            if (leftColumns == null)
                return false;

            for (int i = 0; i < leftColumns.Length; i++)
            {
                if (!leftColumns[i].Equals(rightColumns[i]))
                    return false;
            }

            return true;
        }
    }
}
