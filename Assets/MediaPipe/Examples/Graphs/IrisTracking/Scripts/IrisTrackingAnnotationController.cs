using System.Collections.Generic;
using UnityEngine;

namespace Mediapipe {
  public class IrisTrackingAnnotationController : AnnotationController {
    [SerializeField] GameObject irisPrefab = null;
    [SerializeField] GameObject faceLandmarkListPrefab = null;
    [SerializeField] GameObject faceRectPrefab = null;
    [SerializeField] GameObject faceDetectionsPrefab = null;

        /* ------------------- 追加 ------------------- */
        private MeshFilter meshFilter; // オブジェクトのMeshFilter
        private Mesh faceMesh; // オブジェクトのMesh
        private List<Vector3> vertextList = new List<Vector3>(); // Meshの頂点の座標リスト
        /* ------------------------------------------- */

        private GameObject leftIrisAnnotation;
    private GameObject rightIrisAnnotation;
    private GameObject faceLandmarkListAnnotation;
    private GameObject faceRectAnnotation;
    private GameObject faceDetectionsAnnotation;

    enum Side {
      Left = 1,
      Right = 2,
    }

    void Awake() {
      leftIrisAnnotation = Instantiate(irisPrefab);
      rightIrisAnnotation = Instantiate(irisPrefab);
      faceLandmarkListAnnotation = Instantiate(faceLandmarkListPrefab);
      faceRectAnnotation = Instantiate(faceRectPrefab);
      faceDetectionsAnnotation = Instantiate(faceDetectionsPrefab);

        /* ------------------- 追加 ------------------- */
        meshFilter = GameObject.Find("default").GetComponent<MeshFilter>(); // defaultオブジェクトからMeshFilterを取得
        faceMesh = meshFilter.mesh; // Meshをセット
        vertextList.AddRange(faceMesh.vertices); // Meshから頂点座標リストを取得
        /* ------------------------------------------- */
    }

        void OnDestroy() {
      Destroy(leftIrisAnnotation);
      Destroy(rightIrisAnnotation);
      Destroy(faceLandmarkListAnnotation);
      Destroy(faceRectAnnotation);
      Destroy(faceDetectionsAnnotation);
    }

    public override void Clear() {
      leftIrisAnnotation.GetComponent<IrisAnnotationController>().Clear();
      rightIrisAnnotation.GetComponent<IrisAnnotationController>().Clear();
      faceLandmarkListAnnotation.GetComponent<FaceLandmarkListAnnotationController>().Clear();
      faceRectAnnotation.GetComponent<RectAnnotationController>().Clear();
      faceDetectionsAnnotation.GetComponent<DetectionListAnnotationController>().Clear();
    }

    public void Draw(Transform screenTransform, NormalizedLandmarkList landmarkList,
        NormalizedRect faceRect, List<Detection> faceDetections, bool isFlipped = false)
    {
      if (landmarkList == null) {
        Clear();
        return;
      }

      UpdateFaceMesh(landmarkList); // 追加

      var leftIrisLandmarks = GetIrisLandmarks(landmarkList, Side.Left);
      leftIrisAnnotation.GetComponent<IrisAnnotationController>().Draw(screenTransform, leftIrisLandmarks, isFlipped);

      var rightIrisLandmarks = GetIrisLandmarks(landmarkList, Side.Right);
      rightIrisAnnotation.GetComponent<IrisAnnotationController>().Draw(screenTransform, rightIrisLandmarks, isFlipped);

      faceLandmarkListAnnotation.GetComponent<FaceLandmarkListAnnotationController>().Draw(screenTransform, landmarkList, isFlipped);
      faceRectAnnotation.GetComponent<RectAnnotationController>().Draw(screenTransform, faceRect, isFlipped);
      faceDetectionsAnnotation.GetComponent<DetectionListAnnotationController>().Draw(screenTransform, faceDetections, isFlipped);
    }

    /* ------------------- 追加 ------------------- */
    private int meshScale = -5; // サイズ調整用の変数
    private void UpdateFaceMesh(NormalizedLandmarkList landmarkList)
    {
        // 顔の頂点分だけ実行（478 - 10 = 468）
        for (var i = 0; i < landmarkList.Landmark.Count - 10; i++)
        {
            var landmark = landmarkList.Landmark[i];
            // 検出したLandmarkをMeshの頂点座標にセット
            vertextList[i] = new Vector3(meshScale * landmark.X, meshScale * landmark.Y, meshScale * landmark.Z);
        }
        // 座標リストをMeshに適用
        faceMesh.SetVertices(vertextList);
    }
    /* ------------------------------------------- */

      private IList<NormalizedLandmark> GetIrisLandmarks(NormalizedLandmarkList landmarkList, Side side) {
      var irisLandmarks = new List<NormalizedLandmark>(5);
      var offset = 468 + (side == Side.Left ? 0 : 5);

      for (var i = 0; i < 5; i++) {
        irisLandmarks.Add(landmarkList.Landmark[offset + i]);
      }

      return irisLandmarks;
    }
  }
}
