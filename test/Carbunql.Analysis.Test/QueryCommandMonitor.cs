using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;
internal class QueryCommandMonitor
{
	private readonly ITestOutputHelper Output;

	public QueryCommandMonitor(ITestOutputHelper output)
	{
		Output = output;
	}

	public void Log(IQueryCommand arg)
	{
		var frm = new CommandFormatter();
		var bld = new CommandTextBuilder(frm);
		var sql = bld.Execute(arg.GetTokens(null));
		Output.WriteLine(sql);
		Output.WriteLine("--------------------");
		var len = 20;
		var indent = string.Empty;
		foreach (var item in arg.GetTokens(null))
		{
			var p = item.Parent == null ? "[null]" : item.Parent.Text;
			var s = item.Sender.GetType().Name;
			var l = item.Parents().Count();
			Output.WriteLine($"{(indent + item.Text).PadRight(len)} l:{l} s:{s.PadRight(15)}, p:{p.PadRight(6)}, r:{item.IsReserved}");
		}
	}


	//private void LogCore(IQueryCommand arguments, int indent = 0)
	//{
	//    if (arguments is CaseExpression caseexp)
	//    {
	//        LogCore(caseexp, indent);
	//        return;
	//    }
	//    else if (arguments is WindowFunction winfnarg)
	//    {
	//        LogCore(winfnarg, indent);
	//        return;
	//    }
	//    else if (arguments is ValueCollection values)
	//    {
	//        LogCore(values, indent);
	//        return;
	//    }
	//    else if (arguments is BracketValue bracket)
	//    {
	//        LogCore(bracket, indent);
	//        return;
	//    }
	//    else if (arguments is FunctionValue fnvalue)
	//    {
	//        LogCore(fnvalue, indent);
	//        return;
	//    }
	//    else if (arguments is ValueBase val)
	//    {
	//        LogCore(val, indent);
	//        return;
	//    }
	//    else if (arguments is SelectableItem selectable)
	//    {
	//        LogCore(selectable, indent);
	//        return;
	//    }
	//    else if (arguments is SelectClause selectclauese)
	//    {
	//        LogCore(selectclauese, indent);
	//        return;
	//    }
	//    else if (arguments is PhysicalTable physicalTable)
	//    {
	//        LogCore(physicalTable, indent);
	//        return;
	//    }
	//    else if (arguments is ValuesQuery valuesTable)
	//    {
	//        LogCore(valuesTable, indent);
	//        return;
	//    }
	//    else if (arguments is SelectableTable selectableTable)
	//    {
	//        LogCore(selectableTable, indent);
	//        return;
	//    }
	//    else if (arguments is Relation relation)
	//    {
	//        LogCore(relation, indent);
	//        return;
	//    }
	//    else if (arguments is FromClause fromclause)
	//    {
	//        LogCore(fromclause, indent);
	//        return;
	//    }
	//    else if (arguments is VirtualTable virtualTable)
	//    {
	//        LogCore(virtualTable, indent);
	//        return;
	//    }
	//    else if (arguments is WhereClause whereclause)
	//    {
	//        LogCore(whereclause, indent);
	//        return;
	//    }
	//    else if (arguments is GroupClause groupclause)
	//    {
	//        LogCore(groupclause, indent);
	//        return;
	//    }
	//    else if (arguments is HavingClause havingclause)
	//    {
	//        LogCore(havingclause, indent);
	//        return;
	//    }
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
	//}

	//private void LogCore(HavingClause arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

	//    if (arguments.Condition != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        Output.WriteLine($"{s}Condition");
	//        LogCore(arguments.Condition, indent + 4);
	//    }
	//}

	//private void LogCore(GroupClause arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

	//    var s = (indent + 2).ToSpaceString();
	//    Output.WriteLine($"{s}Collection");

	//    foreach (var item in arguments)
	//    {
	//        LogCore(item, indent + 4);
	//    }
	//}

	//private void LogCore(WhereClause arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

	//    if (arguments.Condition != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        Output.WriteLine($"{s}Condition");
	//        LogCore(arguments.Condition, indent + 4);
	//    }
	//}

	//private void LogCore(FromClause arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

	//    if (arguments.Root != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        Output.WriteLine($"{s}Root");
	//        LogCore(arguments.Root, indent + 4);
	//    }

	//    if (arguments.Relations != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        Output.WriteLine($"{s}Relations");
	//        foreach (var item in arguments.Relations)
	//        {
	//            LogCore(item, indent + 4);
	//        }
	//    }
	//}

	//private void LogCore(Relation arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
	//    Output.WriteLine($"{space}Relation : {arguments.RelationType.ToCommandText()}");

	//    if (arguments.Table != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        Output.WriteLine($"{s}Table");
	//        LogCore(arguments.Table, indent + 4);
	//    }

	//    if (arguments.Condition != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        Output.WriteLine($"{s}on");
	//        LogCore(arguments.Condition, indent + 4);
	//    }
	//}

	//private void LogCore(SelectableTable arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
	//    Output.WriteLine($"{space}Alias : {arguments.Alias}");

	//    if (arguments.ColumnAliases != null)
	//    {
	//        foreach (var item in arguments.ColumnAliases)
	//        {
	//            var s = (indent + 2).ToSpaceString();
	//            Output.WriteLine($"{s}column name");
	//            LogCore(item, indent + 4);
	//        }
	//    }

