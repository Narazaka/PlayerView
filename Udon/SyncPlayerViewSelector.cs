using JetBrains.Annotations;
using UdonSharp;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;

namespace UdonScripts
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SyncPlayerViewSelector : PlayerViewSelectorBase
    {
        [UdonSynced, FieldChangeCallback(nameof(targetPlayerId))] ushort _targetPlayerId;
        public override ushort targetPlayerId
        {
            get => _targetPlayerId;
            protected set
            {
                _targetPlayerId = value;
                SetActives();
            }
        }

        [PublicAPI]
        [NetworkCallable]
        public override void _SetTargetPlayerId(int playerId)
        {
            base._SetTargetPlayerId(playerId);
            if (Networking.IsOwner(gameObject))
            {
                RequestSerialization();
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(_SetTargetPlayerId), playerId);
            }
        }
    }
}
