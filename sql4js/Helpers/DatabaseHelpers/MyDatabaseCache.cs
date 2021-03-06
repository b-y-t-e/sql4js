﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Data.SqlClient;
using System.Data.Common;

namespace sql4js.Helpers.DatabaseHelpers
{
    public static class DatabaseCache
    {
        private static object _lck = new object();

        private static Dictionary<String, DbDataColumns> _columnsCache = new Dictionary<String, DbDataColumns>();

        //////////////////////////////////////////

        public static DbDataColumns GetColumns(
            this DbConnection Connection,
            String TableName)
        {
            lock (_lck)
            {
                TableName = (TableName ?? "").Trim().ToUpper();
                if (!_columnsCache.ContainsKey(TableName))
                {
                    Connection.OpenIfClosed();

                    DbDataColumns columns = new DbDataColumns();
                    using (var lCommand = Connection.CreateCommand())
                    {
                        MyQuery query = new MyQuery();
                        query.Append("select column_name from information_schema.columns where table_name = {0}", TableName);

                        lCommand.CommandTimeout = 60 * 5;
                        lCommand.CommandText = query.ToString();

                        using (var lReader = lCommand.ExecuteReader())
                        {
                            while (lReader.Read())
                            {
                                string name = Convert.ToString(lReader.GetValue(0), CultureInfo.InvariantCulture);
                                columns[name] = new DbDataColumn()
                                {
                                    Name = name,
                                };
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

    public class DbDataColumns : Dictionary<string, DbDataColumn>
    {
        public DbDataColumns()
            : base(StringComparer.OrdinalIgnoreCase)
        {

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
