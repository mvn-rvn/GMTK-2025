using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DialogueHandler : MonoBehaviour
{
    private GameObject textBacking;
    private TextMeshProUGUI textDisplay;

    [SerializeField] private float writeDelay;
    [SerializeField] private float haltTime;
    [SerializeField] private float boxExpandSpeed;
    [SerializeField] private float textFadeSpeed;

    private bool boxActive;
    private bool textActive;

    private Vector3 initialScale;
    private Vector3 closedScale;
    private Color initialColor;

    private void Awake()
    {
        textBacking = transform.GetChild(0).gameObject;
        textDisplay = GetComponentInChildren<TextMeshProUGUI>();
        GetComponent<Canvas>().worldCamera = FindAnyObjectByType<Camera>();

        initialScale = textBacking.transform.localScale;
        closedScale = new Vector3(0, initialScale.y, initialScale.z);
        initialColor = textDisplay.color;
    }

    private void Start()
    {
        if (boxActive) return;

        textBacking.transform.localScale = closedScale;
        textBacking.SetActive(false);
        textDisplay.gameObject.SetActive(false);
    }

    public void OverwriteText(string text, float delay = 0)
    {
        if (!boxActive) StartCoroutine(ActivateBox());
        if (!textActive) StartCoroutine(WriteToBox(text));
        else StartCoroutine(OverwriteToBox(text, delay));
    }

    public void WriteText(string text, float waitTime = 0)
    {
        if (!boxActive) StartCoroutine(ActivateBox());
        StartCoroutine(WriteToBox(text));
        StartCoroutine(DeactivateBox(waitTime));
    }

    public void CloseBox(float waitTime = 0)
    {
        StartCoroutine(DeactivateBox(waitTime));
    }

    private void BoxLogic(Vector3 start, Vector3 end, ref float counter)
    {
        float scaleX = Mathf.SmoothStep(start.x, end.x, counter);
        textBacking.transform.localScale = new Vector3(scaleX, initialScale.y, initialScale.z);

        float delta = Time.deltaTime * boxExpandSpeed;
        counter = Mathf.Clamp(counter + delta, 0, 1);
    }

    private void TextLogic(Color start, Color end, ref float counter)
    {
        float transparency = Mathf.Lerp(start.a, end.a, counter);
        textDisplay.color = new Color(initialColor.r, initialColor.g, initialColor.b, transparency);

        float delta = Time.deltaTime * textFadeSpeed;
        counter = Mathf.Clamp(counter + delta, 0, 1);
    }

    private IEnumerator ActivateBox()
    {
        textBacking.SetActive(true);
        float counter = 0;
        while (counter < 1)
        {
            BoxLogic(closedScale, initialScale, ref counter);
            yield return null;
        }
        textBacking.transform.localScale = initialScale;
        boxActive = true;

        yield break;
    }

    private IEnumerator WriteToBox(string text)
    {
        yield return new WaitUntil(() => boxActive);
        textDisplay.gameObject.SetActive(true);

        /*
        textDisplay.color = initialColor;
        string container = "";
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '#')
            {
                container += '\n';
                i++;
            }
            if (text[i] == '@')
            {
                yield return new WaitForSeconds(haltTime);
                i++;
            }

            container = container + text[i];

            textDisplay.text = container;
            yield return new WaitForSeconds(writeDelay);
        }
        */

        text = text.Replace('#', '\n');
        text = text.Replace("@", string.Empty);
        textDisplay.text = text;

        float counter = 0;
        while (counter < 1)
        {
            TextLogic(Color.clear, initialColor, ref counter);
            yield return null;
        }

        
        textActive = true;
        yield break;
    }

    private IEnumerator OverwriteToBox(string text, float waitTime = 0f)
    {
        yield return new WaitUntil(() => boxActive);

        if (textActive)
        {
            yield return new WaitForSeconds(waitTime);

            float counter = 0;
            while (counter < 1)
            {
                TextLogic(initialColor, Color.clear, ref counter);
                yield return null;
            }
        }

        text = text.Replace('#', '\n');
        text = text.Replace("@", string.Empty);
        textDisplay.text = text;

        float counter2 = 0;
        while (counter2 < 1)
        {
            TextLogic(Color.clear, initialColor, ref counter2);
            yield return null;
        }

        textActive = true;
    }

    private IEnumerator DeactivateBox(float waitTime)
    {
        yield return new WaitUntil(() => textActive);
        yield return new WaitForSeconds(waitTime);

        float counter = 0;
        while (counter < 1)
        {
            TextLogic(initialColor, Color.clear, ref counter);
            yield return null;
        }

        textDisplay.color = Color.clear;
        textDisplay.gameObject.SetActive(false);

        float counter2 = 0;
        while (counter2 < 1)
        {
            BoxLogic(initialScale, closedScale, ref counter2);
            yield return null;
        }
        textBacking.transform.localScale = new Vector3(0, initialScale.y, initialScale.z);
        boxActive = false;

        yield break;
    }
}
