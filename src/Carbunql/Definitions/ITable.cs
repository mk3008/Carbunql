namespace Carbunql.Definitions;

public interface ITable
{
	string? Schema { get; init; }

	string Table { get; init; }
}
