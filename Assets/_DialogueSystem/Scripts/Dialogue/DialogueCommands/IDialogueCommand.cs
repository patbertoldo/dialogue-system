using Cysharp.Threading.Tasks;

namespace Dialogue
{
    public interface IDialogueCommand
    {
        UniTask Execute();
    }
}
