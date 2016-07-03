﻿using UnityEngine;
using System.IO;

public class Record : MonoBehaviour {
    public GameObject textRendererPrefab;
    public Transform meshTransform;
    public MeshRenderer meshRenderer;
    // This info will only be used in debug situations (actual infos are filled out at runtime)
    public RecordInfo info = new RecordInfo("The Recorders", "A Record", new byte[0], new Song[] { new Song("The Only Song") });
    public float spinTime = 0.25f;
    public float riseTime = 1f;
    public float lowerTime = 1f;
    public float riseHeight = 0.25f;

    private float meshHeight = 0;
    private float targetMeshHeight = 0;
    private float meshSpin = 0;
    private float targetMeshSpin = 0;

    void Start () {
        Texture2D frontTexture = LoadTexture();
        if (frontTexture == null) {
            Color bgColor = Random.ColorHSV();
            Color textColor = Util.GetOverlayColor(bgColor);
            frontTexture = TextToTextureRenderer.RenderText(textRendererPrefab, info.artist + "\n" + info.name, bgColor, textColor);
        }

        string songs = "<i>" + info.name + (info.name != null ? "</i> by <i>" + info.artist + ":\n" : "") + "</i>";
        for (int i = 0; i < info.songs.Length; i++) {
            songs += "• " + info.songs[i].name + "\n";
        }
        Color backColor = Util.GetAverageColorFromTexture(frontTexture);
        Texture2D backTexture = TextToTextureRenderer.RenderText(textRendererPrefab, songs, backColor, Util.GetOverlayColor(backColor));

        // Material indices: 0 - back, 1 - front (cover)
        meshRenderer.materials[0].mainTexture = backTexture;
        meshRenderer.materials[1].mainTexture = frontTexture;
    }

    void Update () {
        if (meshHeight < targetMeshHeight) {
            meshHeight += riseHeight * Time.deltaTime / riseTime;
            if (meshHeight > targetMeshHeight) {
                meshHeight = targetMeshHeight;
            }
        }
        if (meshHeight > targetMeshHeight) {
            meshHeight -= riseHeight * Time.deltaTime / lowerTime;
            if (meshHeight < targetMeshHeight) {
                meshHeight = targetMeshHeight;
            }
        }
        if (meshSpin != targetMeshSpin) {
            float delta = targetMeshSpin - meshSpin;
            float sign = Mathf.Sign(delta);
            meshSpin += delta * Time.deltaTime / spinTime;
            if (targetMeshSpin - meshSpin < 0.5 || sign != Mathf.Sign(targetMeshSpin - meshSpin)) {
                meshSpin = targetMeshSpin;
            }
        }
        meshTransform.localPosition = new Vector3(0, meshHeight, 0);
        meshTransform.localEulerAngles = new Vector3(0, meshSpin, 0);
    }

    private Texture2D LoadTexture () {
        if (info.imageData.Length > 0) {
            Texture2D result = new Texture2D(1, 1);
            result.LoadImage(info.imageData);
            return result;
        } else {
            return null;
        }
    }

    public void Flip () {
        targetMeshSpin += 180;
    }

    public void SetSelected (bool selected) {
        targetMeshHeight = selected ? riseHeight : 0;
        if (!selected && targetMeshSpin % 360 != 0) {
            targetMeshSpin += 180;
        }
    }

    public bool IsSelected () {
        return riseHeight != 0 && targetMeshHeight == riseHeight;
    }
}