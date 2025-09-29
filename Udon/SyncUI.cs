using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonScripts
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SyncUI : UdonSharpBehaviour
    {
        [SerializeField] SyncPlayerViewSelector syncPlayerViewSelector;
        [SerializeField] Toggle toggle;

        void OnEnable()
        {
            SetUI();
        }

        public void _OnChange()
        {
            syncPlayerViewSelector.sync = !syncPlayerViewSelector.sync;
            SetUI();
        }

        void SetUI()
        {
            toggle.SetIsOnWithoutNotify(syncPlayerViewSelector.sync);
        }
    }
}
