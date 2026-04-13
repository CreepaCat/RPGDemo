using System;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{

    private static MinimapManager _instance = null;

    public static MinimapManager Instance => _instance;
    public MinimapUI uimp;

    [SerializeField] Collider minimapBondingBox = null;
    public Collider MinimapBondingBox => minimapBondingBox;
    public Transform PlayerTransform => Player.GetInstance().transform;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

    }
    private void Start()
    {
        UpdateMinimap(minimapBondingBox);


    }

    internal Sprite LoadMinimap()
    {
        //todo:使用renderTexture
        return null;
        //throw new NotImplementedException();
    }

    public void UpdateMinimap(Collider minimapBondingBox)
    {
        this.minimapBondingBox = minimapBondingBox;
        if (uimp != null)
            uimp.UpdateMap();//链式调用

    }
}
