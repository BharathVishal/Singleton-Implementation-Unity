using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.Mediation.Editor.Tests")]
[assembly: InternalsVisibleTo("Unity.Mediation.Settings.Editor")]
[assembly: InternalsVisibleTo("Unity.Mediation.Build.Editor")]
[assembly: InternalsVisibleTo("Unity.Mediation.TestApp.Editor")]
[assembly: InternalsVisibleTo("UnityEditor.GameGrowth.Helpers.UnityMediation")]
//Needed for Moq to generate mocks from internal interfaces
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
