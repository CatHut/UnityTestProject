using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TextSampleComponent : MonoBehaviour
{
    public TextMeshPro m_TextMeshPro;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_TextMeshPro.text = MasterData.Instance.EnemyData.EnemyParameterData.First().Value.IMAGE;
    }
}