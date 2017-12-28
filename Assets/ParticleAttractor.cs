using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAttractor : MonoBehaviour {

    public float InterpSpeed = 3f;

    public float StartTime = 0.7f;

    public ParticleSystem MyParticleSystem;

    public Transform PointToAttract;

    private ParticleSystem.Particle[] MyPar;

    bool isLeft = false;
    

    // Use this for initialization
    void Start () {
        //GetComponent<Rigidbody>().velocity = Vector3.up * 0.2f;
        Destroy(gameObject, 1f);
	}
	
	// Update is called once per frame
	void Update () {


        if(StartTime > 0)
        {
            StartTime -= Time.deltaTime;
            return;
        }

        transform.position += Vector3.right * (isLeft ? -0.001f : 0.001f);
        isLeft = !isLeft;

        if (PointToAttract == null)
            Destroy(gameObject);

        int ParCount = MyParticleSystem.particleCount;

        MyPar = new ParticleSystem.Particle[ParCount];

        MyParticleSystem.GetParticles(MyPar);

        

        for (int i = 0; i < ParCount; ++i)
        {
            //MyPar[i].position = Vector3.Lerp(MyPar[i].position, PointToAttract.position, Time.deltaTime * InterpSpeed);

            //float Speed = Mathf.Clamp(InterpSpeed * 10 / (MyPar[i].position - PointToAttract.position).magnitude, 30, 80);

            MyPar[i].velocity = (MyPar[i].velocity / 1.5f) + (PointToAttract.position - MyPar[i].position).normalized * InterpSpeed;

            //MyPar[i].position = Vector3.MoveTowards(MyPar[i].position, PointToAttract.position, Time.deltaTime * Speed);
        }

        
        MyParticleSystem.SetParticles(MyPar, MyPar.Length);
	}
}
