using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Narazaka.VRChat.PlayerView
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SyncPlayerView : UdonSharpBehaviour
    {
        [SerializeField] Transform followRoot;
        [SerializeField] Transform scaler;
        [SerializeField] Transform audioListener;
        [SerializeField] PlayerViewSelectorBase playerViewSelector;
        [SerializeField] NearClipSave nearClipSave;
        [SerializeField] bool resetCullingMatrix;
        [SerializeField, UdonSynced, FieldChangeCallback(nameof(nearClipPlane))] float _nearClipPlane = 0.12f;
        public float nearClipPlane
        {
            get => _nearClipPlane;
            set
            {
                if (_nearClipPlane == value) return;
                _nearClipPlane = value;
                var cam = targetCamera;
                if (cam != null)
                {
                    cam.nearClipPlane = value;
                }
                nearClipSave.Save(value);
            }
        }
        [UdonSynced] Vector3 positionDiff = Vector3.zero;
        [UdonSynced] Quaternion rotationDiff = Quaternion.identity;
        [UdonSynced] float scale = 1f;

        bool syncInitialized = false;

        Camera _targetCamera;
        Camera targetCamera
        {
            get
            {
                if (_targetCamera == null)
                {
                    _targetCamera = GetComponent<Camera>();
                }
                return _targetCamera;
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            syncInitialized = true;
            LoadNearClipPlane();
            CalcBoneDiff(player);
        }

        public override void OnAvatarChanged(VRCPlayerApi player)
        {
            LoadNearClipPlane();
            CalcBoneDiff(player);
        }

        public override void OnAvatarEyeHeightChanged(VRCPlayerApi player, float prevEyeHeightAsMeters)
        {
            LoadNearClipPlane();
            CalcBoneDiff(player);
        }

        void OnEnable()
        {
            LoadNearClipPlane();
            CalcBoneDiff(Networking.GetOwner(gameObject));
            targetCamera.enabled = true;
        }

        // cf. https://qiita.com/yuri_tsukimi/items/5cc5b4dbd79df1ec8503
        // cf. https://tsubokulab.fanbox.cc/posts/2745798
        void CalcBoneDiff(VRCPlayerApi player)
        {
            if (!syncInitialized || !Networking.IsOwner(gameObject) || !player.isLocal) return;

            var trackingData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            positionDiff = trackingData.position - player.GetBonePosition(HumanBodyBones.Head);
            rotationDiff = Quaternion.Inverse(player.GetBoneRotation(HumanBodyBones.Head)) * trackingData.rotation;

            scale = 1f / audioListener.transform.localScale.x;

            RequestSerialization();
        }

        void LoadNearClipPlane()
        {
            var val = nearClipSave.Load();
            if (val > 0)
            {
                nearClipPlane = val;
            }
            playerViewSelector._OnNearClipChanged();
        }

        void OnPreCull()
        {
            var player = Networking.GetOwner(gameObject);
            var position = player.GetBonePosition(HumanBodyBones.Head) + positionDiff;
            var rotation = player.GetBoneRotation(HumanBodyBones.Head) * rotationDiff;
            followRoot.SetPositionAndRotation(position, rotation);
            scaler.localScale = Vector3.one * scale;

            if (resetCullingMatrix)
            {
                var cam = targetCamera;
                cam.ResetWorldToCameraMatrix();
                cam.ResetCullingMatrix();
            }
        }
    }
}
