using UnityEngine;
using UnityEngine.UI;

namespace PhotoPicker
{
    public class ImagePlacer : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _defaultSprite;

        public void SetImage(byte[] photo)
        {
            if (photo != null)
            {
                Texture2D texture = ByteArrayToTexture(photo, 2, 2);
                Debug.Log(texture.width * texture.height * 4);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
                _image.sprite = sprite;
                _image.enabled = true;
            }
            else
            {
                _image.sprite = _defaultSprite;
                _image.enabled = false;
            }
        }

        private Texture2D ByteArrayToTexture(byte[] byteArray, int width, int height, TextureFormat format = TextureFormat.RGBA32)
        {
            Texture2D texture = new Texture2D(width, height, format, true);

            texture.LoadImage(byteArray);

            texture.Apply();

            return texture;
        }
    }
}
