namespace Fanex.Athena.BetList.DataAccess.Util
{
    using Common.DataAccess.Extensions;
    using Fanex.Common.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class SafeDataReader(IDataReader reader, IEnumerable<string> columns) : ISafeDataReader
    {
        private readonly HashSet<string> _existingColumns = [.. columns.Where(reader.HasColumn)];

        public bool Read()
        {
            return reader.Read();
        }

        public bool GetBool(string column, bool defaultValue = false)
        {
            var value = GetValue(column, defaultValue);

            if (value == null || value == Convert.DBNull)
            {
                return false;
            }

            if (value is bool)
            {
                return Convert.ToBoolean(value);
            }

            if (value is int)
            {
                return Convert.ToInt32(value) > 0;
            }

            return false;
        }

        public object GetValue(string column, object defaultValue = null)
        {
            if (_existingColumns.Contains(column))
            {
                return reader[column];
            }

            return defaultValue;
        }

        public string GetString(string column, string defaultValue = "")
        {
            if (_existingColumns.Contains(column))
            {
                return reader.ToString(column);
            }

            return defaultValue;
        }

        public int GetInt(string column, int defaultValue = 0)
        {
            if (_existingColumns.Contains(column))
            {
                return reader.ToInt(column);
            }

            return defaultValue;
        }

        public long GetLong(string column, long defaultValue = 0)
        {
            if (_existingColumns.Contains(column))
            {
                return reader.ToLong(column);
            }

            return defaultValue;
        }

        public long? GetNullableLong(string column, long? defaultValue = null)
        {
            if (_existingColumns.Contains(column))
            {
                var value = reader[column];
                if (value != DBNull.Value)
                {
                    return (long)value;
                }
            }

            return defaultValue;
        }

        public decimal GetDecimal(string column, decimal defaultValue = 0)
        {
            if (_existingColumns.Contains(column))
            {
                return Convert.ToDecimal(reader[column]);
            }

            return defaultValue;
        }

        public DateTime GetDateTime(string column, DateTime defaultValue)
        {
            if (_existingColumns.Contains(column))
            {
                return reader.ToDateTime(column);
            }

            return defaultValue;
        }

        public byte GetByte(string column, byte defaultValue = 0)
        {
            if (_existingColumns.Contains(column))
            {
                return reader.ToByte(column);
            }

            return defaultValue;
        }
    }
}