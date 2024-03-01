using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

public interface ITableDefinition : IQueryCommandable
{
	IEnumerable<AlterTableQuery> ToAlterTableQueries(ITable t);

	bool TryToPlainColumn(ITable t, [MaybeNullWhen(false)] out ColumnDefinition column);
}
