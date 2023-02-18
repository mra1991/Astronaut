using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBehavior : MonoBehaviour
{
    [Tooltip("Where is the end of the level? If the crystal gets to this point the level is won.")]
    [SerializeField] private Transform endPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private void FixedUpdate()
    {
        if (gameObject.transform.position.x >= endPosition.position.x) //if the crystal has made it far enough
        {
            GameManager.instance.LevelUp(); //signal GameManager that the level was successful
            Destroy(gameObject);
        }
    }
}
