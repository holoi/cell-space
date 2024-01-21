/*
*   MoveNet 3D
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace HoloInteractive.MoveNet {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    /// <summary>
    /// Detected 3D body pose.
    /// The xyz coordinates are the world position of the keypoint.
    /// The w coordinate is the confidence score of the keypoint, in range [0, 1].
    /// </summary>
    public readonly struct Pose3D : IReadOnlyList<Vector4> {

        #region --Client API--
        /// <summary>
        /// Number of keypoints in the pose.
        /// </summary>
        public readonly int Count               => keypoints.Length;

        /// <summary>
        /// Nose position.
        /// </summary>
        public readonly Vector4 nose            => this[0];
        
        /// <summary>
        /// Left eye position.
        /// </summary>'
        public readonly Vector4 leftEye         => this[1];

        /// <summary>
        /// Right eye position.
        /// </summary>
        public readonly Vector4 rightEye        => this[2];

        /// <summary>
        /// Left ear position.
        /// </summary>
        public readonly Vector4 leftEar         => this[3];

        /// <summary>
        /// Right ear position.
        /// </summary>
        public readonly Vector4 rightEar        => this[4];

        /// <summary>
        /// Left shoulder position.
        /// </summary>
        public readonly Vector4 leftShoulder    => this[5];

        /// <summary>
        /// Right shoulder position.
        /// </summary>
        public readonly Vector4 rightShoulder   => this[6];

        /// <summary>
        /// Left elbow position.
        /// </summary>
        public readonly Vector4 leftElbow       => this[7];

        /// <summary>
        /// Right elbow position.
        /// </summary>
        public readonly Vector4 rightElbow      => this[8];

        /// <summary>
        /// Left wrist position.
        /// </summary>
        public readonly Vector4 leftWrist       => this[9];

        /// <summary>
        /// Right wrist position.
        /// </summary>
        public readonly Vector4 rightWrist      => this[10];

        /// <summary>
        /// Left hip position.
        /// </summary>
        public readonly Vector4 leftHip         => this[11];

        /// <summary>
        /// Right hip position.
        /// </summary>
        public readonly Vector4 rightHip        => this[12];

        /// <summary>
        /// Left knee position.
        /// </summary>
        public readonly Vector4 leftKnee        => this[13];

        /// <summary>
        /// Right knee position.
        /// </summary>
        public readonly Vector4 rightKnee       => this[14];

        /// <summary>
        /// Left ankle position.
        /// </summary>
        public readonly Vector4 leftAnkle       => this[15];

        /// <summary>
        /// Right ankle position.
        /// </summary>
        public readonly Vector4 rightAnkle      => this[16];

        /// <summary>
        /// Get a pose anchor by index.
        /// </summary>
        /// <param name="idx">Keypoint index. Must be in range [0, 16].</param>
        public readonly Vector4 this [int idx]  => keypoints[idx];
        #endregion


        #region --Operations--
        private readonly Vector4[] keypoints;

        internal Pose3D(Pose pose) { 
            // Compute scale factor
            var xScale = (float) imageWidth / depthMap.width;
            var yScale = (float) imageHeight / depthMap.height;
            var scale = Mathf.Max(xScale, yScale); // Image is always aspect filled in screen
            var xRatio = scale * depthMap.width / imageWidth;
            var yRatio = scale * depthMap.height / imageHeight;
            // Transform
            keypoints = new Vector4[pose.Count];
            for (var i = 0; i < pose.Count; ++i) {
                var keypoint = pose[i];
                var scaledKeypoint = new Vector2(xRatio * (keypoint.x - 0.5f) + 0.5f, yRatio * (keypoint.y - 0.5f) + 0.5f);
                var worldPoint = Unproject(depthMap, scaledKeypoint);
                keypoints[i] = new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, keypoint.z);
            }
        }
        public float Sample (Vector2 point) {
            var s = float2(image.width, image.height); // use unoriented size
            var uv = float2(point.x, point.y);
            var t = Mathf.Deg2Rad * rotation;
            var T = mul(float2x2(cos(t), -sin(t), sin(t), cos(t)), float2x2(1f, 0f, 0f, -1f));
            var uv_r = mul(T, uv - 0.5f) + 0.5f;
            var xy = int2(uv_r * s);
            if (xy.x < 0 || xy.x >= image.width || xy.y < 0 || xy.y >= image.height)
                return -1;
            switch (image.format) {
                case Format.DepthFloat32:   return Sample<float>(xy.x, xy.y);
                case Format.DepthUint16:    return 0.001f * Sample<ushort>(xy.x, xy.y);
                default:                    throw new InvalidOperationException($"Cannot sample depth because image has invalid format: {image.format}");
            }
        }

        public Vector3 Unproject (Vector2 point) { 
            var depth = Sample(point);
            var viewport = new Vector3(point.x, point.y, depth);
            var world = camera.ViewportToWorldPoint(viewport);
            return world;
        }
        #endregion
        private unsafe T Sample<T> (Texture2D texture, int x, int y) where T : unmanaged {
            var plane = image.GetPlane(0);
            var idx = y * plane.rowStride + x * plane.pixelStride;
            var data = (byte*)plane.data.GetUnsafeReadOnlyPtr();
            var sample = *(T*)&data[idx];
            return sample;
        }


        IEnumerator<Vector4> IEnumerable<Vector4>.GetEnumerator () {
            for (var i = 0; i < Count; ++i)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<Vector4>).GetEnumerator();
        #endregion
    }
}