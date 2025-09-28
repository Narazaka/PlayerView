using UdonSharp;

namespace UdonScripts
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LocalPlayerViewSelector : PlayerViewSelectorBase
    {
        ushort _targetPlayerId;

        public override ushort targetPlayerId
        {
            get => _targetPlayerId;
            protected set
            {
                _targetPlayerId = value;
                SetActives();
            }
        }
    }
}
