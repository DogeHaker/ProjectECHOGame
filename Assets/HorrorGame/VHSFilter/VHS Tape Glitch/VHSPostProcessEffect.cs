using UnityEngine;
using UnityEngine.Video;

[ExecuteAlways]
[AddComponentMenu("Image Effects/GlitchEffect")]
[RequireComponent(typeof(Camera))]
public class VHSPostProcessEffect : MonoBehaviour
{
    // ── Shader & Clip ────────────────────────────────────────────────────────

    [Header("Setup")]
    [Tooltip("Assign VHSPostProcessEffect.shader here.")]
    public Shader shader;

    [Tooltip("Video clip to use as the VHS overlay texture.")]
    public VideoClip VHSClip;


    // ── Overlay Controls ─────────────────────────────────────────────────────

    [Header("Overlay")]

    [Range(0f, 2f)]
    [Tooltip("How strongly the VHS overlay is blended on top of the scene. 0 = invisible, 1 = original strength, 2 = double.")]
    public float intensity = 1f;

    [Range(0.1f, 4f)]
    [Tooltip("Zoom scale of the VHS overlay. Values below 1 zoom out (shows borders), above 1 zoom in (crops the clip).")]
    public float scale = 1f;

    [Range(-1f, 1f)]
    [Tooltip("Horizontal position offset of the VHS overlay in UV space. 0 = centred.")]
    public float offsetX = 0f;

    [Range(-1f, 1f)]
    [Tooltip("Vertical position offset of the VHS overlay in UV space. 0 = centred.")]
    public float offsetY = 0f;


    // ── Private ──────────────────────────────────────────────────────────────

    private float         _yScanline;
    private float         _xScanline;
    private Material      _material;
    private VideoPlayer   _player;


    // ── Lifecycle ────────────────────────────────────────────────────────────

    void OnEnable()
    {
        if (_material == null)
            _material = new Material(shader);

        _player = GetComponent<VideoPlayer>();

        if (_player == null)
            _player = gameObject.AddComponent<VideoPlayer>();

        _player.hideFlags      = HideFlags.HideInInspector | HideFlags.NotEditable;
        _player.isLooping      = true;
        _player.renderMode     = VideoRenderMode.APIOnly;
        _player.audioOutputMode= VideoAudioOutputMode.None;
        _player.clip           = VHSClip;

        if (!_player.isPlaying)
            _player.Play();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_material == null || _player == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        // VHS video frame
        _material.SetTexture("_VHSTex", _player.texture);

        // Scanline animation (original behaviour preserved)
        _yScanline += Time.deltaTime * 0.01f;
        _xScanline -= Time.deltaTime * 0.1f;

        if (_yScanline >= 1f)        _yScanline = Random.value;
        if (_xScanline <= 0f || Random.value < 0.05f) _xScanline = Random.value;

        _material.SetFloat("__a1", _yScanline);
        _material.SetFloat("__b2", _xScanline);

        // New overlay controls
        _material.SetFloat("_Intensity", intensity);
        _material.SetFloat("_Scale",     scale);
        _material.SetFloat("_OffsetX",   offsetX);
        _material.SetFloat("_OffsetY",   offsetY);

        Graphics.Blit(source, destination, _material);
    }

    void OnDisable()
    {
        if (_material != null) DestroyImmediate(_material);

        if (_player != null)
        {
            // If the game is actively running, destroy it safely at the end of the frame
            if (Application.isPlaying)
            {
                Destroy(_player);
            }
            else
            {
                // This stops it from running if Unity is in the middle of shutting down Play Mode
                #if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    DestroyImmediate(_player);
                }
                #endif
            }
        }
    }
}
