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
                return reader.GetValue<T>(fieldName);
            }
            
            return defaultValue;
        }    
    }
}