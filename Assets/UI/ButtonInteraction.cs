using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI label;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color hoverColor;

    private void Start()
    {
        normalColor = new Color(0.2f, 1f, 0.2f);
        hoverColor = Color.white;
        label.color = normalColor;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            label.color = normalColor;
            transform.localScale = Vector3.one;
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        label.color = hoverColor;
        transform.localScale = Vector3.one * 1.05f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        label.color = normalColor;
        transform.localScale = Vector3.one;
    }
}