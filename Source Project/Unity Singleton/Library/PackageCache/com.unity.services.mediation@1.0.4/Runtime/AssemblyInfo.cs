using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.Mediation.TestApp")]
[assembly: InternalsVisibleTo("Unity.Mediation.Tests")]
[assembly: InternalsVisibleTo("Unity.Mediation.Build.Editor")]
//Needed for Moq to generate mocks from internal interfaces
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
