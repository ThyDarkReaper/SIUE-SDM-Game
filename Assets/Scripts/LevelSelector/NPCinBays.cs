//This script is used in LevelSelector floors to track level progression for users to see when they have completed a level
//Students are graded the first time they play the level but are able to play over and over
//On start the users can see the NPCs next to their bays, when false and NPC not displayed the player completed that level
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCinBays : MonoBehaviour
{
    void Start()
    {
        //Grab the player experience that is locally saved
        float[] experience = null;//PlayerSaveData.Instance.GetPlayerExperience();

        if (experience == null)
        {
            //Debug.LogWarning("Player EXP NULL");
            return;
        }

        for (int i = 0; i < experience.Length; i++)
        {
            //Go through the array and check if the array is not equal to 0
            if (experience[i] != 0f)
            {
                string npcName = "NPC" + i; //Finds the gameobject NPC that has to have a number next to it indicating the Level (ex: Leve1; NPC1)
                GameObject npc = GameObject.Find(npcName);//Finds the gameobject

                if (npc != null)
                {
                    npc.SetActive(false); //If exp is not equal to 0 sets the object as false
                }
                else
                {
                    Debug.LogWarning("No NPC");
                }
            }
        }
    }
}
