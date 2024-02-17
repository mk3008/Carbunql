using Carbunql.Tables;

namespace Carbunql.Clauses;

public class ConstraintDefinition : IQueryCommandable
{
	public IEnumerable<CommonTable> GetCommonTables()
	{
		throw new NotImplementedException();
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		throw new NotImplementedException();
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		throw new NotImplementedException();
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		throw new NotImplementedException();
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		throw new NotImplementedException();
	}
}
