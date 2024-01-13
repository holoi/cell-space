using UnityEngine;
using Unity.Sentis;
using Bibcam.Decoder;

public sealed class MoveNet3DSample : MonoBehaviour {
    [SerializeField] BibcamMetadataDecoder _decoder = null;
    [SerializeField] BibcamTextureDemuxer _demux = null;

    [SerializeField] private ModelAsset modelAsset;
    [SerializeField] private RenderTexture outputTexture;

    int _imageWidth;
    int _imageHeight;

    private Model runtimeModel;
    private IWorker worker;
    public MoveNet3DVisualizer visualizer;
    private Ops ops;

    void Start () {
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, runtimeModel);
        ops = WorkerFactory.CreateOps(backendType, null);
    }

    void Update () {
        var inputTensor = TextureConverter.ToTensor(_demux.ColorTexture, _imageWidth, _imageHeight, 3);
        Inference(inputTensor);
        inputTensor.Dispose();
    }

    void OnDestroy()
    {
        worker.Dispose();
    }

    private void Inference(Tensor input)
    {
        worker.Execute(input);

        TensorFloat result = engine.PeekOutput() as TensorFloat;

        if (result == null) { return; /* ガード */ }
        
        result.MakeReadable();

        Debug.Log(result);

        // for (var i = 5; i < 17; ++i) {
        //     pose[i]
        //     var point = Instantiate(keypointPrefab, (Vector3)pose[i], Quaternion.identity, transform);
        //     point.gameObject.SetActive(true);
        //     currentSkeleton.Add(point);
        // }

        result.Dispose(); 
    }

    void OnDisable () {
        // Dispose the predictor
        predictor?.Dispose();
    }
}