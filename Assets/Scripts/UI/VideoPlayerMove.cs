using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerMove : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    private void Awake()
    {
        VideoManager.Instance.RegisterVideoPlayer(videoPlayer);
    }

    private void OnDestroy()
    {
        VideoManager.Instance.UnregisterVideoPlayer();
    }
 
}
