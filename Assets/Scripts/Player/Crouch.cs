using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Crouch : MonoBehaviour
{
    public float crouchHeight;
    public Move player;

    public Transform headCheck;
    public Transform headCheck1;
    public float headCheckLength;

    LayerMask GroundLayer;
    private Vector2 normalHeight;
    private float yInput;


    public void Start()
    {
        GroundLayer = player.GroundLayer;
        normalHeight = transform.localScale;
    }

    private void Update()
    {
        yInput = Input.GetAxisRaw("Vertical");

        bool isHeadHitting = HeadDetect();

        if((yInput < 0 || isHeadHitting) && player.OnGround)
        {
            if(transform.localScale.y != crouchHeight)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(normalHeight.x, crouchHeight), 0.1f);
        }
        else 
        {
            if(transform.localScale.y != normalHeight.y)
            transform.localScale = Vector2.Lerp(transform.localScale, normalHeight, 0.25f);
        }
    }

    bool HeadDetect()
    {
        bool hit = Physics2D.Raycast(headCheck.position, Vector2.up, headCheckLength, GroundLayer);
        return hit;
        bool hit1 = Physics2D.Raycast(headCheck1.position, Vector2.up, headCheckLength, GroundLayer);
        return hit1;
    }

    private void OnDrawGizmos()
    {
        if (headCheck == null) return;

        Vector2 from = headCheck.position;
        Vector2 to = new Vector2(headCheck.position.x, headCheck.position.y + headCheckLength);

        if (headCheck1 == null) return;

        Vector2 from1 = headCheck1.position;
        Vector2 to1 = new Vector2(headCheck1.position.x, headCheck1.position.y + headCheckLength);

    }
}
