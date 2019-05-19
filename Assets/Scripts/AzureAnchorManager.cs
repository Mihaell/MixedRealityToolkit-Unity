using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.WSA;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System.Threading.Tasks;
//using Microsoft.Azure.SpatialAnchors;
using System.Linq;

public class AzureAnchorManager : BaseInputHandler, IMixedRealitySpeechHandler
{
    // private CloudSpatialAnchorSession cloudSpatialAnchorSession = null;
    public Material cubeMaterial;

    public Material relocalizedCubeMaterial;

    public string accountId;

    public string accountKey;

    private List<GameObject> cubes;

    //private CloudSpatialAnchorSession cloudAnchorSession;

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        Debug.Log(eventData.Command.Keyword);
        Debug.Log(eventData.Confidence.ToString());
        switch (eventData.Command.Keyword)
        {
            case "session":

                Task.Run(OnCreateSessionAsync);
                break;

            case "cube":
                Task.Run(OnCreateCubeAsync);
                break;

            case "clear":
                Task.Run(OnClearCubesAsync);
                break;

            case "reload":
                Task.Run(OnReloadCubesAsync);
                break;

            default:
                break;
        }
    }

    async Task OnCreateSessionAsync()
    {
        Debug.Log("Create session async called");
        /*if (this.cloudAnchorSession == null)
        {
            Debug.Log("Starting session");
            this.cloudAnchorSession = new CloudSpatialAnchorSession();
            this.cloudAnchorSession.Configuration.AccountId = accountId;
            this.cloudAnchorSession.Configuration.AccountKey = accountKey;

            this.cloudAnchorSession.Error += OnError;
            this.cloudAnchorSession.AnchorLocated += OnAnchorLocated;
            this.cloudAnchorSession.LocateAnchorsCompleted += OnLocateAnchorsCompleted;

            this.cloudAnchorSession.Start();

            Debug.Log("Session started");
        }*/
    }

    /*void OnError(object sender, SessionErrorEventArgs args)
    {
        Debug.Log("Greska " + args.ToString());
    }

    void OnAnchorLocated(object sender, AnchorLocatedEventArgs args)
    {
        UnityEngine.WSA.Application.InvokeOnAppThread(
            () =>
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                cube.GetComponent<Renderer>().material = this.relocalizedCubeMaterial;

                var worldAnchor = cube.AddComponent<WorldAnchor>();

                worldAnchor.SetNativeSpatialAnchorPtr(args.Anchor.LocalAnchor);

                cube.name = args.Identifier;

                Debug.Log("Anchor located");
            },
            false
        );
    }

    void OnLocateAnchorsCompleted(object sender, LocateAnchorsCompletedEventArgs args)
    {
        Debug.Log("Anchor location completed");
        args.Watcher.Stop();
    }*/

    async Task OnCreateCubeAsync()
    {
        Debug.Log("Creating cube async");
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        cube.transform.position = MixedRealityToolkit.InputSystem.GazeProvider.HitPosition;
        cube.GetComponent<Renderer>().material = this.cubeMaterial;
        this.cubes.Add(cube);

        var worldAnchor = cube.AddComponent<WorldAnchor>();

        //var cloudSpatialAnchor = new CloudSpatialAnchor(
        //    worldAnchor.GetNativeSpatialAnchorPtr(), false);

        await this.WaitForSessionReadyToCreateAsync();

        Debug.Log("Session ready, creating anchor");

        //await this.cloudAnchorSession.CreateAnchorAsync(cloudSpatialAnchor);

        //cube.name = cloudSpatialAnchor.Identifier;
        Debug.Log("Cube created async");
    }

    async Task OnReloadCubesAsync()
    {
        Debug.Log("Reloading cubes");
        if (this.cubes.Count > 0)
        {
            var identifiers = this.cubes.Select(c => c.name).ToArray();

            await this.OnClearCubesAsync();

            /*var watcher = this.cloudAnchorSession.CreateWatcher(
                new AnchorLocateCriteria()
                {
                    Identifiers = identifiers,
                    BypassCache = true,
                    RequestedCategories = AnchorDataCategory.Spatial,
                    Strategy = LocateStrategy.AnyStrategy
                }
            );*/
        }
    }

    async Task OnClearCubesAsync()
    {
        foreach (var cube in this.cubes)
        {
            Destroy(cube);
        }
        this.cubes.Clear();
    }

    async Task WaitForSessionReadyToCreateAsync()
    {
        while (true)
        {
            Debug.Log("Waiting for session ready");
            //var status = await this.cloudAnchorSession.GetSessionStatusAsync();

            //if (status.ReadyForCreateProgress >= 1.0f)
            //{
                break;
            //}
            await Task.Delay(250);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // this.cloudSpatialAnchorSession = new CloudSpatialAnchorSession();
        Debug.Log("Starting speech recognition");

        this.cubes = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
