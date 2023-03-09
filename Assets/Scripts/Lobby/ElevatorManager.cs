using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    [SerializeField] private Animator outerDoor;
    [SerializeField] private Animator innerDoor;
    [SerializeField] private GameObject invisibleWall;

    [NonSerialized]
    public bool doorsClosed;

    // Declare and initialize a new List of GameObjects called currentCollisions.
    List <GameObject> currentCollisions = new();
     
    void OnTriggerEnter (Collider col) {
 
        // Add the GameObject collided with to the list.
        currentCollisions.Add (col.gameObject);
    }
 
    void OnTriggerExit (Collider col) {
 
        // Remove the GameObject collided with from the list.
        currentCollisions.Remove (col.gameObject);
    }
    
    public List<GameObject> GetPlayersInElevator()
    {
        return currentCollisions.FindAll(x => x.CompareTag("Player"));
    }

    public IEnumerator CloseDoors()
    {
        // Enable Invisible Wall
        invisibleWall.SetActive(true);
        
        // Close inner door
        innerDoor.SetTrigger("Close");

        yield return new WaitForSecondsRealtime(0.5f);
        
        // Close outer door
        outerDoor.SetTrigger("Close");
        
        yield return new WaitForSecondsRealtime(2);
        
        // Disable Invisible Wall
        invisibleWall.SetActive(false);

        doorsClosed = true;
    }

    public IEnumerator OpenDoors()
    {
        doorsClosed = false;
        
        // Open outer door
        outerDoor.SetTrigger("Open");

        yield return new WaitForSecondsRealtime(0.5f);
        
        // Open inner door
        innerDoor.SetTrigger("Open");
    }

    public IEnumerator MoveDown()
    {
        if (doorsClosed)
        {
            
        }
        else
        {
            throw new InvalidAsynchronousStateException("Cannot move the elevator while the doors are open. Safety First!");
        }

        return null;
    }

    public IEnumerator MoveUp()
    {
        if (doorsClosed)
        {
            
        }
        else
        {
            throw new InvalidAsynchronousStateException("Cannot move the elevator while the doors are open. Safety First!");
        }

        return null;
    }
}
