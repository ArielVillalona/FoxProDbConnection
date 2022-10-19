using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FoxProDbExtentionConnection
{
    public static class SystemDataExtension
    {
        public static async Task<IEnumerable<T>> ToListAsync<T>(this DataSet ds)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                return await Task.FromResult(ds.Tables[0].ConvertToList<T>());
            }
            return null;
        }
        public static async Task<T> FirstAsync<T>(this DataSet ds)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                return await Task.FromResult(ds.Tables[0].Rows[0].OnlyOne<T>());
            }
            return default;
        }
        public static IEnumerable<T> ToList<T>(this DataSet ds)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].ConvertToList<T>();
            }
            return null;
        }
        public static T First<T>(this DataSet ds)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0].OnlyOne<T>();
            }
            return default;
        }

        #region Private Members
        private static T OnlyOne<T>(this DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            var dataTableName = from c in dr.Table.Columns.Cast<DataColumn>() select new { name = c.ColumnName.Trim().ToLower(), type = c.DataType.Name };

            foreach (var pro in temp.GetProperties().Where(x => dataTableName.Any(y => string.Equals(y.name, x.Name, StringComparison.OrdinalIgnoreCase))).ToList())
            {
                var columnName = pro.Name.Trim().ToLower();
                var columnData = dr[columnName];
                var columnDataType = dataTableName.FirstOrDefault(x => x.name == columnName).type;
                var EntityType = pro.PropertyType;

                try
                {
                    //si el tipo de datos es el mismo
                    if (columnDataType == pro.PropertyType.Name)
                    {
                        if (columnDataType == "String")
                            pro.SetValue(obj, columnData.ToString().Trim(), null);
                        else
                            pro.SetValue(obj, columnData, null);
                    }
                    else//si el tipo de datos no es el mismo
                    {
                        if (pro.PropertyType == typeof(Int32))
                        {
                            Regex regex = new Regex("^(?:-(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))|(?:0|(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))))(?:.\\d+|)$");
                            if (regex.IsMatch(columnData.ToString()??"0"))
                                pro.SetValue(obj, Convert.ChangeType(columnData, pro.PropertyType), null);
                            else
                                throw new Exception();
                        }
                        else
                            pro.SetValue(obj, Convert.ChangeType(columnData, pro.PropertyType), null);
                    }
                }
                catch (Exception)
                {
                    throw new Exception($@"Error al intentar convertir '{columnName}' con valor '{columnData}' de tipo '{columnDataType}' a '{EntityType.Name}'");
                }
            }
            return obj;
        }

        private static List<T> ConvertToList<T>(this DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = row.OnlyOne<T>();
                data.Add(item);
            }
            return data;
        }
        #endregion
    }
}