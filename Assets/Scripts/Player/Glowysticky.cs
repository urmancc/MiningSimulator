using UnityEngine;
using System.Collections;

public class GlowStickMaster : MonoBehaviour
{
    [Header("Shake Logic")]
    private int targetShakes;
    private int currentShakes = 0;
    public float moveDistance = 0.1f;
    private Vector3 originalLocalPos;

    [Header("Lighting")]
    public Light stickLight;

    [Header("Throwing & Trajectory")]
    public LineRenderer trajectoryLine;
    public GameObject glowStickPrefab;
    public Transform throwPoint; 
    public float throwForce = 15f;

    [Header("Camera Shake")]
    public float camShakeDuration = 0.1f;
    public float camShakeAmount = 0.05f;

    void Start()
    {
        originalLocalPos = transform.localPosition;
        // Pick random number between 5 and 10
        targetShakes = Random.Range(5, 11);
        
        if(stickLight != null) stickLight.enabled = false;
        if(trajectoryLine != null) trajectoryLine.enabled = false;
    }

    void Update()
    {
        // 1. Shaking Logic (F Key)
        if (Input.GetKeyDown(KeyCode.F))
        {
            currentShakes++;
            HandleStickVisualShake();

            if (currentShakes == targetShakes)
            {
                if(stickLight != null) stickLight.enabled = true;
                Debug.Log("You got light!");
            }
        }

        // 2. Trajectory Logic (Hold T)
        if (Input.GetKey(KeyCode.T))
        {
            trajectoryLine.enabled = true;
            UpdateTrajectory();
        }

        // 3. Throw Logic (Release T)
        if (Input.GetKeyUp(KeyCode.T))
        {
            ThrowStick();
            trajectoryLine.enabled = false;
        }
    }

    void HandleStickVisualShake()
    {
        // Simple up/down toggle relative to home position
        if (currentShakes % 2 != 0)
            transform.localPosition = originalLocalPos + new Vector3(0, moveDistance, 0);
        else
            transform.localPosition = originalLocalPos;
            
        StartCoroutine(ShakeCamera());
    }

    void UpdateTrajectory()
    {
        Vector3 startPos = throwPoint.position;
        Vector3 startVelocity = Camera.main.transform.forward * throwForce;
        int points = 20;
        trajectoryLine.positionCount = points;

        for (int i = 0; i < points; i++)
        {
            float time = i * 0.1f;
            Vector3 pos = startPos + startVelocity * time + 0.5f * Physics.gravity * time * time;
            trajectoryLine.SetPosition(i, pos);
        }
    }

    void ThrowStick()
    {
        GameObject obj = Instantiate(glowStickPrefab, throwPoint.position, throwPoint.rotation);
        obj.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        
        // Transfer the light state to the thrown object
        Light l = obj.GetComponentInChildren<Light>();
        if(l != null) l.enabled = stickLight.enabled;
    }

    IEnumerator ShakeCamera()
    {
        Vector3 camOriginalPos = Camera.main.transform.localPosition;
        float elapsed = 0f;
        while (elapsed < camShakeDuration)
        {
            float x = Random.Range(-1f, 1f) * camShakeAmount;
            float y = Random.Range(-1f, 1f) * camShakeAmount;
            Camera.main.transform.localPosition = camOriginalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.localPosition = camOriginalPos;
    }
}
