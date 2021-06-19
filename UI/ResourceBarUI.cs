using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceBarUI : MonoBehaviour
{
    [SerializeField, Range(0f, 1600f)]
    float maxBarWidth = 270f;
    [SerializeField]
    GameObject barImage;
    [SerializeField]
    TextMeshProUGUI resourceAmountText;
    RectTransform barRect;

    void Awake()
    {
        barRect = barImage.GetComponent<RectTransform>();
    }

    public void UpdateResource(float curAmount, float maxAmount)
    {
        float barWidth = maxBarWidth * (curAmount / maxAmount) * 1f;
        barRect.sizeDelta = new Vector2(barWidth, barRect.sizeDelta.y);
        resourceAmountText.text = Mathf.RoundToInt(curAmount).ToString() + "/" + Mathf.RoundToInt(maxAmount).ToString();
    }
}
