using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartTrigger : MonoBehaviour {

    public enum TriggerType
    {
        From,
        To
    }
    public TriggerType tType;
    public Transform parent;
    private static Transform aim;
    private float startScale;
    private LineRenderer renderer;
    public GameObject LineRendererPrefab;

    private void Start()
    {
        startScale = transform.localScale.y;
        gameObject.AddComponent<Billboard>();
        BoxCollider bc = gameObject.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(0.3f, 0.3f, 0.01f);
    }

    public void OnMouseEnter()
    {
        if(tType == TriggerType.To) aim = parent;
        transform.localScale = Vector3.one * startScale * 1.2f;
    }

    public void OnMouseExit()
    {
        transform.localScale = Vector3.one * startScale;
    }

    public void OnMouseDrag()
    {
        if (tType == TriggerType.From)
        {
            if (renderer)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 50))
                {
                    renderer.SetPosition(1, hit.point);
                }
            }
            else
            {
                renderer = Instantiate(LineRendererPrefab, transform.position, transform.rotation, transform).GetComponent<LineRenderer>();
                renderer.SetPosition(0, transform.position);
            }
        }
        OnMouseExit();
    }

    public void OnMouseUp()
    {
        if (tType == TriggerType.From)
        {
            FindObjectOfType<IKController>().MoveTo(parent, aim);
        }
        if (renderer)
        {
            Destroy(renderer.gameObject);
        }
    }

}
