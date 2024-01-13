/*
 *   MoveNet Multipose
 *   Copyright © 2023 NatML Inc. All Rights Reserved.
 */

using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace NatML.Examples {

    using UnityEngine;
    using VideoKit;
    using Vision;
    using Visualizers;
    using Bibcam.Decoder;
    using NatML.Features;
    using UnityEngine.Rendering;

    [MLEdgeModel.Embed("@natml/movenet-multipose")]
    public class MoveNetMultiposeSample : MonoBehaviour {

    [SerializeField] BibcamMetadataDecoder _decoder = null;
    [SerializeField] BibcamTextureDemuxer _demux = null;
    public MoveNetMultiposeVisualizer visualizer;

    private MoveNetMultiposePredictor predictor;


    private async void Start() {
        // Create the MoveNet Multipose predictor
        predictor = await MoveNetMultiposePredictor.Create();
        // Listen for camera frames
    }

    void LateUpdate()
    {
        // Run it only when the textures are ready.
        if (_demux?.ColorTexture == null) return;

        // Camera parameters
        // var meta = _decoder.Metadata;
        // var ray = BibcamRenderUtils.RayParams(meta);
        // var iview = BibcamRenderUtils.InverseView(meta);


        var rt1 = _demux.ColorTexture;
        var rt2 = _demux.DepthTexture;

        var t1 = new Texture2D(rt1.width, rt1.height, TextureFormat.RGB24, false);
        var old_rt = RenderTexture.active;
        RenderTexture.active = rt1;
        t1.ReadPixels(new Rect(0, 0, rt1.width, rt1.height), 0, 0);
        t1.Apply();
        RenderTexture.active = old_rt;

        // t2.ReadPixels(new Rect(0, 0, rt2.width, rt2.height), 0, 0);
        // t2.Apply();

       var image = new MLImageFeature(t1);
//       var depth = new MLDepthFeature(t2);

       // var poses = predictor.Predict(image);

     //   visualizer.Render(poses);

        // AsyncGPUReadback.Request(_demux.ColorTexture, 0, 
        //      request => {
        //     // Create an image feature
        // //   var feature = new MLImageFeature(request.GetData<byte>(), request.width, request.height);
        //     // Predict
        //  //  var poses = predictor.Predict(feature);
        //   // visualizer.Render(poses);
        // });


     }

    private void OnDisable () {
        // Stop listening for camera frames
        // Dispose the predictor
        predictor?.Dispose();
    }
    }
}