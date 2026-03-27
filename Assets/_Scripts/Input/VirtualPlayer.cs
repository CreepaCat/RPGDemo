using System;
using UnityEngine;

public class VirtualPlayer : MonoBehaviour
{
    [SerializeField] InputReader inputReader;
    public InputReader InputReader => inputReader;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputReader.EnablePlayerActions();
    }

}
