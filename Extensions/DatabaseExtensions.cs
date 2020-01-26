using System;
using System.Data;

namespace LinqToSQL.Net.Extensions
{
    public static class DatabaseExtensions
    {
        public static SqlDbType GetSqlDBType(this DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                    return SqlDbType.Text;
                case DbType.AnsiStringFixedLength:
                    return SqlDbType.Char;
                case DbType.Binary:
                    return SqlDbType.VarBinary;
                case DbType.Boolean:
                    return SqlDbType.Bit;
                case DbType.Byte:
                    return SqlDbType.TinyInt;
                case DbType.Currency:
                    return SqlDbType.Money;
                case DbType.Date:
                    return SqlDbType.DateTime;
                case DbType.DateTime:
                    return SqlDbType.DateTime;
                case DbType.Decimal:
                    return SqlDbType.Decimal;
                case DbType.Double:
                    return SqlDbType.Float;
                case DbType.Guid:
                    return SqlDbType.UniqueIdentifier;
                case DbType.Int16:
                    return SqlDbType.Int;
                case DbType.Int32:
                    return SqlDbType.Int;
                case DbType.Int64:
                    return SqlDbType.BigInt;
                case DbType.Object:
                    return SqlDbType.Variant;
                case DbType.SByte:
                    return SqlDbType.TinyInt;
                case DbType.Single:
                    return SqlDbType.Real;
                case DbType.String:
                    return SqlDbType.Text;
                case DbType.StringFixedLength:
                    return SqlDbType.NChar;
                case DbType.Time:
                    return SqlDbType.DateTime;
                case DbType.UInt16:
                    return SqlDbType.Int;
                case DbType.UInt32:
                    return SqlDbType.Int;
                case DbType.UInt64:
                    return SqlDbType.BigInt;
                case DbType.VarNumeric:
                    return SqlDbType.Decimal;

                default:
                    return SqlDbType.VarChar;
            }
        }

        public static DbType? GetDbType(Type type)
        {
            DbType? result = null;

            if (type == typeof(Int32) || type == typeof(Int32?))
                result = DbType.Int32;
            else if (type == typeof(Byte) || type == typeof(Byte?))
                result = DbType.Byte;
            else if (type == typeof(SByte) || type == typeof(SByte?))
                result = DbType.SByte;
            else if (type == typeof(Int16) || type == typeof(Int16?))
                result = DbType.Int16;
            else if (type == typeof(Int64) || type == typeof(Int64?))
                result = DbType.Int64;
            else if (type == typeof(UInt32) || type == typeof(UInt32?))
                result = DbType.UInt32;
            else if (type == typeof(UInt16) || type == typeof(UInt16?))
                result = DbType.UInt16;
            else if (type == typeof(UInt64) || type == typeof(UInt64?))
                result = DbType.UInt64;
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
                result = DbType.DateTime;
            else if (type == typeof(float) || type == typeof(float?))
                result = DbType.Decimal;
            else if (type == typeof(decimal) || type == typeof(decimal?))
                result = DbType.Decimal;
            else if (type == typeof(double) || type == typeof(double?))
                result = DbType.Double;
            else if (type == typeof(Guid) || type == typeof(Guid?))
                result = DbType.Guid;
            else if (type == typeof(bool) || type == typeof(bool?))
                result = DbType.Boolean;
            else if (type == typeof(byte[]) || type == typeof(byte[]))
                result = DbType.Binary;
            else if (type == typeof(string))
                result = DbType.String;

            return result;
        }
    }
}
