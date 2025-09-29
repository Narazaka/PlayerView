using JetBrains.Annotations;
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace Narazaka.VRChat.PlayerView
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
        // Narazaka.VRChat.PlayerSelectUI.PlayerSelectReceiver
        [NonSerialized]
        public IUdonEventReceiver _selectChangeReceiver;
        // Narazaka.VRChat.PlayerSelectUI.PlayerSelectReceiver
        public void _AddSelectChangeListener() => AddListener(_selectChangeReceiver);

        ushort _targetPlayerId;

        public ushort targetPlayerId
        {
            get => _targetPlayerId;
            protected set
            {
                _targetPlayerId = value;
                SetActives();
                _selectedPlayer = targetPlayer;
                NotifyListeners("_OnSelectedPlayerChanged");
            }
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
                var c = SyncPlayerView(p);
                c.gameObject.SetActive(live && p.playerId == targetPlayerId);
            }
        }

        public SyncPlayerView SyncPlayerView(VRCPlayerApi player)
        {
            return (SyncPlayerView)Networking.FindComponentInPlayerObjects(player, followerSource);
        }

        IUdonEventReceiver[] listeners = new IUdonEventReceiver[0];

        public void AddListener(IUdonEventReceiver receiver)
        {
            if (Array.IndexOf(listeners, receiver) >= 0) return;
            var newListeners = new IUdonEventReceiver[listeners.Length + 1];
            Array.Copy(listeners, newListeners, listeners.Length);
            newListeners[newListeners.Length - 1] = receiver;
            listeners = newListeners;
        }

        void NotifyListeners(string eventName)
        {
            foreach (var l in listeners)
            {
                l.SendCustomEvent(eventName);
            }
        }
    }
}
