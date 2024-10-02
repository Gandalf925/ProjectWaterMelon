using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleSceneDirector : MonoBehaviour
{
    [SerializeField] List<BubbleController> bubbles;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] AudioClip seDrop;
    [SerializeField] AudioClip seMerge;
    int score;

    BubbleController currentBubble;
    const float spawnItemY = 3.5f;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        gameOverPanel.SetActive(false);
        StartCoroutine(SpawnCurrentBubble());
    }

    private void Update()
    {
        if (!currentBubble) return;

        currentBubble.rotating = true;
        if (currentBubble.rotating)
        {
            currentBubble.transform.Rotate(0, 0, 0.2f);
        }

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // x軸の範囲を-3～3に制限
        float clampedX = Mathf.Clamp(worldPoint.x, -2.9f, 2.9f);
        Vector2 bubblePosition = new Vector2(clampedX, spawnItemY);

        currentBubble.transform.position = bubblePosition;

        if (Input.GetMouseButtonUp(0))
        {
            currentBubble.GetComponent<Rigidbody2D>().gravityScale = 1;
            currentBubble = null;
            StartCoroutine(SpawnCurrentBubble());
            audioSource.PlayOneShot(seDrop);
        }
    }

    BubbleController SpawnBubble(Vector2 position, int colorType = -1)
    {
        int index = Random.Range(0, bubbles.Count / 2);

        if (0 < colorType)
        {
            index = colorType;
        }

        BubbleController bubble = Instantiate(bubbles[index], position, Quaternion.identity);

        bubble.puzzleSceneDirector = this;
        bubble.colorType = index;

        return bubble;
    }

    IEnumerator SpawnCurrentBubble()
    {
        yield return new WaitForSeconds(1.0f);

        currentBubble = SpawnBubble(new Vector2(0, spawnItemY));
        currentBubble.GetComponent<Rigidbody2D>().gravityScale = 0;
    }

    public void Merge(BubbleController bubbleA, BubbleController bubbleB)
    {
        if (currentBubble == bubbleA || currentBubble == bubbleB)
        {
            enabled = false;
            gameOverPanel.SetActive(true);

            return;
        }

        if (bubbleA.isMerged || bubbleB.isMerged) return;
        if (bubbleA.colorType != bubbleB.colorType) return;

        int nextColor = bubbleA.colorType + 1;
        if (bubbles.Count <= nextColor) return;

        Vector2 lerpPosition = Vector2.Lerp(bubbleA.transform.position, bubbleB.transform.position, 0.5f);

        BubbleController newBubble = SpawnBubble(lerpPosition, nextColor);

        bubbleA.isMerged = true;
        bubbleB.isMerged = true;

        Destroy(bubbleA.gameObject);
        Destroy(bubbleB.gameObject);

        score += newBubble.colorType * 20;
        scoreText.text = $"Score: {score}";

        audioSource.PlayOneShot(seMerge);
    }

    public void OnClickRetry()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("PuzzleScene");
    }
}
