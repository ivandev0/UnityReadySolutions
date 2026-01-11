using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrototUnity.Progress {
    public class ProgressBarUI : MonoBehaviour {
        [SerializeField] private Progressable progressBar;
        [SerializeField] private Vector3 progressBarPosition;
        [SerializeField] private Color progressBarColor = Color.white;
        [SerializeField] private bool hideOnFullProgress;
        [SerializeField] private bool showNumbers = false;
        
        private GameObject progressBarObject;
        private Image barImage;
        private TextMeshPro textMesh;
    
        private void Start() {
            CreateCanvas();

            if (progressBar == null) {
                throw new ArgumentException($"Component {progressBar} must implement interface IHasProgressBar");
            }
            
            progressBar.ProgressChangedEvent += (bar, _) => {
                var normalizedValue = bar.Current / bar.Total;
                barImage.fillAmount = normalizedValue;
                Render(bar);
            }; 
            
            Render(progressBar);
        }

        private void Render(Progressable progress) {
            if (showNumbers) textMesh.text = $"{progress.Current} / {progress.Total}";
            HideOrShow(hide: hideOnFullProgress && progress.IsFull());
        }

        private void HideOrShow(bool hide) {
            if (hide) Hide(); else Show();
        }
        
        private void Show() {
            progressBarObject.SetActive(true);
        }
    
        private void Hide() {
            progressBarObject.SetActive(false);
        }

        private void CreateCanvas() {
            progressBarObject = new GameObject("ProgressBarUI");
            progressBarObject.transform.parent = transform;
            progressBarObject.transform.localRotation = Quaternion.identity;
            progressBarObject.AddComponent<Canvas>();
            var canvasRectTransform = progressBarObject.GetComponent<RectTransform>();
            canvasRectTransform.localPosition = progressBarPosition;
            canvasRectTransform.sizeDelta = Vector2.one;
            
            var backgroundImageObject = new GameObject("BackgroundImage");
            backgroundImageObject.transform.parent = progressBarObject.transform;
            backgroundImageObject.transform.localRotation = Quaternion.identity;
            var backgroundImage = backgroundImageObject.AddComponent<Image>();
            backgroundImage.color = Color.black;
            var backgroundRectTransform = backgroundImageObject.GetComponent<RectTransform>();
            backgroundRectTransform.localPosition = Vector3.zero;
            backgroundRectTransform.sizeDelta = new Vector2(1.05f, 0.30f);
            
            var barImageObject = new GameObject("BarImage");
            barImageObject.transform.parent = progressBarObject.transform;
            barImageObject.transform.localRotation = Quaternion.identity;
            barImage = barImageObject.AddComponent<Image>();
            barImage.sprite = CreateSprite(progressBarColor);
            barImage.type = Image.Type.Filled;
            barImage.fillMethod = Image.FillMethod.Horizontal;
            barImage.fillAmount = 1f;
            var barImageRectTransform = barImageObject.GetComponent<RectTransform>();
            barImageRectTransform.localPosition = Vector3.zero;
            barImageRectTransform.sizeDelta = new Vector2(1, 0.25f);
            
            var text = new GameObject("Text");
            text.transform.parent = progressBarObject.transform;
            text.transform.localRotation = Quaternion.identity;
            textMesh = text.AddComponent<TextMeshPro>();
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.sortingOrder = 1;
            textMesh.enableAutoSizing = true;
            textMesh.fontSizeMin = 1;
            textMesh.color = new Color(1, 0, 1);
            var textRectTransform = text.GetComponent<RectTransform>();
            textRectTransform.localPosition = Vector3.zero;
            textRectTransform.sizeDelta = new Vector2(1, 0.25f);
        }

        private static Sprite CreateSprite(Color color) {
            var width = 1;
            var height = 1;
            var texture = new Texture2D(width, height);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        }
    }
}
