using NetWork.NGO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Module.CameraModule
{
    public class RenderTextureCreator : MonoBehaviour
    {

        private RenderTexture _renderTexture;
        private Camera _chracterChooseCamera;
        public RenderTexture RenderTexture { get { return _renderTexture; } }
        public RenderTexture CreateSelectPlayerRenderTexture()
        {
            // 1. RenderTexture 생성
            RenderTexture renderTexture = new RenderTexture(512, 512, 32, RenderTextureFormat.ARGB32);

            // 2. 깊이 스텐실 포맷 설정
            renderTexture.depthStencilFormat = GraphicsFormat.D32_SFloat_S8_UInt;

            // 3. 추가 설정
            renderTexture.antiAliasing = 1;  // None (1x)
            renderTexture.autoGenerateMips = false;  // Mipmap 비활성화
            renderTexture.useMipMap = false;
            renderTexture.enableRandomWrite = false; // Random Write 비활성화
            renderTexture.wrapMode = TextureWrapMode.Clamp;
            renderTexture.filterMode = FilterMode.Bilinear;
            renderTexture.anisoLevel = 0;  // Aniso Level 0

            // 4. RenderTexture 활성화
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
            GetComponentInParent<CharacterSelectorNgo>().SetSelectPlayerRawImage(_renderTexture);
        }

    }
}
