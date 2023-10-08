using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Carbunql.PostgresAnalyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class LambdaParameterAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "Cbql001";
		private const string Title = "Lambda parameter names must match";
		private const string MessageFormat = "Lambda parameter '{0}' does not match";
		private const string Category = "Naming";

		private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			//	// TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
			//	// See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
			context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
			//context.RegisterSyntaxNodeAction(AnalyzeLambda, SyntaxKind.SimpleLambdaExpression);
		}


		private static void AnalyzeSymbol(SymbolAnalysisContext context)
		{
			// TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
			var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

			// Find just those named type symbols with names containing lowercase letters.
			if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
			{
				// For all such symbols, produce a diagnostic.
				var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

				context.ReportDiagnostic(diagnostic);
			}
		}

		private static void AnalyzeLambda(SyntaxNodeAnalysisContext context)
		{
			var lambda = (SimpleLambdaExpressionSyntax)context.Node;
			var parameterName = lambda.Parameter.Identifier.Text;
			var body = lambda.Body;

			if (body is BinaryExpressionSyntax binaryExpression && binaryExpression.Left is IdentifierNameSyntax left && left.Identifier.Text != parameterName)
			{
				var diagnostic = Diagnostic.Create(Rule, lambda.GetLocation(), parameterName);
				context.ReportDiagnostic(diagnostic);
			}
		}
	}
}