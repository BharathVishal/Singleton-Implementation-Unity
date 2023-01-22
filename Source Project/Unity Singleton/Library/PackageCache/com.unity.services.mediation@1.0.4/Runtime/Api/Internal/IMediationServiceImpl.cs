using System.Threading.Tasks;

namespace Unity.Services.Mediation
{
    interface IMediationServiceImpl : IMediationService
    {
        Task Initialize(string gameId, string installId);

        InitializationState InitializationState { get; }
    }
}
