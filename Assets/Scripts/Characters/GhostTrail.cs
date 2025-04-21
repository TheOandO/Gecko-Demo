using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    public float ghostDelay;
    private float ghostDelaySecond;
    public GameObject ghost;
    public bool makeGhost = false;

	void Start()
    {
        ghostDelaySecond = ghostDelay;
	}

    void Update()
    {
        if (makeGhost)
        {
            if (ghostDelaySecond > 0)
            {
                ghostDelaySecond -= Time.deltaTime;
            }
            else
            {
                GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                currentGhost.transform.localScale = transform.localScale;

                ghostDelaySecond = ghostDelay;
                Destroy(currentGhost, 1f);
            }
        }

    }
}
