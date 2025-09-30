using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace Narazaka.VRChat.PlayerView
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class NearClipPlaneUI : UdonSharpBehaviour
    {
        [SerializeField] SyncPlayerViewSelector syncPlayerViewSelector;
        [SerializeField] Slider slider;
        [SerializeField] TMP_InputField valueField;

        void OnEnable()
        {
            SetUI();
            syncPlayerViewSelector.AddListener((IUdonEventReceiver)this);
        }

        public void _OnChange()
        {
            var syncPlayerView = syncPlayerViewSelector.SyncPlayerView(Networking.LocalPlayer);
            if (syncPlayerView == null) return;
            syncPlayerView.nearClipPlane = slider.value;
            syncPlayerView.RequestSerialization();
            SetUI();
        }

        public void _OnValueChange()
        {
            if (float.TryParse(valueField.text, out var v))
            {
                v = Mathf.Clamp(v, slider.minValue, slider.maxValue);
                slider.SetValueWithoutNotify(v);
                _OnChange();
            }
            else
            {
                SetUI();
            }
        }

        public void _OnSelectedPlayerChanged()
        {
            SetUI();
        }

        void SetUI()
        {
            var localPlayer = Networking.LocalPlayer;
            slider.interactable = valueField.interactable = syncPlayerViewSelector.targetPlayerId == localPlayer.playerId;

            var syncPlayerView = syncPlayerViewSelector.SyncPlayerView(localPlayer);
            if (syncPlayerView == null) return;
            slider.SetValueWithoutNotify(syncPlayerView.nearClipPlane);
            valueField.SetTextWithoutNotify(syncPlayerView.nearClipPlane.ToString("F4"));
        }
    }
}