	//    if (arguments.Table != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        Output.WriteLine($"{s}Table");
	//        LogCore(arguments.Table, indent + 4);
	//    }
	//}

	//private void LogCore(VirtualTable arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

	//    if (arguments.Query != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        Output.WriteLine($"{s}Table");
	//        LogCore(arguments.Query, indent + 4);
	//    }
	//}

	//private void LogCore(ValuesQuery arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

	//    var s = (indent + 2).ToSpaceString();
	//    Output.WriteLine($"{s}rows");
	//    //foreach (var item in arguments.Rows)
	//    //{
	//    //    LogCore(item, indent + 4);
	//    //}
	//}

	//private void LogCore(PhysicalTable arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
	//    Output.WriteLine($"{space}DefaultName : {arguments.GetDefaultName()}");
	//}

	//private void LogCore(SelectClause arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
	//    Output.WriteLine($"{space}Count : {arguments.Count}");

	//    var s = (indent + 2).ToSpaceString();
	//    Output.WriteLine($"{s}items");
	//    foreach (var item in arguments)
	//    {
	//        LogCore(item, indent + 4);
	//    }
	//}

	//private void LogCore(SelectableItem arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
	//    Output.WriteLine($"{space}Alias : {arguments.Alias}");

	//    if (arguments.Value != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        Output.WriteLine($"{s}Value");
	//        LogCore(arguments.Value, indent + 4);
	//    }
	//}

	//private void LogCore(CaseExpression arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
	//    Output.WriteLine($"{space}CurrentCommand : {arguments.GetCurrentCommandText()}");
	//    Output.WriteLine($"{space}DefaultName : {arguments.GetDefaultName()}");

	//    if (arguments.CaseCondition != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        Output.WriteLine($"{s}Condition");
	//        LogCore(arguments.CaseCondition, indent + 4);
	//    }

	//    if (arguments.WhenExpressions != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        foreach (var item in arguments.WhenExpressions)
	//        {
	//            if (item.Condition != null)
	//            {
	//                Output.WriteLine($"{s}when");
	//                LogCore(item, indent + 4);
	//            }
	//            else
	//            {
	//                Output.WriteLine($"{s}else");
	//                LogCore(item, indent + 4);
	//            }
	//        }
	//    }

	//    if (arguments.OperatableValue != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        var v = arguments.OperatableValue;
	//        Output.WriteLine($"{s}Operator : {v.Operator}");
	//        LogCore(v.Value, indent + 4);
	//    }
	//}

	//private void LogCore(WhenExpression arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

	//    if (arguments.Condition != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        var v = arguments.Condition;
	//        Output.WriteLine($"{s}Condition");
	//        LogCore(arguments.Condition, indent + 4);
	//    }

	//    if (arguments.Value != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        var v = arguments.Condition;
	//        Output.WriteLine($"{s}Value");
	//        LogCore(arguments.Value, indent + 4);
	//    }
	//}


	//private void LogCore(ValueBase arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
	//    Output.WriteLine($"{space}CurrentCommand : {arguments.GetCurrentCommandText()}");
	//    Output.WriteLine($"{space}DefaultName : {arguments.GetDefaultName()}");

	//    if (arguments.OperatableValue != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        var v = arguments.OperatableValue;
	//        Output.WriteLine($"{s}Operator : {v.Operator}");
	//        LogCore((IQueryCommand)v.Value, indent + 4);
	//    }
	//}

	//private void LogCore(WindowFunction arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

	//    if (arguments.PartitionBy != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        Output.WriteLine($"{s}partition by");
	//        LogCore(arguments.PartitionBy, indent + 4);
	//    }

	//    if (arguments.OrderBy != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        Output.WriteLine($"{s}order by");
	//        LogCore(arguments.OrderBy, indent + 4);
	//    }
	//}

	//private void LogCore(BracketValue arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

	//    if (arguments.Inner != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        var v = arguments.OperatableValue;
	//        Output.WriteLine($"{s}Inner");
	//        LogCore(arguments.Inner, indent + 4);
	//    }

	//    if (arguments.OperatableValue != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        var v = arguments.OperatableValue;
	//        Output.WriteLine($"{s}Operator : {v.Operator}");
	//        LogCore(v.Value, indent + 4);
	//    }
	//}

	//private void LogCore(ValueCollection arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
	//    Output.WriteLine($"{space}Count : {arguments.Count}");

	//    var s = (indent + 2).ToSpaceString();
	//    Output.WriteLine($"{s}items");
	//    foreach (var item in arguments)
	//    {
	//        LogCore(item, indent + 4);
	//    }
	//}

	//private void LogCore(FunctionValue arguments, int indent = 0)
	//{
	//    var space = indent.ToSpaceString();

	//    Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
	//    Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

	//    if (arguments.Argument != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        Output.WriteLine($"{s}Arguments");
	//        LogCore(arguments.Argument, indent + 4);
	//    }

	//    if (arguments.WindowFunction != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        Output.WriteLine($"{s}WindowsFunction");
	//        LogCore(arguments.WindowFunction, indent + 4);
	//    }

	//    if (arguments.OperatableValue != null)
	//    {
	//        var s = (indent + 2).ToSpaceString();
	//        var v = arguments.OperatableValue;
	//        Output.WriteLine($"{s}Operator : {v.Operator}");
	//        LogCore(v.Value, indent + 4);
	//    }
	//}
}