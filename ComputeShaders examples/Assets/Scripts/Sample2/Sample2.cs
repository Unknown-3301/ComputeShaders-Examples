using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComputeShaders;

public class Sample2 : MonoBehaviour
{
    [SerializeField] GameObject particlePrefab;

    [Header("Number of particles")]
    [SerializeField] [Min(1)] int red;
    [SerializeField] [Min(1)] int green;
    [SerializeField] [Min(1)] int blue;

    //positive means attraction and negative means repulsion
    [Header("Attraction strength")]
    [SerializeField] bool randomizeOnStart;
    [Tooltip("The attraction strength between the red particles")] [SerializeField] float redToRed;
    [Tooltip("The strength of the red particles attracting the green particles")] [SerializeField] float redToGreen;
    [Tooltip("The strength of the red particles attracting the blue particles")] [SerializeField] float redToBlue;
    [Tooltip("The strength of the blue particles attracting the red particles")] [SerializeField] float blueToRed;
    [Tooltip("The strength of the blue particles attracting the green particles")] [SerializeField] float blueToGreen;
    [Tooltip("The Pulling strength between the blue particles")] [SerializeField] float blueToBlue;
    [Tooltip("The strength of the green particles attracting the red particles")] [SerializeField] float greenToRed;
    [Tooltip("The Pulling strength between the green particles")] [SerializeField] float greenToGreen;
    [Tooltip("The strength of the green particles attracting the blue particles")] [SerializeField] float greenToBlue;

    Particle[] particles;
    GameObject[] particlesObject;
    float[] interactions;

    CSDevice device;
    ComputeShaders.ComputeShader shader;
    CSStructuredBuffer<Particle> particlesBuffer;
    CSStructuredBuffer<float> particlesInteractions;
    CSCBuffer<Sample2Info> info;

    void Start()
    {
        if (randomizeOnStart)
        {
            float r = 0.2f;
            redToRed = Random.Range(-r, r);
            redToGreen = Random.Range(-r, r);
            redToBlue = Random.Range(-r, r);
            greenToRed = Random.Range(-r, r);
            greenToGreen = Random.Range(-r, r);
            greenToBlue = Random.Range(-r, r);
            blueToRed = Random.Range(-r, r);
            blueToGreen = Random.Range(-r, r);
            blueToBlue = Random.Range(-r, r);
        }

        float height = Camera.main.orthographicSize * 2;
        float width = height * 16f / 9f;

        particles = new Particle[red + blue + green];
        particlesObject = new GameObject[particles.Length];
        for (int i = 0; i < particles.Length; i++)
        {
            Color c = Color.white;
            int t = 0;
            if (i < red)
            {
                c = new Color(0.85f, 0.2f, 0.2f);
            }
            else if (i < red + green)
            {
                c = new Color(0.2f, 0.85f, 0.2f);
                t = 1;
            }
            else
            {
                c = new Color(0.2f, 0.2f, 0.85f);
                t = 2;
            }

            particles[i] = new Particle()
            {
                Position = new Vector2(Random.Range(0, width), Random.Range(0, height)),
                Color = c,
                Type = t,
            };

            GameObject p = Instantiate(particlePrefab, particles[i].Position, Quaternion.identity);
            p.GetComponent<SpriteRenderer>().color = c;
            particlesObject[i] = p;
        }

        interactions = new float[9];
        SetForces();

        device = new CSDevice();
        shader = device.CreateComputeShader(@"Assets\Scripts\Sample2\SimulatorShader.compute", "Simulate");
        device.SetComputeShader(shader);

        particlesBuffer = device.CreateStructuredBuffer(particles, Particle.Size);
        particlesBuffer.EnableCPU_Raw_ReadWrite();
        particlesInteractions = device.CreateStructuredBuffer(interactions, sizeof(float));
        info = device.CreateBuffer(new Sample2Info() { Particles = particles.Length, Width = width, Height = height }, Sample2Info.Size);

        device.SetRWStructuredBuffer(particlesBuffer, 0);
        device.SetRWStructuredBuffer(particlesInteractions, 1);
        device.SetBuffer(info, 0);
    }

    void SetForces()
    {
        interactions[0] = redToRed;
        interactions[1] = redToGreen;
        interactions[2] = redToBlue;
        interactions[3] = greenToRed;
        interactions[4] = greenToGreen;
        interactions[5] = greenToBlue;
        interactions[6] = blueToRed;
        interactions[7] = blueToGreen;
        interactions[8] = blueToBlue;
    }

    void Update()
    {
        SetForces();
        particlesInteractions.SetData(interactions);

        device.Dispatch(Mathf.CeilToInt(particles.Length / 16f), 1, 1);

        particlesBuffer.GetData(ref particles);
        for (int i = 0; i < particlesObject.Length; i++)
        {
            particlesObject[i].transform.position = particles[i].Position;
        }
    }

    private void OnDestroy()
    {
        particlesBuffer.Dispose();
        particlesInteractions.Dispose();
        info.Dispose();
        shader.Dispose();
        device.Dispose();
    }
}
