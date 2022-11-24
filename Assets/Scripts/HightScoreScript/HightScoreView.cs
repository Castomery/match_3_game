using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HightScoreView : MonoBehaviour
{
    private HightScoreManager hightScoreManager;
    [SerializeField] private RectTransform _parent;
    [SerializeField] private HightScoreTemplate _template;

    private void Awake()
    {
       hightScoreManager = new HightScoreManager();
    }

    private void Start()
    {
        List<HightScoreEntry> hightScoreEntries = hightScoreManager.HightScoreEntries;

        hightScoreEntries.Sort();
        hightScoreEntries.Reverse();

        _parent.sizeDelta = new Vector2(0, 100 * hightScoreEntries.Count);

        for (int i = 0; i < hightScoreEntries.Count; i++)
        {
            _template.UpdateData((i + 1).ToString(), hightScoreEntries[i].score.ToString(), hightScoreEntries[i].name);
            Instantiate(_template, _parent);
        }
    }
}
