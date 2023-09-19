using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PaperScrap : MonoBehaviour
{
    
    AllManager allmng;
    public int worth;
    public Transform paperScrapsUiPosition;
    public float collectTime;

    void Start()
    {
        paperScrapsUiPosition = GameObject.Find("PaperScrapsUI").transform;
        allmng = GameObject.Find("AllManager").GetComponent<AllManager>();
    }

    void Update()
    {
        
    }

    IEnumerator CollectScrap(){
        GetComponent<AudioSource>().Play();
        float startTime = Time.time;
        float elapsedTime = 0f;
        Vector2 startPosition = (Vector2)transform.position;

        while(Time.time - startTime < collectTime){
            float t = elapsedTime / collectTime;
            t = EaseFunctions.easeInCubic(t);
            transform.position = Vector2.Lerp(startPosition, (Vector2)paperScrapsUiPosition.position, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = paperScrapsUiPosition.position;
        Destroy(this.gameObject);
    }

    public void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Lara")){
            allmng.CollectPaperScrap (worth);
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine(CollectScrap());
        }
        else{
            // do nothing
        }
    }
}
