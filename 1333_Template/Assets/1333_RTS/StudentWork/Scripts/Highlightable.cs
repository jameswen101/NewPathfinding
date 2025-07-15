using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlightable : MonoBehaviour
{
    [SerializeField] private GameObject highlightVisual;

    public void SetHighlight(bool on)
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(on);
    }
}
