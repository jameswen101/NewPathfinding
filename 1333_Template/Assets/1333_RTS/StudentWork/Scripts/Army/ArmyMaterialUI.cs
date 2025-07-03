using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyMaterialUI : MonoBehaviour
{
    [SerializeField] private RectTransform layoutGroupParent;
    [SerializeField] private ArmyMaterialButton ButtonPrefab;
    [SerializeField] private TeamMaterialsCollection materialsCollection;
    [SerializeField] private ArmyMaterialSelector armyMaterialSelector;

    // Start is called before the first frame update
    void Start()
    {
        foreach (TeamMaterialInfo t in materialsCollection.materials)
        {
            Debug.Log($"Spawning button for {t.name}");
            ArmyMaterialButton button = Instantiate(ButtonPrefab, layoutGroupParent);
            button.Setup(t, armyMaterialSelector);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
