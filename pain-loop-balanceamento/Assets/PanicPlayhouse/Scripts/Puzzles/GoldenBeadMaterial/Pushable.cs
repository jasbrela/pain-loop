﻿using DG.Tweening;
using FMODUnity;
using PanicPlayhouse.Scripts.Audio;
using PanicPlayhouse.Scripts.Chunk;
using UnityEngine;

namespace PanicPlayhouse.Scripts.Puzzles.GoldenBeadMaterial
{
    public class Pushable : Interactable
    {
        [SerializeField] private int value;
        [SerializeField] private float radius;
        [SerializeField] private LayerMask avoidOverlap;
        [SerializeField] private float duration = 1f;
        
        [Header("SFX")]
        [SerializeField] private EventReference drag;

        private AudioManager _audio;
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, radius);
        }
        
        public int Value => value;
        public bool IsBlocked { get; set; }
        
        public void Push(Vector3 forward)
        {
            if (IsBlocked) return;
            
            Collider[] results = new Collider[10];
            var size = Physics.OverlapSphereNonAlloc(gameObject.transform.position + forward * radius, radius/2, results, avoidOverlap);
            
            if (size > 0) return;

            if (_audio == null) _audio = FindObjectOfType<AudioManager>();
            _audio.PlayOneShot(drag);

            gameObject.transform.DOMove(gameObject.transform.position + forward * radius, duration);
        }
        
    }
}