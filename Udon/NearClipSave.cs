using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.PlayerView
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class NearClipSave : UdonSharpBehaviour
    {
        [SerializeField] int maxEntries = 20;
        [UdonSynced] float[] nearClipPlanes = new float[0];

        public float Load()
        {
            Debug.Log($"NearClipSave: Load");
            var avatarId = AvatarId();
            return Get(FindIndex(avatarId));
        }

        public void Save(float value)
        {
            Debug.Log($"NearClipSave: Save");
            var avatarId = AvatarId();
            var index = FindIndex(avatarId);
            if (index < 0)
            {
                var len = nearClipPlanes.Length;
                if (len >= maxEntries * 2)
                {
                    var newNearClipPlanes = new float[(maxEntries - 1) * 2];
                    System.Array.Copy(nearClipPlanes, 2, newNearClipPlanes, 0, newNearClipPlanes.Length);
                    nearClipPlanes = newNearClipPlanes;
                    len = nearClipPlanes.Length;
                }
                var newLen = len + 2;
                var newArray = new float[newLen];
                System.Array.Copy(nearClipPlanes, newArray, len);
                nearClipPlanes = newArray;
                index = len;
                nearClipPlanes[index] = avatarId;
            }
            Set(index, value);
            RequestSerialization();
        }

        float AvatarId()
        {
            var player = Networking.LocalPlayer;
            return player.GetAvatarEyeHeightAsMeters();
        }

        int FindIndex(float avatarId)
        {
            var len = nearClipPlanes.Length;
            for (var i = 0; i < len; i += 2)
            {
                Debug.Log($"NearClipSave: {i}: {nearClipPlanes[i]} == {avatarId} {nearClipPlanes[i] == avatarId} {Mathf.Approximately(nearClipPlanes[i], avatarId)} {Mathf.Abs(nearClipPlanes[i] - avatarId)}");
                if (Mathf.Approximately(nearClipPlanes[i], avatarId))
                {
                    return i;
                }
            }
            return -1;
        }

        float Get(int index)
        {
            if (index >= 0)
            {
                return nearClipPlanes[index + 1];
            }
            return 0;
        }

        void Set(int index, float value)
        {
            if (index >= 0)
            {
                nearClipPlanes[index + 1] = value;
            }
        }
    }
}
