using System;
using System.Collections;
using System.Collections.Generic;
using PhotoPicker;
using UnityEngine;
using UnityEngine.UI;

public class PhotosController : MonoBehaviour
{
    [SerializeField] private ImagePlacer _imagePlacer;
    [SerializeField] private GameObject _noItemObj;
    private byte[] _photosPath = null;
    //[SerializeField] private Button _addButton;

    public event Action SetPhoto;

    public void OnSetImageButtonClick()
    {
        ImagePicker.PickImage(OnImageSelected);
    }

    private void OnImageSelected(Texture2D texture)
    {
        Texture2D copy = GetReadableCopy(texture);
        if (copy != null)
        {
            _photosPath = TextureToByteArray(copy, copy.width / 2, copy.height / 2);
            UpdatePhotos();
        }
    }

    public byte[] TextureToByteArray(Texture2D texture, int newWidth, int newHeight)
    {
        Texture2D readableTexture = new Texture2D(newWidth, newHeight, texture.format, true);

        // Уменьшаем размер текстуры
        Color[] pixels = texture.GetPixels();

        // Ресайз текстуры
        Color[] resizedPixels = new Color[newWidth * newHeight];
        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                int originalX = x * texture.width / newWidth;
                int originalY = y * texture.height / newHeight;
                resizedPixels[y * newWidth + x] = pixels[originalY * texture.width + originalX];
            }
        }

        // Устанавливаем уменьшенные пиксели и применяем изменения
        readableTexture.SetPixels(resizedPixels);
        readableTexture.Apply();

        // Кодируем текстуру в PNG и возвращаем массив байтов
        return readableTexture.EncodeToPNG();
    }

    public Texture2D GetReadableCopy(Texture2D source)
    {
        if (source == null)
        {
            return null;
        }

        RenderTexture renderTex = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

    public Texture2D ByteArrayToTexture(byte[] bytes)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        return texture;
    }

    public void UpdatePhotos()
    {
        _imagePlacer.gameObject.SetActive(false);
        if (_noItemObj != null)
        {
            _noItemObj.gameObject.SetActive(true);
        }

        if (_photosPath != new byte[] { })
        {
            _imagePlacer.SetImage(_photosPath);
            _imagePlacer.gameObject.SetActive(true);
            SetPhoto?.Invoke();
            //_addButton.enabled = false;
            if (_noItemObj != null)
            {
                _noItemObj.gameObject.SetActive(false);
            }
        }
    }

    public void ResetPhotos()
    {
        _imagePlacer.gameObject.SetActive(false);
        _photosPath = null;
        if (_noItemObj != null)
        {
            _noItemObj.gameObject.SetActive(true);
        }

        // _addButton.gameObject.SetActive(true);
    }

    public void SetPhotos(byte[] photo)
    {
        _photosPath = photo;
        UpdatePhotos();
    }

    public byte[] GetPhoto()
    {
        if (_photosPath == null)
        {
            Debug.Log("Null");
            return null;
        }
        else
        {
            return _photosPath;
        }
    }
}