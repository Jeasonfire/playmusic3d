﻿using UnityEngine;
using System.Collections;

public class TextToTextureRenderer : MonoBehaviour {
    private static TextToTextureRenderer textRenderer;

    public RenderTexture textTexture;
    public Camera textCamera;
    public TextMesh textMesh;

    public static Texture2D RenderText (GameObject textRendererPrefab, string text, Color bgColor) {
        if (textRenderer == null) {
            textRenderer = Instantiate<GameObject>(textRendererPrefab).GetComponent<TextToTextureRenderer>();
        }
        textRenderer.textCamera.backgroundColor = bgColor;
        textRenderer.textMesh.text = text;
        textRenderer.textCamera.Render();

        int texWidth = textRenderer.textTexture.width;
        int texHeight = textRenderer.textTexture.height;
        Texture2D result = new Texture2D(texWidth, texHeight);
        RenderTexture previouslyActive = RenderTexture.active;
        RenderTexture.active = textRenderer.textTexture;
        result.ReadPixels(new Rect(0, 0, texWidth, texHeight), 0, 0);
        result.Apply();
        RenderTexture.active = previouslyActive;

        return result;
    }
}
