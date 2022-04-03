using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelManager : MonoBehaviour
{
    GameManager gameManager;
    BattleManager battleManager;
    AudioSource audioSource;

    [SerializeField] MapData mapData;
    [SerializeField] Vector3 defaultSpawnLocation;
    [SerializeField] AudioClip loadingMusic;
    [SerializeField] AudioClip backgroundMusic;
    [SerializeField] AudioClip battleMusic;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        battleManager = BattleManager.Instance;

        audioSource = gameObject.GetComponent<AudioSource>();
        BindEvents();
        if(loadingMusic != null)
        {
            audioSource.loop = true;
            audioSource.clip = loadingMusic;
            audioSource.Play();
        }
    }

    IEnumerator InitPartyLeader()
    {
        yield return null;
        MapManager.Instance.SetMapData(mapData);
        yield return null;
        GameObject playerObj = gameManager.SpawnUnit(GameManager.Instance.party[0], defaultSpawnLocation, true, true);
        yield return null;
        Camera.main.GetComponent<CameraController>().SetFocus(playerObj);

        if(backgroundMusic != null)
        {
            audioSource.loop = true;
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(defaultSpawnLocation, .5f);
        Handles.Label(defaultSpawnLocation, "Default Spawn");
    }

    void OnGameManagerLoaded()
    {
        StartCoroutine(InitPartyLeader());
    }

    void OnEncounterAwake()
    {
        if(battleMusic != null)
        {
            audioSource.loop = true;
            audioSource.clip = battleMusic;
            audioSource.Play();
        }
    }

    void OnEncounterEnd()
    {
        if(backgroundMusic != null)
        {
            audioSource.loop = true;
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }
    }

    void BindEvents()
    {
        gameManager.OnGameManagerLoaded += OnGameManagerLoaded;
        battleManager.OnEncounterAwake += OnEncounterAwake;
        battleManager.OnEncounterEnd += OnEncounterEnd;
    }

    void UnbindEvents()
    {
        gameManager.OnGameManagerLoaded -= OnGameManagerLoaded;
        battleManager.OnEncounterAwake -= OnEncounterAwake;
        battleManager.OnEncounterEnd -= OnEncounterEnd;
    }

    void OnDestroy()
    {
        UnbindEvents();
    }
}
