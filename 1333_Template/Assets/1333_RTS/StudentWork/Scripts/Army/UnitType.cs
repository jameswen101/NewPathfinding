using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UnitType", menuName = "UnitType")]

public class UnitType : ScriptableObject
{
    [SerializeField] private int width = 1;
    [SerializeField] private int height = 1;
    [SerializeField] private int maxHp = 1;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private int damage = 1;
    [SerializeField] private int defence = 1;
    public AttackType attackType = 0;
    [SerializeField] private int range = 1;

    public GameObject unitPrefab;

    public int Width => width;
    public int Height => height;
    public int MaxHp => maxHp;
    public float MoveSpeed => moveSpeed;
    public int Damage => damage;
    public int Defence => defence;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
