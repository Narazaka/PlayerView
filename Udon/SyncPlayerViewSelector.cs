using JetBrains.Annotations;
using UdonSharp;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using UnityEngine;

namespace Narazaka.VRChat.PlayerView
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SyncPlayerViewSelector : PlayerViewSelectorBase
    {
        [SerializeField] bool _sync = true;

        [PublicAPI]
        public bool sync
        {
            get => _sync;
            set
            {
                _sync = value;
                if (value)
                {
                    targetPlayerId = syncTargetPlayerId;
                }
            }
        }

        [UdonSynced, FieldChangeCallback(nameof(syncTargetPlayerId))] ushort _syncTargetPlayerId;
        public ushort syncTargetPlayerId
        {
            get => _syncTargetPlayerId;
            protected set
            {
                _syncTargetPlayerId = value;
                if (sync)
                {
                    targetPlayerId = value;
                }
            }
        }

        [PublicAPI]
        public override void _SetTargetPlayerId(int playerId)
        {
            if (sync)
            {
                _SetTargetPlayerIdSync(playerId);
            }
            else
            {
                base._SetTargetPlayerId(playerId);
            }
        }

        [NetworkCallable]
        public void _SetTargetPlayerIdSync(int playerId)
        {
            syncTargetPlayerId = (ushort)playerId;
            if (Networking.IsOwner(gameObject))
            {
                RequestSerialization();
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(_SetTargetPlayerIdSync), playerId);
            }
        }
    }
}
