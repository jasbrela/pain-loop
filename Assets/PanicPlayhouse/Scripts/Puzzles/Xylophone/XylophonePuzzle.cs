﻿using System.Collections.Generic;
using FMODUnity;
using PanicPlayhouse.Scripts.Audio;
using PanicPlayhouse.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using Event = PanicPlayhouse.Scripts.ScriptableObjects.Event;

namespace PanicPlayhouse.Scripts.Puzzles.Xylophone
{
    public class XylophonePuzzle : MonoBehaviour
    {
        [Header("Insanity")]
        [SerializeField] private float insanityPenalty;
        [SerializeField] private float insanityReward;
        [SerializeField] private FloatVariable insanity;
        
        [Header("Monster")]
        [SerializeField] private Event triggerMonster;
        [SerializeField] private int triggerInterval; // this could be changed based on insanity...
        
        [Header("Puzzle")]
        [SerializeField] private Event onFinish;
        [FormerlySerializedAs("order")] [SerializeField] private List<XylophoneButton> password;
        [SerializeField] private List<SpriteRenderer> drawingsToColor;
        [SerializeField] private List<SpriteRenderer> chalkToColor;
        [SerializeField] private Color disabledColor;

        [Header("SFX")]
        [SerializeField] private EventReference success;
        
        private AudioManager _audio;
        
        private List<XylophoneButton> _uniqueButtons;
        private int _buttonCount;
        private int _triggerCount;
        
        private bool IsActivated { get; set; }
        private bool IsFinished { get; set; }
        
        private void Start()
        {
            _audio = FindObjectOfType<AudioManager>();
            _uniqueButtons = new List<XylophoneButton>(FindObjectsOfType<XylophoneButton>());
            ColorChalk();
            
            if (password.Count == 0)
            {
                gameObject.SetActive(false);
#if UNITY_EDITOR
                Debug.Log(name.Bold().Color("#FF4500") + " has been deactivated.");
#endif
                return;
            }
            
            foreach (XylophoneButton button in password)
            {
                button.Puzzle = this;
            }
            
            ActivatePuzzle();
        }

        private void ColorChalk()
        {
            for (int i = 0; i < chalkToColor.Count; i++)
            {
                chalkToColor[i].color = password[i].Color;
            }
        }

        public void EnableFlowers()
        {
            foreach (SpriteRenderer drawing in drawingsToColor)
            {
                drawing.color = disabledColor;
                drawing.gameObject.SetActive(true);
            }
        }

        public void ActivatePuzzle()
        {
            if (IsActivated || IsFinished) return;
            
            IsActivated = true;

#if UNITY_EDITOR
            Debug.Log(name.Bold().Color("#00FA9A") + " has been activated.");
#endif
            
            foreach (XylophoneButton button in _uniqueButtons)
            {
                button.IsBlocked = false;
            }
        }

        public void OnPressButton(XylophoneButton button, EventReference reference)
        {
            if (IsFinished || !IsActivated) return;

            _audio.PlayOneShot(reference, button.transform.position);
            
            if (password[_buttonCount] == button)
            {
                drawingsToColor[_buttonCount].color = password[_buttonCount].Color;
                _buttonCount++;
            }
            else
            {
                foreach (SpriteRenderer sprite in drawingsToColor)
                    sprite.color = disabledColor;

                _triggerCount++;
                _buttonCount = 0;
            }
            
            if (_triggerCount == triggerInterval && triggerMonster != null)
            {
                _triggerCount = 0;
                triggerMonster.Raise();
            }
            
            insanity.Value += insanityPenalty;

            if (_buttonCount != password.Count) return;
            
            IsFinished = true;
            IsActivated = false;
            insanity.Value -= insanityReward;
            if (onFinish != null) onFinish.Raise();
            _audio.PlayOneShot(success);

            foreach (XylophoneButton btn in _uniqueButtons) btn.IsBlocked = true;
        }
    }
}