using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class RenderTextureCreator : MonoBehaviour
{

    private RenderTexture _renderTexture;
    private Camera _chracterChooseCamera;
    public RenderTexture RenderTexture { get { return _renderTexture; } }
    public RenderTexture CreateSelectPlayerRenderTexture()
    {
        // 1. RenderTexture ����
        RenderTexture renderTexture = new RenderTexture(512, 512, 32, RenderTextureFormat.ARGB32);

        // 2. ���� ���ٽ� ���� ����
        renderTexture.depthStencilFormat = GraphicsFormat.D32_SFloat_S8_UInt;

        // 3. �߰� ����
        renderTexture.antiAliasing = 1;  // None (1x)
        renderTexture.autoGenerateMips = false;  // Mipmap ��Ȱ��ȭ
        renderTexture.useMipMap = false;
        renderTexture.enableRandomWrite = false; // Random Write ��Ȱ��ȭ
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderTexture.filterMode = FilterMode.Bilinear;
        renderTexture.anisoLevel = 0;  // Aniso Level 0

        // 4. RenderTexture Ȱ��ȭ
        renderTexture.Create();

        return renderTexture;
    }

    private void Awake()
    {
        _renderTexture = CreateSelectPlayerRenderTexture();
        _chracterChooseCamera = GetComponent<Camera>();
        _chracterChooseCamera.targetTexture = _renderTexture;
    }
    void OnDestroy()
    {
        if (Camera.main.targetTexture != null)
        {
            Camera.main.targetTexture.Release();
            Camera.main.targetTexture = null;
        }
    }
    private void Start()
    {
        GetComponentInParent<CharacterSelectorNGO>().SetSelectPlayerRawImage(_renderTexture);
    }

}
