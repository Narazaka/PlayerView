using JetBrains.Annotations;
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonScripts
{
    public abstract class PlayerViewSelectorBase : UdonSharpBehaviour
    {
        [SerializeField] SyncPlayerView followerSource;

        [SerializeField] bool _live = true;

        [PublicAPI]
        public bool live
        {
            get => _live;
            set
            {
                _live = value;
                SetActives();
            }
        }

        [PublicAPI] public void _ToggleLive() => live = !live;
        [PublicAPI] public void _SetLiveOn() => live = true;
        [PublicAPI] public void _SetLiveOff() => live = false;

        // Narazaka.VRChat.PlayerSelectUI.PlayerSelectReceiver
        [NonSerialized]
        public VRCPlayerApi _selectedPlayer;
        // Narazaka.VRChat.PlayerSelectUI.PlayerSelectReceiver
        public void _OnSelectPlayer() => _SetTargetPlayer(_selectedPlayer);

        public abstract ushort targetPlayerId
        {
            get;
            protected set;
        }

        public VRCPlayerApi targetPlayer
        {
            get => targetPlayerId == 0 ? null : VRCPlayerApi.GetPlayerById(targetPlayerId);
            private set => targetPlayerId = (ushort)(value != null ? value.playerId : 0);
        }

        [PublicAPI]
        public void _SetTargetPlayer(VRCPlayerApi player)
        {
            _SetTargetPlayerId(Utilities.IsValid(player) ? player.playerId : 0);
        }

        [PublicAPI]
        public virtual void _SetTargetPlayerId(int playerId)
        {
            targetPlayerId = (ushort)playerId;
        }

        protected void SetActives()
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
                c.gameObject.SetActive(live && p.playerId == targetPlayerId);
            }
        }
    }
}
