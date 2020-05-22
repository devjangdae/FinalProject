using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAnim
{

    public AnimationClip idle;
    public AnimationClip run;
    public AnimationClip jump;

}
public class CharacterController : MonoBehaviour
{

    private float h = 0.0f;
    private float v = 0.0f;
    private float r = 0.0f;

    private Transform tr;

    public float rotSpeed = 80.0f;
    public float moveSpeed = 10.0f;

    public PlayerAnim playerAnim;

    [HideInInspector]
    public Animation anim;


    // Start is called before the first frame update
    void Start()
    {

        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();
        anim.clip = playerAnim.idle;
        anim.Play();

        
    }

    // Update is called once per frame
    void Update()
    {

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        Debug.Log("H=" + h.ToString());
        Debug.Log("V=" + v.ToString());

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        float vec = Vector3.Magnitude(moveDir.normalized);

        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);

        Debug.Log("델타타임 : " + Time.deltaTime + "");

        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);

        if (v >= 0.1f)
        {
            anim.CrossFade(playerAnim.run.name, 0.3f);
        }
        else if(v <= -0.01f)
        {
            anim.CrossFade(playerAnim.run.name, 0.3f);

        }
        else
        {
            anim.CrossFade(playerAnim.idle.name, 0.3f);
        }
        /*
        else if (h >= 0.01f)
        {
            anim.CrossFade(playerAnim.runR.name, 0.3f);

        }
        else if (h <= -0.01f)
        {
            anim.CrossFade(playerAnim.runL.name, 0.3f);

        }*/



    }
}
