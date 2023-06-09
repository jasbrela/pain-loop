﻿using System.Collections.Generic;
using System;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

namespace PanicPlayhouse.Scripts.Entities
{
    public class EntityAnimation : MonoBehaviour
    {
        [Header("SFX")]
        [SerializeField] private GenericDictionary<string, EventReference> oneShotSFXes = new GenericDictionary<string, EventReference>();

        [Header("Dependencies")]
        [SerializeField] private Animator animator;

        [HideIf("AnimatorIsNull")]
        public GenericDictionary<string, Animation> animations = new GenericDictionary<string, Animation>();

        public bool AnimatorIsNull => animator == null;

        private PanicPlayhouse.Scripts.Audio.AudioManager _audioManager;
        private PanicPlayhouse.Scripts.Audio.AudioManager audioManager
        {
            get
            {
                if (_audioManager == null)
                    _audioManager = FindObjectOfType<PanicPlayhouse.Scripts.Audio.AudioManager>();

                return _audioManager;
            }
        }

        private void OnValidate()
        {
            if (animator == null) return;

            foreach (string key in animations.Keys)
            {
                if (animations[key] == null)
                    animations[key] = new Animation();

                animations[key].SetAnimator(animator);
            }
        }

        private void Awake()
        {
            OnValidate();
        }

        public void PlayOneShotSFX(string sfx)
        {
            if (!oneShotSFXes.ContainsKey(sfx))
                return;

            if (audioManager == null)
                return;

            audioManager.PlayOneShot(oneShotSFXes[sfx], transform.position);
        }

        public Animation this[string key]
        {
            get => animations[key];
            set => animations[key] = value;
        }


        [Serializable]
        public class Animation
        {
            [HideIf("AnimatorIsNull")]
            [SerializeField]
            [AnimatorParam("_animator")]
            protected string _param;
            protected Animator _animator;

            public bool AnimatorIsNull => _animator == null;

            public virtual void SetValue() => _animator.SetTrigger(_param);
            public virtual void SetValue(bool value) => _animator.SetBool(_param, value);
            public virtual void SetValue(float value) => _animator.SetFloat(_param, value);
            public virtual void SetValue(int value) => _animator.SetInteger(_param, value);

            public Animation SetAnimator(Animator animator)
            {
                _animator = animator;
                return this;
            }

            public Animation()
            {
                _animator = null;
                _param = "New Animation";
            }

            public Animation(Animator animator)
            {
                _animator = animator;
                _param = "New Animation";
            }

            public Animation(Animator animator, string param)
            {
                _animator = animator;
                this._param = param;
            }
        }
    }
}