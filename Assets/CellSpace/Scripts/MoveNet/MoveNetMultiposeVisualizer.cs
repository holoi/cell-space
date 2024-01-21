namespace HoloInteractive.MoveNet {

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Linq;

    /// <summary>
    /// MoveNet multi-pose visualizer.
    /// This visualizer uses visualizes the pose keypoints overlaid on a UI panel.
    /// </summary>
    public sealed class MoveNetMultiposeVisualizer : MonoBehaviour {

        #region --Inspector--
        public Image bodyRect;
        public RectTransform keypointRect;

        [SerializeField] Transform keypointPrefab;
        [SerializeField] LineRenderer bonePrefab;
        #endregion


        #region --Client API--
        /// <summary>
        /// Render detected poses.
        /// </summary>
        /// <param name="poses">Body poses to render.</param>
        public void Render (params Pose[] poses) {
            // Delete current
            foreach (var rect in currentRects)
                GameObject.Destroy(rect.gameObject);
            foreach (var keypoint in currentKeypoints)
                GameObject.Destroy(keypoint.gameObject);
            foreach (var currentSkeleton in currentSkeletons)
                foreach (var point in currentSkeleton)
                    GameObject.Destroy(point.gameObject);
            currentRects.Clear();
            currentKeypoints.Clear();
            currentSkeletons.Clear();

            // Visualize
            foreach (var pose in poses) {
                var poseUI = Instantiate(bodyRect, transform);
                poseUI.gameObject.SetActive(true);

                VisualizeRect(pose, poseUI);
                currentRects.Add(poseUI);
                foreach (var point in pose) {
                    var keypointUI = Instantiate(keypointRect, transform);
                    keypointUI.gameObject.SetActive(true);
                    VisualizeAnchor(point, keypointUI);
                    currentKeypoints.Add(keypointUI);
                }

                //var currentSkeleton = new List<Transform>();
                //// Instantiate keypoints
                //for (var i = 5; i < 17; ++i) {
                //    var point = Instantiate(keypointPrefab, (Vector3) pose[i], Quaternion.identity, transform);
                //    point.gameObject.SetActive(true);
                //    currentSkeleton.Add(point);
                //}
                //currentSkeletons.Add(currentSkeleton);
                
                //foreach (var positions in new [] {
                //    new [] { pose.leftShoulder, pose.rightShoulder },
                //    new [] { pose.leftShoulder, pose.leftElbow, pose.leftWrist },
                //    new [] { pose.rightShoulder, pose.rightElbow, pose.rightWrist },
                //    new [] { pose.leftShoulder, pose.leftHip },
                //    new [] { pose.rightShoulder, pose.rightHip },
                //    new [] { pose.leftHip, pose.rightHip },
                //    new [] { pose.leftHip, pose.leftKnee, pose.leftAnkle },
                //    new [] { pose.rightHip, pose.rightKnee, pose.rightAnkle }
                //}) {
                //    var bone = Instantiate(bonePrefab, transform.position, Quaternion.identity, transform);
                //    bone.gameObject.SetActive(true);
                //    bone.positionCount = positions.Length;
                //    bone.SetPositions(positions.Select(v => (Vector3)v).ToArray());
                //    currentSkeleton.Add(bone.transform);
                //};
            }
        }
        #endregion


        #region --Operations--
        private readonly List<Image> currentRects = new List<Image>();
        private readonly List<RectTransform> currentKeypoints = new List<RectTransform>();
        readonly List<List<Transform>> currentSkeletons = new List<List<Transform>>();

        private void VisualizeRect (Pose pose, Image prefab) {
            var rectTransform = prefab.transform as RectTransform;
            var imageTransform = transform as RectTransform;
            rectTransform.anchorMin = 0.5f * Vector2.one;
            rectTransform.anchorMax = 0.5f * Vector2.one;
            rectTransform.pivot = Vector2.zero;
            rectTransform.sizeDelta = Vector2.Scale(imageTransform.rect.size, pose.rect.size);
            rectTransform.anchoredPosition = Rect.NormalizedToPoint(imageTransform.rect, pose.rect.position);
        }

        private void VisualizeAnchor (Vector2 point, RectTransform anchor) {
            var imageTransform = transform as RectTransform;
            anchor.anchorMin = 0.5f * Vector2.one;
            anchor.anchorMax = 0.5f * Vector2.one;
            anchor.pivot = 0.5f * Vector2.one;
            anchor.anchoredPosition = Rect.NormalizedToPoint(imageTransform.rect, point);
        }
        #endregion
    }
}