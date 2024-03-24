using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Descriptions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Dropdown;
    // Start is called before the first frame update
    void Start()
    {
        Dropdown.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Dropdown.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Dropdown.SetActive(false);
    }
}
