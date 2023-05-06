using UnityEngine;

namespace Prototyping
{
    public class RandomColor : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            SetColor(NewRandomColor());
        }

        private void SetColor(Color color)
        {
            meshRenderer.material.color = color;
        }


        private static Color NewRandomColor()
        {
            var randomColor = Random.ColorHSV(0f, 1f);
            return randomColor;
        }
    }
}
