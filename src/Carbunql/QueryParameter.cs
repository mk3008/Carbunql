using MessagePack;
using System.Data;

namespace Carbunql;

[MessagePackObject(keyAsPropertyName: true)]
public class QueryParameter : IDbDataParameter
{
	public QueryParameter(string parameterName, object? value)
	{
		ParameterName = parameterName;
		Value = value;
	}

	public byte Precision { get; set; }

	public byte Scale { get; set; }

	public int Size { get; set; }

	public DbType DbType { get; set; } = DbType.String;

	public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

	public bool IsNullable => ValidateNullable();

	private bool ValidateNullable()
	{
		if (Value == null) return true;
		return Value.GetType().IsGenericType && Value.GetType().GetGenericTypeDefinition() == typeof(Nullable<>);
	}

#pragma warning disable CS8767
	public string ParameterName { get; set; }
#pragma warning restore CS8767

#pragma warning disable CS8767
	public string SourceColumn { get; set; } = string.Empty;
#pragma warning restore CS8767

	public DataRowVersion SourceVersion { get; set; } = DataRowVersion.Default;

	public object? Value { get; set; }
}
