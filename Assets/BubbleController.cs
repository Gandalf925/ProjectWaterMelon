using UnityEngine;

public class BubbleController : MonoBehaviour
{
    public PuzzleSceneDirector puzzleSceneDirector;
    public int colorType;
    public bool isMerged;
    public bool rotating;

    private void Update()
    {

        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        BubbleController bubble = collision.gameObject.GetComponent<BubbleController>();
        if (!bubble) return;

        puzzleSceneDirector.Merge(this, bubble);
    }
}
