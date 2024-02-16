
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class TestRenderTexture : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Camera miniMapCamera;
    public LayerMask hitLayer;

    [Header("Evnts")]
    public UnityEvent<Collider> onFinde = new UnityEvent<Collider>();


    private RawImage rawImage;


    private void Start()
    {
        rawImage = GetComponent<RawImage>();
        if (miniMapCamera == null)
        {

            Debug.LogError($"map Camera nod found!.  {name} Script is disabled");
            enabled = false;

        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var renderTexture = rawImage.texture;
        var textureSize = new Vector2(renderTexture.width, renderTexture.height);
        // rawImage'a tıklandığında  tıklana ekran pozisoyounun RawImage nesnesinin üzerindeki konumunu alıyoruz.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImage.GetComponent<RectTransform>(), eventData.position, null, out var localPosition);
        // RawIamge'ın UI size bilgilerini alıyoruz
        var rectWidth = rawImage.GetComponent<RectTransform>().rect.width;
        var rectHeight = rawImage.GetComponent<RectTransform>().rect.height;

        // Tıklanan ekran pozisyounun rect üzerinde sol alt (0,0) sağ üst (rectWidth,rectHeight) olarak maplenmesini sağlıyoruz.
        // bunun sebebi pivotu merkezde olması
        var mapedLocal = new Vector2(MapTo(localPosition.x, rectWidth), MapTo(localPosition.y, rectHeight));

        // gelen Rect değerini widht ve height değerlerini RenderTexture üzerinde ki konunumunu hesaplıyoruz.
        // çünkü rectin size'ı render textureden farklı olabilir.
        var clampedLocal = Clamp(mapedLocal, new Vector2(rectWidth, rectHeight), textureSize);
        // Texture bizim için kameraının gördüğü görüntü olduğu için elde ettiğimiz pozisyounu kullanarak 
        // kameradan bir ray göndererek çarptığı nesneyi buluyoruz.
        var ray = miniMapCamera.ScreenPointToRay(clampedLocal, Camera.MonoOrStereoscopicEye.Mono);

        if (Physics.Raycast(ray, out var hitInfo, miniMapCamera.farClipPlane, hitLayer))
        {
            onFinde?.Invoke(hitInfo.collider);
            Debug.DrawLine(hitInfo.point, hitInfo.point + Vector3.up * 10, Color.red, 20);
        }

    }
    // -x ile +x  arasında gelen değeri 0 ile nValue değerine mapler
    static float MapTo(float inputValue, float nValue)
    {

        var half = nValue / 2;
        return (inputValue + half);
    }

    // max vectürü kadar alanda gelen value vectörünü clamp değerine mapler
    private Vector2 Clamp(Vector2 value, Vector2 max, Vector2 calmp)
    {
        var x = (value.x / max.x) * calmp.x;
        var y = (value.y / max.y) * calmp.y;
        return new Vector2(x, y);
    }
}
