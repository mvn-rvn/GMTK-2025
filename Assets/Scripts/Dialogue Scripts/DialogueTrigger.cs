using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private string displayText; //if null, doesn't activate (put no text if using it to trigger fading)
    [SerializeField] private bool fadesAutomatically = true; //if no, then fade when player leaves trigger
    [SerializeField] private bool fadeOnExit = false; 
    [SerializeField] private bool deleteAfterActivation = true; 
    [SerializeField] private float fadeTime = 5; //time to fade after text has finished writing

    private DialogueHandler handler;

    private void Awake()
    {
        handler = FindAnyObjectByType<DialogueHandler>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (displayText == null) return;
        if (fadesAutomatically) handler.WriteText(displayText, fadeTime);
        else handler.OverwriteText(displayText, fadeTime);
        if (deleteAfterActivation && !fadeOnExit) Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (fadeOnExit) handler.CloseBox(fadeTime);
        if (deleteAfterActivation) Destroy(gameObject);
    }
}
