using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");
    private Animator animator;
    private CapsuleCollider2D capsuleCollider2D;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<Player>();
            if (player == null || !player.IsHoldingKey)
            {
                FindObjectOfType<AudioManager>().Play("doorIsClosed");
                return;
            }
            FindObjectOfType<AudioManager>().Play("doorOpen");
            animator.SetBool(IsOpen, true);
            player.UseKey();
            capsuleCollider2D.isTrigger = true;
        }
    }
}
