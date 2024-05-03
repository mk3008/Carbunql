using Carbunql.Clauses;
using Carbunql.Definitions;

namespace Carbunql.Annotations;

//public class ClassTableDefinitionClause : TableDefinitionClause
//{
//    public ClassTableDefinitionClause(ITable t) : base(t)
//    {
//    }

//    public ClassTableDefinitionClause(string schema, string table) : base(schema, table)
//    {
//    }

//    public virtual Type Type { get; } = typeof(object);

//    public string Comment { get; set; } = string.Empty;

//    public IEnumerable<PropertyColumnDefinition> PropertyColumnDefinitions => this.OfType<PropertyColumnDefinition>();

//    public List<string> PrimaryKeyProperties { get; init; } = new();

//    public IEnumerable<ParentRelationDefinition> ParentRelationDefinitions => this.OfType<ParentRelationDefinition>();

//    public List<string> ChildProperties { get; init; } = new();
//}

//public class ClassTableDefinitionClause<T> : ClassTableDefinitionClause
//{
//    public ClassTableDefinitionClause(ITable t) : base(t)
//    {
//    }

//    public ClassTableDefinitionClause(string schema, string table) : base(schema, table)
//    {
//    }

//    public override Type Type { get; } = typeof(T);
//}