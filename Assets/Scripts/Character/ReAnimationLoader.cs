using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReAnimationLoader : MonoBehaviour
{
    public GlobalCharacterMaterials GCM;
    public GlobalVariables GV;
    public Material[] skin;
    public Material[] scrubs;
    public Material[] hair;
    public Material[] glasses;
    public Material[] labcoat;
    // Start is called before the first frame update
    //to do list

    //right facing ponytail. 
    //Everything i did for scrubs, hair.
    public void StartLoading()
    {
        int id;
        //ADD HERE WHATEVER YOU NEED TO GRAB CHARACTER ID
         if (Application.platform != RuntimePlatform.WebGLPlayer){
            Debug.Log("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");
            //FireBase tmp = GetComponent<FireBase>();
            //tmp.enabled = false;
            //return;
            id = 1243211;//GV.getCharacterID();
        }
        else {
            id = PlayerSaveData.Instance.GetPlayerCustomization();
        }
        string ids = "" + id;
        char[] charID = ids.ToCharArray();
        string converter = charID[0] + "";
        int sexID = Int32.Parse(converter);
        converter = charID[3] + "";
        int skinID = Int32.Parse(converter);
        skin = GCM.getSkinMaterial(sexID, skinID);
        converter = charID[1] + "";
        int hairID = Int32.Parse(converter);
        converter = charID[2] + "";
        int hairColorID = Int32.Parse(converter);
        hair = GCM.getHair(hairID, hairColorID);
        converter = charID[6] + "";
        int glassesID = Int32.Parse(converter);
        glasses = GCM.getGlasses(glassesID);
        converter = charID[5] + "";
        int labcoatID = Int32.Parse(converter);
        labcoat = GCM.getLabcoat(labcoatID);
        converter = charID[4] + "";
        int scrubsColor = Int32.Parse(converter);
        scrubs = GCM.getScrubs(scrubsColor);
    }


    public Material[] getScrubsLeftAnimationFrames()
    {
        Material[] formattedScrubs = new Material[4];
        //format the scrubs to be good for the animation
        formattedScrubs[0] = scrubs[1];
        formattedScrubs[1] = scrubs[0];
        formattedScrubs[2] = scrubs[2];
        formattedScrubs[3] = scrubs[0];
        return formattedScrubs;
    }
    public Material[] getScrubsRightAnimationFrames()
    {
        Material[] formattedScrubs = new Material[4];
        formattedScrubs[0] = scrubs[4];
        formattedScrubs[1] = scrubs[3];
        formattedScrubs[2] = scrubs[5];
        formattedScrubs[3] = scrubs[3];
        return formattedScrubs;
    }
    public Material[] getHairFrames()
    {
        return hair;
    }
    public Material[] getSkinFrames()
    {
        return skin;
    }
    public Material[] getLabcoat()
    {
        return labcoat;

    }
    public Material[] getGlasses()
    {
        return glasses;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
