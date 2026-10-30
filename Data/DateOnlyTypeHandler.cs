using System.Data;
using Dapper;

namespace PrestQR_API.Data
{
    /// <summary>
    /// Permite que Dapper lea y envíe columnas <c>date</c> de PostgreSQL como <see cref="DateOnly"/>
    /// (por ejemplo prestamos.fecha_limite).
    /// </summary>
    public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = value;
        }

        public override DateOnly Parse(object value) => value switch
        {
            DateOnly fecha => fecha,
            DateTime fechaHora => DateOnly.FromDateTime(fechaHora),
            _ => throw new InvalidCastException($"No se puede convertir {value.GetType()} a DateOnly.")
        };
    }
}
