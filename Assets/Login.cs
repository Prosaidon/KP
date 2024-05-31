using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    private FirebaseController firebaseController;

    private void Start()
    {
        firebaseController = FirebaseController.instance;
        if (firebaseController == null)
        {
            Debug.LogError("FirebaseController not found in the scene.");
        }
    }

    public void FirebaseAuth()
    {
        if (firebaseController != null)
        {
            Debug.Log("FirebaseAuth");
            firebaseController.OpenProfilePanel();
        }
        else
        {
            Debug.LogError("FirebaseController is null. Cannot call OpenProfilePanel.");
        }
    }
}

