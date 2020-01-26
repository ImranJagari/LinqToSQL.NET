using LinqToSQL.Net.Attributes.Columns;
using LinqToSQL.Net.Attributes.Columns;
using LinqToSQL.Net.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LinqToSQL.Net
{
    public class QueryBuilder<T>
    {
        List<ParameterExpression> _rootParameters;
        private Expression<Func<T, bool>> _whereExpressions;
        private List<string> _groupByExpressions;
        private List<string> _orderByExpressions;
        private List<string> _projectionExpressions;
        private List<string> _sumExpressions;
        private List<string> _countExpressions;
        private Dictionary<Type, (string, string)> _joinExpressions;
        private int _limitValue;
        private int _offsetValue;

        public QueryBuilder()
        {
            _rootParameters = new List<ParameterExpression>();
            _whereExpressions = null;
            _groupByExpressions = new List<string>();
            _orderByExpressions = new List<string>();
            _projectionExpressions = new List<string>();
            _sumExpressions = new List<string>();
            _countExpressions = new List<string>();
            _joinExpressions = new Dictionary<Type, (string, string)>();
        }

        public QueryBuilder<T> Select(params Expression<Func<T, object>>[] expressions)
        {
            this._projectionExpressions.Add(string.Join(",", expressions.Select(x => ConvertExpressionToString(x))));
            return this;
        }
        public QueryBuilder<T> SelectAll()
        {
            this._projectionExpressions = Properties();

            return this;
        }

        public QueryBuilder<T> Where(Expression<Func<T, bool>> expression)
        {
            _rootParameters.AddRange(expression.Parameters);

            _whereExpressions = _whereExpressions == null
                ? expression
                : Expression.Lambda<Func<T, bool>>(
                    Expression.AndAlso(_whereExpressions.Body, expression.Body),
                    _whereExpressions.Parameters);
            return this;
        }

        private string Where()
        {
            if (_whereExpressions != null)
            {
                var where = ConvertExpressionToString(_whereExpressions.Body);
                return string.IsNullOrEmpty(where) ? string.Empty : string.Format("WHERE {0}", where);
            }
            return string.Empty;
        }

        public QueryBuilder<T> Join<J>(Expression<Func<T, object>> left, Expression<Func<J, object>> right)
        {
            _rootParameters.AddRange(left.Parameters);
            _rootParameters.AddRange(right.Parameters);

            _joinExpressions.Add(typeof(J),(ConvertExpressionToString(left), ConvertExpressionToString(right)));
            return this;
        }
        private QueryBuilder<T> Join(Type joinTable)
        {
            if (_joinExpressions.ContainsKey(joinTable))
                return this;

            _joinExpressions.Add(joinTable,
                (typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(x => x.PropertyType == joinTable)
                .GetCustomAttribute<ForeignKeyAttribute>().KeyName,
                joinTable.GetCustomAttribute<PrimaryKeyAttribute>().ColumnName));

            return this;
        }
        private string Join()
        {
            return string.Join(" ",
                _joinExpressions.Select(
                    x =>
                        string.Format("INNER JOIN {0} ON {1}.{2}={3}.{4}", GetTableName(x.Key), GetTableName(typeof(T)), x.Value.Item1,
                            GetTableName(x.Key), x.Value.Item2)));
        }

        public string GetTableName(Type type)
        {
            return type.GetCustomAttribute<TableNameAttribute>().Name;
        }

        public QueryBuilder<T> GroupBy(params Expression<Func<T, object>>[] expression)
        {
            _groupByExpressions.AddRange(expression.Select(ConvertExpressionToString));
            return this;
        }

        private string GroupBy()
        {
            return _groupByExpressions.Any() ? string.Format("GROUP BY {0}", string.Join(",", _groupByExpressions)) : string.Empty;
        }

        public QueryBuilder<T> OrderBy(params Expression<Func<T, object>>[] expression)
        {
            _orderByExpressions.AddRange(expression.Select(ConvertExpressionToString));
            return this;
        }

        private string OrderBy()
        {
            return _orderByExpressions.Any() ? string.Format("ORDER BY {0}", string.Join(",", _orderByExpressions)) : string.Empty;
        }


        public QueryBuilder<T> Limit(int limit)
        {
            _limitValue = limit;
            return this;
        }

        private string Limit()
        {
            if (_limitValue > 0)
                return string.Format("LIMIT {0}", _limitValue);
            return string.Empty;
        }

        public QueryBuilder<T> Offset(int offset)
        {
            _offsetValue = offset;
            return this;
        }

        private string Offset()
        {
            if (_offsetValue > 0)
                return string.Format("OFFSET {0}", _offsetValue);
            return string.Empty;
        }


        public QueryBuilder<T> Sum(params Expression<Func<T, object>>[] expression)
        {
            _sumExpressions.AddRange(expression.Select(ConvertExpressionToString));
            return this;
        }
        private string Sum()
        {
            return string.Join(", ", _sumExpressions.Select(x => string.Format("SUM({0}) AS `{0}`", x)));
        }
        public QueryBuilder<T> Count(params Expression<Func<T, object>>[] expression)
        {
            _countExpressions.AddRange(expression.Select(ConvertExpressionToString));
            return this;
        }
        private string Count()
        {
            return string.Join(", ", _countExpressions.Select(x => string.Format("COUNT({0}) AS `{0}`", x)));
        }

        public List<string> Projections()
        {
            var properties = _projectionExpressions.Any() ? _projectionExpressions : new List<string>();

            properties.AddRangeIf(_groupByExpressions, elem => !properties.Contains(elem));

            var propertiesTabled = (_projectionExpressions.Any() ? _projectionExpressions : properties).Select(x => GetTableName(typeof(T)) + '.' + x).ToList();
            return propertiesTabled;
        }

        private string Projection()
        {
            return string.Join(", ", Projections());
        }

        private List<string> Properties()
        {
            var properties = typeof(T).GetProperties().Where(prop => !Attribute.IsDefined(prop, typeof(IgnoreAttribute)));
            return properties.Select(prop => prop.Name).ToList();
        }

        private string ConvertExpressionToString(Expression body)
        {
            if (body == null)
            {
                return string.Empty;
            }
            if (body is ConstantExpression)
            {
                return ValueToString(((ConstantExpression)body).Value);
            }
            if (body is MemberExpression)
            {
                var member = ((MemberExpression)body);

                if (member.Member.MemberType == MemberTypes.Property && _rootParameters.Exists(x => x.Name == member.Member.Name))
                    return null;

                if (member.Member.MemberType == MemberTypes.Property)
                {
                    if (DatabaseExtensions.GetDbType(member.Type).HasValue)
                    {
                        string baseParameter = ConvertExpressionToString(member.Expression);
                        string sqlPart = !string.IsNullOrEmpty(baseParameter) ? baseParameter + '.' + member.Member.Name + (member.Type == typeof(bool) ? "=1" : "") : GetTableName(typeof(T)) + '.' + (member.Member.Name + (member.Type == typeof(bool) ? "=1" : ""));
                        return sqlPart.Trim();
                    }
                    else
                    {
                        this.Join(member.Type);
                        return GetTableName(member.Type).Trim();
                    }
                }
                var value = GetValueOfMemberExpression(member);
                if (value is IEnumerable && !(value is string))
                {
                    var sb = new StringBuilder();
                    foreach (var item in value as IEnumerable)
                    {
                        sb.AppendFormat("{0},", ValueToString(item));
                    }
                    return sb.Remove(sb.Length - 1, 1).ToString().Trim();
                }
                return ValueToString(value).Trim();
            }
            if (body is UnaryExpression)
            {
                string sqlPart = $"{ConvertExpressionTypeToString(((UnaryExpression)body).NodeType)} ({ConvertExpressionToString(((UnaryExpression)body).Operand)})";
                return sqlPart.Trim();
            }
            if (body is BinaryExpression)
            {
                var binary = body as BinaryExpression;
                return string.Format("({0}{1}{2})", ConvertExpressionToString(binary.Left),
                    ConvertExpressionTypeToString(binary.NodeType),
                    ConvertExpressionToString(binary.Right)).Trim();
            }
            if (body is MethodCallExpression)
            {
                var method = body as MethodCallExpression;
                return string.Format("({0} IN ({1}))", ConvertExpressionToString(method.Arguments[0]),
                    ConvertExpressionToString(method.Object)).Trim();
            }
            if (body is LambdaExpression)
            {
                return ConvertExpressionToString(((LambdaExpression)body).Body).Trim();
            }
            return "";
        }

        private static string ValueToString(object value)
        {
            if (value is bool)
            {
                return Convert.ChangeType(value, typeof(int)).ToString();
            }
            if (value is string)
            {
                return string.Format("'{0}'", value);
            }
            if (value is DateTime)
            {
                return string.Format("'{0:yyyy-MM-dd HH:mm:ss}'", value);
            }
            return value.ToString();
        }

        private static object GetValueOfMemberExpression(MemberExpression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

        private static string ConvertExpressionTypeToString(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.And:
                    return " AND ";
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Or:
                    return " OR ";
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.Not:
                    return "NOT";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                default:
                    return "";
            }
        }

        public static implicit operator string(QueryBuilder<T> query) => query.GetQueryString();

        private string GetQueryString()
        {
            string projection = CheckAndTrim(Projection());
            string sum = CheckAndTrim(Sum());
            string count = CheckAndTrim(Count());

            string queries = "";
            if (!string.IsNullOrEmpty(projection))
                queries = projection;

            if (!string.IsNullOrEmpty(sum) && !string.IsNullOrEmpty(queries))
                queries = string.Join(", ", queries, sum);
            else if(!string.IsNullOrEmpty(sum))
                queries = sum;

            if (!string.IsNullOrEmpty(count) && !string.IsNullOrEmpty(queries))
                queries = string.Join(", ", queries, count);
            else if (!string.IsNullOrEmpty(count))
                queries = count;

            string where = Where();
            string join = Join();
            string groupBy = GroupBy();
            string orderBy = OrderBy();
            string offset = Offset();
            string limit = Limit();
            return string.Format("SELECT {0} FROM {1} {2} {3} {4} {5} {6} {7}", queries,
                GetTableName(typeof(T)), join, where, groupBy, orderBy, offset, limit).Trim();
        }

        public string CheckAndTrim(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Trim();
                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            return null;
        }
    }
}
