using UnityEngine;
using Unity.Sentis;
using Bibcam.Decoder;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace RealityDesignLab.MoveNet
{
    public sealed class MoveNetMultipose3D : MonoBehaviour {
        [SerializeField] private BibcamMetadataDecoder _decoder = null;
        [SerializeField] private BibcamTextureDemuxer _demux = null;

        [SerializeField] private Camera _camera;
        [SerializeField] private MoveNetMultiposeVisualizer _visualizer = null;

        [SerializeField] private ModelAsset _modelAsset;
        [SerializeField] private float _minScore;
        private RenderTexture _image;
        private RenderTexture _depth;

        int _imageWidth;
        int _imageHeight;
        int _depthWidth;
        int _depthHeight;

        public const int MODEL_IMAGE_SIZE = 256; 

        private OneEuroFilter _filter;
        private Model _runtimeModel;
        private IWorker _worker;
        private Ops _ops;
        private TextureTransform _textureTransform;

        void Start () {
            _runtimeModel = ModelLoader.Load(_modelAsset);
            _worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, _runtimeModel);
            _ops = WorkerFactory.CreateOps(BackendType.GPUCompute, new TensorCachingAllocator());
            _filter = new OneEuroFilter(0.5f, 3f, 1f);
            _textureTransform = new TextureTransform().SetDimensions(MODEL_IMAGE_SIZE, MODEL_IMAGE_SIZE, 3).SetTensorLayout(TensorLayout.NHWC);
        }

        void Update () {

            if (_demux.ColorTexture == null || _demux.DepthTexture == null)
                return;

            _imageWidth = _demux.ColorTexture.width;
            _imageHeight = _demux.ColorTexture.height;
            _depthWidth = _demux.DepthTexture.width;
            _depthHeight = _demux.DepthTexture.height;

            _image = _demux.ColorTexture;
            _depth = _demux.DepthTexture;

            var inputTensor = TextureConverter.ToTensor(_image, _textureTransform);
            var tensorScaled = _ops.Mul(inputTensor, 255f);
            Inference(tensorScaled);
            inputTensor.Dispose();
            tensorScaled.Dispose();
        }

        void OnDestroy()
        {
            _worker.Dispose();
        }

        // Pose3d Unproject(Pose pose) {
        //     var xScale = (float) imageWidth / depthWidth;
        //     var yScale = (float) imageHeight / depthHeight;
        //     var scale = Mathf.Max(xScale, yScale); // Image is always aspect filled in screen
        //     var xRatio = scale * depthWidth / imageWidth;
        //     var yRatio = scale * depthHeight / imageHeight;
        //     // Transform
        //     var keypoints = new Vector4[pose.Count];
        //     for (var i = 0; i < pose.Count; ++i) {
        //         var keypoint = pose[i];
        //         var scaledKeypoint = new Vector2(xRatio * (keypoint.x - 0.5f) + 0.5f, yRatio * (keypoint.y - 0.5f) + 0.5f);
        //         var worldPoint = Unproject(depthMap, scaledKeypoint);
        //         keypoints[i] = new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, keypoint.z);
        //     }
        // }

        // private unsafe T Sample<T> (int x, int y) where T : unmanaged {
        //     var plane = _depth.GetPlane(0);
        //     var idx = y * plane.rowStride + x * plane.pixelStride;
        //     var data = (byte*)plane.data.GetUnsafeReadOnlyPtr();
        //     var sample = *(T*)&data[idx];
        //     return sample;
        // }
        // public float Sample(Vector2 point) {
        //     var s = float2(_depth.width, _depth.height); // use unoriented size
        //     var uv = float2(point.x, point.y);
        //     var t = Mathf.Deg2Rad * rotation;
        //     var T = mul(float2x2(cos(t), -sin(t), sin(t), cos(t)), float2x2(1f, 0f, 0f, -1f));
        //     var uv_r = mul(T, uv - 0.5f) + 0.5f;
        //     var xy = int2(uv_r * s);
        //     if (xy.x < 0 || xy.x >= image.width || xy.y < 0 || xy.y >= image.height)
        //         return -1;
        //     switch (image.format) {
        //         case Format.DepthFloat32:   return Sample<float>(xy.x, xy.y);
        //         case Format.DepthUint16:    return 0.001f * Sample<ushort>(xy.x, xy.y);
        //         default:                    throw new InvalidOperationException($"Cannot sample depth because image has invalid format: {image.format}");
        //     }
        // }

        // Vector3 Unproject(Vector2 point) { 
        //     var depth = Sample(_depth, point);
        //     var viewport = new Vector3(point.x, point.y, depth);
        //     var world = _camera.ViewportToWorldPoint(viewport);
        //     return world;
        // }

        //         internal Pose3D(Pose pose) { 
        //     // Compute scale factor
        //     var xScale = (float) imageWidth / depthMap.width;
        //     var yScale = (float) imageHeight / depthMap.height;
        //     var scale = Mathf.Max(xScale, yScale); // Image is always aspect filled in screen
        //     var xRatio = scale * depthMap.width / imageWidth;
        //     var yRatio = scale * depthMap.height / imageHeight;
        //     // Transform
        //     keypoints = new Vector4[pose.Count];
        //     for (var i = 0; i < pose.Count; ++i) {
        //         var keypoint = pose[i];
        //         var scaledKeypoint = new Vector2(xRatio * (keypoint.x - 0.5f) + 0.5f, yRatio * (keypoint.y - 0.5f) + 0.5f);
        //         var worldPoint = Unproject(depthMap, scaledKeypoint);
        //         keypoints[i] = new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, keypoint.z);
        //     }
        // }
        // public float Sample (Vector2 point) {
        //     var s = float2(image.width, image.height); // use unoriented size
        //     var uv = float2(point.x, point.y);
        //     var t = Mathf.Deg2Rad * rotation;
        //     var T = mul(float2x2(cos(t), -sin(t), sin(t), cos(t)), float2x2(1f, 0f, 0f, -1f));
        //     var uv_r = mul(T, uv - 0.5f) + 0.5f;
        //     var xy = int2(uv_r * s);
        //     if (xy.x < 0 || xy.x >= image.width || xy.y < 0 || xy.y >= image.height)
        //         return -1;
        //     switch (image.format) {
        //         case Format.DepthFloat32:   return Sample<float>(xy.x, xy.y);
        //         case Format.DepthUint16:    return 0.001f * Sample<ushort>(xy.x, xy.y);
        //         default:                    throw new InvalidOperationException($"Cannot sample depth because image has invalid format: {image.format}");
        //     }
        // }

        // public Vector3 Unproject (Vector2 point) { 
        //     var depth = Sample(point);
        //     var viewport = new Vector3(point.x, point.y, depth);
        //     var world = camera.ViewportToWorldPoint(viewport);
        //     return world;
        // }
        // #endregion
        // private unsafe T Sample<T> (Texture2D texture, int x, int y) where T : unmanaged {
        //     var plane = image.GetPlane(0);
        //     var idx = y * plane.rowStride + x * plane.pixelStride;
        //     var data = (byte*)plane.data.GetUnsafeReadOnlyPtr();
        //     var sample = *(T*)&data[idx];
        //     return sample;
        // }



        private void Inference(Tensor input)
        {
            _worker.Execute(input);

            var keypoints = _worker.PeekOutput() as TensorFloat;
            
            if (keypoints == null) { return; }

            keypoints.MakeReadable();

            var keypointData = keypoints.ToReadOnlyArray();

           // keypointData = _filter?.Filter(keypointData) ?? keypointData;

//            Create poses
            var result = new List<Pose>();
            // var result3d = new List<Pose3D>();

            for (int i = 0, ilen = keypoints.shape[1], istride = keypoints.shape[2]; i < ilen; ++i)
            {
                var offset = i * istride;
                var pose = new Pose(keypointData, offset);
                if (pose.score >= _minScore)
                {
                    result.Add(pose);
                    // result3d.Add(new Pose3D(pose));
                }
            }

            keypoints.Dispose();

            _visualizer.Render(result.ToArray());
        }
    }
}