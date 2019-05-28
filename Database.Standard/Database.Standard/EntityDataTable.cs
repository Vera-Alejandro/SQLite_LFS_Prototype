using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Reflection;

namespace Interstates.Control.Database
{
    public abstract class EntityDataTable : DataTable
    {
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public virtual DataTable Load()
        {
            return new DataTable();
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        protected DataTable GetDataTable(System.Collections.IList list, Type typ)
        {
            DataTable dt = new DataTable();
            PropertyInfo[] pi = typ.GetProperties();
            List<int> listMapping = new List<int>(pi.Length);
            int intCount = 0;

            foreach (PropertyInfo p in pi)
            {
                try
                {
                    dt.Columns.Add(new DataColumn(p.Name, p.PropertyType));
                    listMapping.Add(intCount++);
                }
                catch { } // Not all property types will be acceptable
            }

            foreach (object obj in list)
            {
                object[] row = new object[dt.Columns.Count];
                int i = 0;

                // get the property values that are to be stored in the DataTable
                foreach (DataColumn col in dt.Columns)
                {
                    PropertyInfo prop = pi[listMapping[col.Ordinal]];
                    row[i++] = prop.GetValue(obj, null);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
