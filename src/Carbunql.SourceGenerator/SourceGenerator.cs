using Microsoft.CodeAnalysis;

namespace Carbunql.SourceGenerator;

[Generator]
public class SourceGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		throw new Exception("run source generator");
	}
}