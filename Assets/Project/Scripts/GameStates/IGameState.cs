using System.Threading;
using Cysharp.Threading.Tasks;

namespace LWFlo
{
    public interface IGameState
    {
        UniTask OnInitialize(CancellationToken cancellationToken);
        UniTask OnRun(CancellationToken cancellationToken);
    }
}