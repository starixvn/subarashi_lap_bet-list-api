namespace Fanex.Athena.Common.DataAccess.Extensions
{
    using Fanex.Common.Extensions;
    using System;
    using System.Data;

    public static class DataReaderExtensions
    {
        public static bool HasColumn(this IDataRecord reader, string columnName)
        {
            var fieldCount = reader.FieldCount;

            for (var i = 0; i < fieldCount; i++)
            {
                var name = reader.GetName(i);
                if (name.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static int? ToNullableInt(this IDataReader reader, string fieldName, int? defaultValue = null)
        {
            return reader[fieldName] != DBNull.Value ? reader.ToInt(fieldName) : defaultValue;
        }

        public static T GetVal<T>(this IDataReader reader, string fieldName, T defaultValue = default)

        {
            if (reader.HasColumn(fieldName)
                && !reader.IsDBNull(fieldName))
            {
                return GetValue<T>(reader[fieldName]);
            }

            return defaultValue;
        }

        private static T GetValue<T>(object value)
        {
            var t = typeof(T);

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null)
                {
                    return default(T);
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return (T)Convert.ChangeType(value, t);
        }
    }
}