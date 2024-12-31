using System;
using System.IO;
using System.Reflection;
using Serilog;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SwitchNormalizer.TopRightArea;

public static class TopRightAreaExtension
{
    private static readonly Texture2D _ConstructionIcon = LoadTexture2D("icon.png", 24, 24)!;

    public static void AddButton(Action<bool> onClick) {
        var objectOfType = Object.FindObjectOfType<UI.TopRightArea>();
        if (objectOfType == null) {
            return;
        }

        var transform = objectOfType.transform!.Find("Strip");
        if (transform == null) {
            return;
        }

        var gameObject = new GameObject("SwitchNormalizerButton");
        gameObject.transform!.parent = transform;
        gameObject.transform.SetSiblingIndex(9);
        gameObject.AddComponent<Button>()!.onClick!.AddListener(() => onClick(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)));
        var image = gameObject.AddComponent<Image>()!;
        image.sprite = Sprite.Create(_ConstructionIcon, new Rect(0.0f, 0.0f, 24f, 24f), new Vector2(0.5f, 0.5f))!;
        image.rectTransform!.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 32f);
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 32f);
    }

    private static byte[] GetBytes(string path) {
        using var manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path)!;
        using var destination            = new MemoryStream();
        manifestResourceStream.CopyTo(destination);
        return destination.ToArray();
    }

    private static Texture2D? LoadTexture2D(string path, int width, int height) {
        try {
            var bytes = GetBytes(typeof(TopRightAreaExtension).Namespace + "." + path);
            var tex   = new Texture2D(width, height);
            tex.LoadImage(bytes);
            return tex;
        } catch (Exception ex) {
            Log.Error(ex, "Failed to load texture {0}", path);
        }

        return null;
    }
}
