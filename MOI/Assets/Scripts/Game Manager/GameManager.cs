using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adding this lets you edit the settings in the Unity Inspector
[System.Serializable]
public class GameManager : MonoBehaviour
{
    [Header("Camera Controls")]
    public KeyCode CameraUp = KeyCode.UpArrow;
    public KeyCode CameraDown = KeyCode.DownArrow;

    [Header("Player Controls")]
    public KeyCode MovePlayerLeft = KeyCode.A;
    public KeyCode MovePlayerRight = KeyCode.D;
    public KeyCode PlayerJump = KeyCode.Space;

    public static GameManager Instance { get; private set; }

    // Dictionary to remember the current smoothed value for each key combination
    private Dictionary<string, float> axisStates = new Dictionary<string, float>();

    private void Awake()
    {
        // Standard Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keeps GameManager alive between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Gets a smoothed axis value for any two keys. 
    /// </summary>
    public float GetCustomAxis(KeyCode posKey, KeyCode negKey, float sensitivity = 3f, float gravity = 3f)
    {
        // Create a unique text ID for this combination (e.g., "UpArrow_DownArrow")
        string axisID = $"{posKey}_{negKey}";

        // If this is the first time a script asked for these keys, add them to the dictionary at 0
        if (!axisStates.ContainsKey(axisID))
        {
            axisStates.Add(axisID, 0f);
        }

        // Retrieve the current value from memory
        float currentValue = axisStates[axisID];
        float target = 0f;

        // Calculate the raw target
        if (Input.GetKey(posKey)) target += 1f;
        if (Input.GetKey(negKey)) target -= 1f;

        // Apply smoothing
        if (target != 0f)
            currentValue = Mathf.MoveTowards(currentValue, target, sensitivity * Time.deltaTime);
        else
            currentValue = Mathf.MoveTowards(currentValue, 0f, gravity * Time.deltaTime);

        // Save the updated value back into the dictionary for the next frame
        axisStates[axisID] = currentValue;

        return currentValue;
    }
}
