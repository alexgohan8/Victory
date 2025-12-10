using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;

public class VideoManager : MonoBehaviour
{
    public static VideoManager Instance { get; private set; }

    private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    private Camera renderCamera;
    //private string _currentVideoAddress;
    private bool _isPlaying = false;
    private const float PartialPlaybackPercent = 0.7f;
    // Cache: address → loaded VideoClip
    private readonly Dictionary<string, VideoClip> _cachedVideos = new();

    public bool IsPlaying => _isPlaying;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        BattleEvents.OnBattleEnd += HandleBattleEnd;
    }

    private void OnDestroy()
    {
        BattleEvents.OnBattleEnd -= HandleBattleEnd;
        ReleaseAllVideos();
    }

    private void HandleBattleEnd() 
    {
        if(_isPlaying) 
            _isPlaying = false;
    }

    private void Start()
    {
        audioSource = AudioManager.Instance.SourceSfx;
        renderCamera = Camera.main;
    }

    public void RegisterVideoPlayer(VideoPlayer player) 
    {
        videoPlayer = player; 
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.EnableAudioTrack(0, true);             // Track 0 = first audio stream in clip
        videoPlayer.SetTargetAudioSource(0, audioSource);  // Route to AudioSource
        videoPlayer.targetCamera = renderCamera;
    }
    public void UnregisterVideoPlayer() => videoPlayer = null;

    /// <summary>
    /// Play a move video. If playPartial = true, play only ~95% of it. 
    /// Uses cached clips if previously loaded.
    /// </summary>
    /*
    public async Task PlayMoveVideoAsync(string moveId, string variant = null, bool playPartial = false)
    {
        _isPlaying = true;
        BattleManager.Instance.Freeze();
        MoveEvents.RaiseMoveCutsceneStart();

        if (string.IsNullOrEmpty(moveId) || 
            videoPlayer == null || 
            SettingsManager.Instance.IsAutoBattleEnabled) 
        {
            _isPlaying = false;
            BattleManager.Instance.Unfreeze();
            return;
        }
        

        // Build consistent address
        _currentVideoAddress = variant == null
            ? AddressableLoader.GetMoveVideoAddress(moveId)
            : AddressableLoader.GetMoveVideoVariantAddress(moveId, variant.ToLower());

        // Try to get cached clip
        if (!_cachedVideos.TryGetValue(_currentVideoAddress, out var clip))
        {
            clip = await AddressableLoader.LoadAsync<VideoClip>(_currentVideoAddress);
            if (clip == null)
            {
                LogManager.Warning($"[VideoManager] Video not found: {_currentVideoAddress}");
                return;
            }

            // Cache it
            _cachedVideos[_currentVideoAddress] = clip;
        }

        // Assign and play
        LogManager.Trace($"[VideoManager] Now playing video: {_currentVideoAddress}");
        videoPlayer.clip = clip;
        videoPlayer.targetCameraAlpha = 1f;
        videoPlayer.Play();

        if (playPartial)
        {
            // Wait ~95% duration
            float duration = (float)clip.length;
            float waitMs = duration * PartialPlaybackPercent * 1000f;
            await Task.Delay(Mathf.CeilToInt(waitMs));
            videoPlayer.Stop();
        }
        else
        {
            // Wait until completion (loopPointReached)
            var tcs = new TaskCompletionSource<bool>();
            void OnDone(VideoPlayer vp)
            {
                vp.loopPointReached -= OnDone;
                tcs.TrySetResult(true);
            }
            videoPlayer.loopPointReached += OnDone;
            await tcs.Task;
        }

        LogManager.Trace($"[VideoManager] Finished playing video: {_currentVideoAddress}");
        _isPlaying = false;
        videoPlayer.targetCameraAlpha = 0f;
        BattleManager.Instance.Unfreeze();
        MoveEvents.RaiseMoveCutsceneEnd();
    }
    */

    //streamingAssets version
    public async Task PlayMoveVideoAsync(string moveId, string variant = null, bool playPartial = false)
    {
        _isPlaying = true;
        BattleManager.Instance.Freeze();
        MoveEvents.RaiseMoveCutsceneStart();

        if (string.IsNullOrEmpty(moveId) || 
            videoPlayer == null || 
            SettingsManager.Instance.IsAutoBattleEnabled) 
        {
            _isPlaying = false;
            BattleManager.Instance.Unfreeze();
            MoveEvents.RaiseMoveCutsceneEnd();
            return;
        }
        

        // Build consistent address
        string videoFilename = variant == null
            ? AddressableLoader.GetMoveVideoAddress(moveId)
            : AddressableLoader.GetMoveVideoVariantAddress(moveId, variant.ToLower());

        // Assign and play
        LogManager.Trace($"[VideoManager] Now playing video: {videoFilename}");

        #if UNITY_EDITOR || UNITY_LINUX
            videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "test_linux" + ".webm");
        #else
            videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFilename + ".mp4");
        #endif

        videoPlayer.Prepare();

        var prepareTcs = new TaskCompletionSource<bool>();
        videoPlayer.prepareCompleted += vp => prepareTcs.TrySetResult(true);
        await prepareTcs.Task;

        double videoLength = videoPlayer.length;
        LogManager.Trace($"[VideoManager] Video length: {videoLength} seconds");


        videoPlayer.targetCameraAlpha = 1f;
        videoPlayer.Play();

        if (playPartial)
        {
            // Wait ~95% duration
            float waitMs = (float)videoLength * PartialPlaybackPercent * 1000f;
            await Task.Delay(Mathf.CeilToInt(waitMs));
            videoPlayer.Stop();
        }
        else
        {
            // Wait until completion (loopPointReached)
            var tcs = new TaskCompletionSource<bool>();
            void OnDone(VideoPlayer vp)
            {
                vp.loopPointReached -= OnDone;
                tcs.TrySetResult(true);
            }
            videoPlayer.loopPointReached += OnDone;
            await tcs.Task;
        }

        LogManager.Trace($"[VideoManager] Finished playing video: {videoFilename}");
        _isPlaying = false;
        videoPlayer.targetCameraAlpha = 0f;
        BattleManager.Instance.Unfreeze();
        MoveEvents.RaiseMoveCutsceneEnd();
    }

    /// <summary>
    /// Releases all cached video clips (and clears address cache).
    /// </summary>
    public void ReleaseAllVideos()
    {
        foreach (var kvp in _cachedVideos)
            AddressableLoader.Release(kvp.Key);

        _cachedVideos.Clear();
        //_currentVideoAddress = null;

        Debug.Log("[VideoManager] Released all cached video clips.");
    }

}
