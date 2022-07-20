using UnityEngine.Events;
using UnityEngine;
using System;

namespace Actions
{
    public class Ant
    {
        /// <summary>
        /// Notify that the Ant has died, along with its Rank.
        /// </summary>
        public static UnityAction<int> HasDied;
        
    }

    public class Toasty
    {
        /// <summary>
        /// Toasty has been bitten with his current HP.
        /// </summary>
        public static UnityAction<int> HasBeenBitten;
        public static UnityAction HasDied;
        public static UnityAction IsReadyToPlay;
        public static UnityAction HealToasty;
    }

    public class Game
    {
        public static UnityAction HasClicked;
        public static Action<AudioClip> PlayMusic;
        public static Action<AudioClip> PlaySFX;
        public static Action<AudioClip> PlayFootsteps;
        public static Action<AudioClip> PlayTap;
        public static Action StopSFX;
        public static Action LoseGame;
        public static Action WinGame;
    }
}
