using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    static private FollowCam S;
    static public GameObject POI; //static point of interest

    public enum eView {none, slingshot, castle, both}
    
    [Header("Inscribed")] 
    public float easing = 0.5f;

    public Vector2 minXY = Vector2.zero; //vector2.zero is [0,0]

    public GameObject viewBothGO;
    
    [Header("Dynamic")] 
    public float camZ; //desired z pos of the camera

    public eView nextView = eView.slingshot;

    void Awake()
    {
        S = this;
        camZ = this.transform.position.z;
    }

    private void FixedUpdate()
    {
        
        //if (POI == null) return; // is no poi, then return
        //position of the POI
        //Vector3 destination = POI.transform.position;
        //Limit the minimum values of destination.x and destiniation y
        Vector3 destination = Vector3.zero;
        if (POI != null)
        {
            //if poi has a rigid body, check to see sleeping status
            Rigidbody poiRigid = POI.GetComponent<Rigidbody>();
            if ((poiRigid != null) && poiRigid.IsSleeping())
            {
                POI = null;
            }
        }

        if (POI != null)
        {
            destination = POI.transform.position;
        }
        
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y); 
        //interpolate from current Camera position to destination
        destination = Vector3.Lerp(transform.position, destination, easing);
        //Force destination.z to be camZ to keep cam far away
        destination.z = camZ;
        //set the camera to the destination
        transform.position = destination;
        //Set orthographicsize of the camera to keep Ground in view
        Camera.main.orthographicSize = destination.y + 10;
    }
    
     public void SwitchView (eView newView)
        {
            if (newView == eView.none)
            {
                newView = nextView;
            }

            switch (newView)
            {
                case eView.slingshot:
                    POI = null;
                    nextView = eView.castle;
                    break;
                case eView.castle:
                    POI = MissionDemolition.GET_CASTLE();
                    nextView = eView.both;
                    break;
                case eView.both:
                    POI = viewBothGO;
                    nextView = eView.slingshot;
                    break;
            }
        }

        public void SwitchView()
        {
            SwitchView(eView.none);
        }

        static public void SWITCH_VIEW(eView newView)
        {
            S.SwitchView(newView);
        }
    }
