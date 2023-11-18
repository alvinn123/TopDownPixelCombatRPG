using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullWolf : Shooter
{
    private Animator myAnimator;
    readonly int ATTACK_HASH = Animator.StringToHash("Attack");
    private AudioSource bossMusicAudioSource;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent < SpriteRenderer>();
        bossMusicAudioSource = GetComponent<AudioSource>();
        
        StartCoroutine(RollDice());
    }

    public override void Attack()
    {
        myAnimator.SetTrigger(ATTACK_HASH);

        // Enable and play the boss music audio source when attacking.
        if (bossMusicAudioSource != null && !bossMusicAudioSource.isPlaying)
        {
            bossMusicAudioSource.enabled = true;
            bossMusicAudioSource.Play();
        }
        oscillate = !oscillate;
        stagger = !stagger;

        base.Attack();
    }
    private IEnumerator RollDice()
    {
        while (true)
        {
            // Roll the dice between 1 and 2 (inclusive).
            int diceRoll = Random.Range(1, 3);

            // If the dice is 1, set oscillate to true. If the dice is 2, set oscillate to false.
            oscillate = (diceRoll == 1);
            stagger = (diceRoll == 1);
            angleSpread = Random.Range(0, 2) == 0 ? 45 : 359;


            // Wait for 15 seconds before rolling the dice again.
            yield return new WaitForSeconds(15f);
        }
    }
}
