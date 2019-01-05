using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Data.SqlClient;
using System.Data.Common;

namespace sql4js.Helpers.DatabaseHelpers
{
    public static class DatabaseHelper
    {
        private static object _lck = new object();

        private static Dictionary<String, IList<DbDataColumn>> _columnsCache = new Dictionary<String, IList<DbDataColumn>>();

        //////////////////////////////////////////

        public static IList<DbDataColumn> GetColumns(
            this DbConnection Connection,
            String TableName)
        {
            lock (_lck)
            {
                TableName = (TableName ?? "").Trim().ToUpper();
                if (!_columnsCache.ContainsKey(TableName))
                {
                    Connection.OpenIfClosed();

                    List<DbDataColumn> columns = new List<DbDataColumn>();
                    using (var lCommand = Connection.CreateCommand())
                    {
                        lCommand.CommandTimeout = 60 * 5;
                        lCommand.CommandText = " PRAGMA table_info(" + TableName + "); ";
                       
                        using (var lReader = lCommand.ExecuteReader())
                        {
                            while (lReader.Read())
                            {
                                columns.Add(new DbDataColumn()
                                {
                                    Name = Convert.ToString(lReader.GetValue(1), CultureInfo.InvariantCulture),
                                });
                            }
                        }
                    }
                    _columnsCache[TableName] = columns;
                }
                return _columnsCache[TableName];
            }
        }

        public static void ClearCache()
        {
            lock (_lck)
            {
                _columnsCache.Clear();
            }
        }

        public static Int32 SPID(
            this DbConnection Connection)
        {
            return Connection.SelectScalar<Int32>("SELECT @@SPID");
        }

        public static Boolean IsValidConnection(
            this DbConnection Connection)
        {
            lock (_lck)
            {
                Connection.OpenIfClosed();
                var lValue = Connection.SelectScalar<Int32>("select 1");
                return lValue == 1;
            }
        }

    }


    public class DbDataColumn
    {
        public String Name;

        // public Boolean IsDateTime;

        public String Default;

        public Boolean IsNull;

        public String ValueForNull;
    }
}
