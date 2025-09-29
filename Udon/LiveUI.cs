using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.PlayerView
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LiveUI : UdonSharpBehaviour
    {
        [SerializeField] SyncPlayerViewSelector syncPlayerViewSelector;
        [SerializeField] Toggle toggle;

        void OnEnable()
        {
            SetUI();
        }

        public void _OnChange()
        {
            syncPlayerViewSelector.live = !syncPlayerViewSelector.live;
            SetUI();
        }

        void SetUI()
        {
            toggle.SetIsOnWithoutNotify(syncPlayerViewSelector.live);
        }
    }
}
