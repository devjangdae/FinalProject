using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RayInteraction : MonoBehaviour
{
    public LayerMask whatIsTarget;
    private Camera playerCam;
    public float distance = 3f;

    public GameObject[] NPC_List;
    public GameObject[] NPCinteraction_List;

    //public GameObject NPC;

    //public GameObject NPCInfo;
    //public GameObject NPCinteraction1;

    //public GameObject NPC2;
    //public GameObject NPCInfo2;
    //public GameObject NPCinteraction2;
    


    /*
    public GameObject[] arrNPC;
    public GameObject[] arrNPCinteraction;
    */
    private Transform moveTarget;


    private string[] npcTag;
    private float targetDistance;
    private bool npcbool= false;
    private bool npcbool2= false;
    private RaycastHit hit;


    // Start is called before the first frame update
    void Start()
    {
        playerCam = Camera.main;

        //NPC_List = new GameObject[NPC_List.Length];
        //NPCinteraction_List = new GameObject[NPCinteraction_List.Length];
        //npcTag = new string[NPC_List.Length];
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 rayOrigin = playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        Vector3 rayDir = playerCam.transform.forward;

        Ray ray = new Ray(rayOrigin, rayDir);

        RaycastHit hit;



        //Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);
        Debug.DrawRay(transform.position + new Vector3(0, 1, 0), transform.forward * distance, Color.red);

        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, out hit, distance))
        {
            if (hit.collider.CompareTag("npc"))
            {
                NPC_List[0].transform.LookAt(this.transform);

                //npcbool = true;
                NPCinteraction_List[0].SetActive(true);
            }

        }
        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, out hit, distance))
        {

            if (hit.collider.tag == "npc2")
            {

                NPC_List[1].transform.LookAt(this.transform);

                //npcbool2 = true;
                NPCinteraction_List[1].SetActive(true);

            }
        }


            /*
            if (hit.collider.tag == "npc2")
            {

                NPC2.transform.LookAt(this.transform);

                if (Input.GetMouseButton(0))
                {
                    npcbool2 = true;
                    NPCInfo2.SetActive(false);
                    NPCinteraction2.SetActive(true);


                }
                else if (npcbool2 == false)
                {
                    NPCInfo2.SetActive(true);
                }
            }
            else if(hit.collider.tag != "npc2")
            {
                NPCInfo2.SetActive(false);
                if(Input.GetMouseButton(0) && npcbool2 == true)
                {
                    NPCinteraction2.SetActive(false);
                    npcbool2 = false;

                }
            }


        }
        */
            /*if (Input.GetMouseButton(1))
            {

                if (Physics.Raycast(ray,out hit,distance, whatIsTarget))
                {
                    GameObject hitTarget = hit.collider.gameObject;

                    hitTarget.GetComponent<Renderer>().material.color = Color.red;


                    moveTarget = hitTarget.transform;
                    targetDistance = hit.distance;
                    //Debug.Log(hit.collider.gameObject.name);
                    //Debug.Log("뭔가에 광선이 걸렸다.");
                }
            }

            if (Input.GetMouseButton(1))
            {
                if(moveTarget != null)
                {
                    moveTarget.GetComponent<Renderer>().material.color = Color.white;
                }
                moveTarget = null;
            }

            if(moveTarget != null)
            {
                moveTarget.position = ray.origin + ray.direction * targetDistance;
            }
            */

        

    }
}
