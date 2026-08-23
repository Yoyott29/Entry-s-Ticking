using UnityEngine;
using System.Collections;

public class Barrel : MonoBehaviour
{
    public Vector3Int tilePosition;
    public Animator animator;
    public string breakTrigger = "Break";
    public float residueDuration = 1.2f;

    public void Break()
    {
        if (animator != null)
            animator.SetTrigger(breakTrigger);

        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(residueDuration);
        Destroy(gameObject);
    }
}
