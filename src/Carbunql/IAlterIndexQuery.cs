using Carbunql.Building;

namespace Carbunql;

public interface IAlterIndexQuery : IQueryCommandable, ICommentable
{
	string? IndexName { get; }
}
