using UnityEngine;

namespace Utils
{
    public static class MouseChecker
    {
        public static bool IsMouseInsideContainer(Canvas canvas, RectTransform containerTransform)
        {
            var mousePosition = Input.mousePosition;
            var canvasRect = ((RectTransform)canvas.transform).rect;
            var normalizedMousePosition = new Vector2(mousePosition.x / Screen.width * canvasRect.width,
                mousePosition.y / Screen.height * canvasRect.height);
            var (squareMin, squareMax) = GetRectTransformSquare(canvas, containerTransform);
            
            return normalizedMousePosition.x > squareMin.x &&
                   normalizedMousePosition.x < squareMax.x &&
                   normalizedMousePosition.y > squareMin.y &&
                   normalizedMousePosition.y < squareMax.y;
        }

        private static (Vector2 from, Vector2 to) GetRectTransformSquare(Canvas canvas, RectTransform rectTransform)
        {
            var from = rectTransform.anchorMin * ((RectTransform)canvas.transform).rect.size;
            var to = rectTransform.anchorMax * ((RectTransform)canvas.transform).rect.size;

            if (rectTransform.rect.x < 0)
                from += Vector2.right * rectTransform.rect.x;
            else
                to += Vector2.right * rectTransform.rect.x;

            if (rectTransform.rect.y < 0)
                from += Vector2.up * rectTransform.rect.y;
            else
                to += Vector2.up * rectTransform.rect.y;

            return (from, to);
        }
    }
}
