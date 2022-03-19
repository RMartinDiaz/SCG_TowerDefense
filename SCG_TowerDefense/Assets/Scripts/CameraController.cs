using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //When the right mouse click is pressed, we start the coroutine which moves the camera depending on mouse movement.
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(CameraMouseMovement());
        }

        //If the mouse wheel is scrolling we change the camera's FOV.

        if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
        {
            Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
        }
    }

    IEnumerator CameraMouseMovement()
    {
        Vector3 startingCameraPosition = this.transform.position;
        Vector3 startingMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;

        while (Input.GetMouseButton(1))
        {
            Vector3 correctMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;

            //Debug.Log(startingMousePosition + " ____ " + correctMousePosition);

            this.transform.position = startingCameraPosition - correctMousePosition + startingMousePosition;

            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}
