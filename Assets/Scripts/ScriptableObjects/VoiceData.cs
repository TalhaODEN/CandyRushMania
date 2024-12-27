using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VoiceData", menuName = "Game/Menu/VoiceData")]
public class VoiceData : ScriptableObject
{
    public Sprite ActiveMusicButtonSprite{ get {return activeMusicButtonSprite;} }
    public Sprite ActiveSoundButtonSprite { get { return activeSoundButtonSprite; } }
    public Sprite PassiveMusicButtonSprite { get { return passiveMusicButtonSprite; } }
    public Sprite PassiveSoundButtonSprite { get { return passiveSoundButtonSprite; } }

    [Header("Music and Volume Button Sprites")]
    [SerializeField] private Sprite activeMusicButtonSprite;
    [SerializeField] private Sprite activeSoundButtonSprite;
    [SerializeField] private Sprite passiveMusicButtonSprite;
    [SerializeField] private Sprite passiveSoundButtonSprite;

    [Header("Bool Variables")]
    public bool isMusicActive;
    public bool isSoundActive;
}
