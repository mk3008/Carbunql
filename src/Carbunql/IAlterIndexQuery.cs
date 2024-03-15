using Carbunql.Building;
using Carbunql.Definitions;

namespace Carbunql;

public interface IAlterIndexQuery : IQueryCommandable, ICommentable, ITable
{
	string? IndexName { get; }
}
