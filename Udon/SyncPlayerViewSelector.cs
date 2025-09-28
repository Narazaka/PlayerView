using JetBrains.Annotations;
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonScripts
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SyncPlayerViewSelector : UdonSharpBehaviour
    {
        [SerializeField] SyncPlayerView followerSource;

        [UdonSynced, FieldChangeCallback(nameof(targetPlayerId))] ushort _targetPlayerId;

        // Narazaka.VRChat.PlayerSelectUI.PlayerSelectReceiver
        [NonSerialized]
        public VRCPlayerApi _selectedPlayer;
        // Narazaka.VRChat.PlayerSelectUI.PlayerSelectReceiver
        public void _OnSelectPlayer() => _SetTargetPlayerSync(_selectedPlayer);

        public ushort targetPlayerId
        {
            get => _targetPlayerId;
            private set
            {
                _targetPlayerId = value;
                SetActives();
            }
        }

        public VRCPlayerApi targetPlayer
        {
            get => targetPlayerId == 0 ? null : VRCPlayerApi.GetPlayerById(targetPlayerId);
            private set => targetPlayerId = (ushort)(value != null ? value.playerId : 0);
        }

        [PublicAPI]
        public void _SetTargetPlayerSync(VRCPlayerApi player)
        {
            _SetTargetPlayerIdSync(Utilities.IsValid(player) ? player.playerId : 0);
        }

        [PublicAPI]
        [NetworkCallable]
        public void _SetTargetPlayerIdSync(int playerId)
        {
            targetPlayerId = (ushort)playerId;
            if (Networking.IsOwner(gameObject))
            {
                RequestSerialization();
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(_SetTargetPlayerIdSync), playerId);
            }
        }

        void SetActives()
        {
            if (!Utilities.IsValid(targetPlayer))
            {
                return;
            }
            var players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(players);
            foreach (var p in players)
            {
                var c = Networking.FindComponentInPlayerObjects(p, followerSource);
                c.gameObject.SetActive(p.playerId == targetPlayerId);
            }
        }
    }
}
