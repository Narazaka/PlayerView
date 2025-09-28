using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonScripts
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class InitialActivator : UdonSharpBehaviour
    {
        [SerializeField] GameObject[] objectsToActivate;

        private void Start()
        {
            foreach (var obj in objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }
    }
}
