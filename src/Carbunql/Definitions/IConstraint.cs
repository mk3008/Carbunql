﻿namespace Carbunql.Definitions;

//CONSTRAINT sale_pkey PRIMARY KEY (sale_id)
public interface IConstraint : ITableDefinition
{
	string ConstraintName { get; }
}
