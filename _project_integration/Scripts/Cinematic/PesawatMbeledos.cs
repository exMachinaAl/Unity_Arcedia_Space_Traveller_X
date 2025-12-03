using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PesawatMbeledos : MonoBehaviour
{
    public Transform victim;
    public Transform lookAtTarget;
    public PlayableDirector director;


    void Start()
    {
        victim = Manager_Player.Instance.humanCtrl.transform;

        // director.Play();
    }


    public void LookAtTarget()
    {
        // victim.LookAt(lookAtTarget);
        Logger.LogNormal("Cinematic | pesawatMbeledos", "masuk ke pesawat dengan cutscene");

        Manager_Player.Instance.EnterShip(lookAtTarget.GetComponent<FlightControllerV1>());

        GameObject nearestPlanet = FindObjectOfType<Game_PlanetGenerationDataV2>().transform.gameObject;

        Manager_Player.Instance.flightCtrl.transform.position = nearestPlanet.transform.position + new Vector3(0, 100, 0);

        Manager_Player.Instance.flightCtrl.shipTransform.LookAt(nearestPlanet.transform);
    }

    public void PesawatJalanSendiri()
    {
        Rigidbody rb = Manager_Player.Instance.flightCtrl.transform.GetComponent<Rigidbody>();
        int speed = 25;

        // // Gerakkan pesawat
        Vector3 move = transform.forward * 1 * speed;
        rb.velocity = move;
    }
}
