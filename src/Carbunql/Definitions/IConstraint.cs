namespace Carbunql.Definitions;

//CONSTRAINT sale_pkey PRIMARY KEY (sale_id)
public interface IConstraint : ITableDefinition
{
}

public static class IConstraintExtension
{
	public static AddConstraintCommand ToAddCommand(this IConstraint constraint)
	{
		return new AddConstraintCommand(constraint);
	}
}

