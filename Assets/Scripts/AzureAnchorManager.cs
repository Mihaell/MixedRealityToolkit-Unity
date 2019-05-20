using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.WSA;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System.Threading.Tasks;
using Microsoft.Azure.SpatialAnchors;
using System.Linq;
using System;

public class AzureAnchorManager : BaseInputHandler, IMixedRealitySpeechHandler
{
    private CloudSpatialAnchorSession cloudSpatialAnchorSession = null;

    public Material cubeMaterial;

    public Material relocalizedCubeMaterial;

    public string accountId;

    public string accountKey;

    public TextMesh debugText;

    private List<GameObject> cubes;

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        Debug.Log(eventData.Command.Keyword);
        Debug.Log(eventData.Confidence.ToString());
        switch (eventData.Command.Keyword)
        {
            case "session":
                OnCreateSessionAsync();
                break;

            case "cube":
                OnCreateCubeAsync();
                break;

            case "clear":
                OnClearCubesAsync();
                break;

            case "reload":
                OnReloadCubesAsync();
                break;

            default:
                break;
        }
    }

    async Task OnCreateSessionAsync()
    {
        Debug.Log("Create session async called");
        debugText.text = "Create session async called";
        if (this.cloudSpatialAnchorSession == null)
        {
            Debug.Log("Starting session");
            debugText.text = "Starting session";
            this.cloudSpatialAnchorSession = new CloudSpatialAnchorSession();
            this.cloudSpatialAnchorSession.Configuration.AccountId = accountId.Trim();
            this.cloudSpatialAnchorSession.Configuration.AccountKey = accountKey.Trim();
            cloudSpatialAnchorSession.LogLevel = SessionLogLevel.All;

            this.cloudSpatialAnchorSession.Error += OnError;
            this.cloudSpatialAnchorSession.AnchorLocated += OnAnchorLocated;
            this.cloudSpatialAnchorSession.LocateAnchorsCompleted += OnLocateAnchorsCompleted;
            this.cloudSpatialAnchorSession.SessionUpdated += OnSessionUpdated;


            this.cloudSpatialAnchorSession.Start();

            if (this.cloudSpatialAnchorSession != null)
            {
                Debug.Log("Session started");
                debugText.text = "Session started";
            } else
            {
                Debug.Log("Failed to start session");
                debugText.text = "Failed to start session";
            }
        }
    }

    void OnSessionUpdated(object sender, SessionUpdatedEventArgs args)
    {
        /*Debug.Log("Session updated");
        debugText.text = "Session updated";*/
        /*var status = args.Status;
        if (status.UserFeedback == SessionUserFeedback.None) return;
        this.debugText.text = $"Feedback: {Enum.GetName(typeof(SessionUserFeedback), status.UserFeedback)} -" +
            $" Recommend Create={status.RecommendedForCreateProgress: 0.#%}";*/
    }

    void OnError(object sender, SessionErrorEventArgs args)
    {
        /*Debug.Log("Greska " + args.ToString());
        debugText.text = "Greska " + args.ToString();*/
    }

    void OnAnchorLocated(object sender, AnchorLocatedEventArgs args)
    {
        /*Debug.LogFormat("Anchor located maybe");
        debugText.text = "Anchor located maybe";*/
        switch (args.Status)
        {
            case LocateAnchorStatus.Located:
                /*Debug.Log("Anchor located");
                debugText.text = "Anchor located";*/
                UnityEngine.WSA.Application.InvokeOnAppThread(
                    () =>
                    {
                        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                        cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                        cube.GetComponent<Renderer>().material = this.relocalizedCubeMaterial;

                        var worldAnchor = cube.AddComponent<WorldAnchor>();

                        worldAnchor.SetNativeSpatialAnchorPtr(args.Anchor.LocalAnchor);

                        cube.name = args.Identifier;

                        /*Debug.Log("Anchored cube created");
                        debugText.text = "Anchored cube created";*/
                    },
                    false
                );
                // Go add your anchor to the scene...
                break;
            case LocateAnchorStatus.AlreadyTracked:
                /*Debug.Log("Anchor already tracked");
                debugText.text = "Anchor already tracked";*/
                // This anchor has already been reported and is being tracked
                break;
            case LocateAnchorStatus.NotLocatedAnchorDoesNotExist:
                /*Debug.Log("Anchor doesn't exist");
                debugText.text = "Anchor doesn't exist";*/
                // The anchor was deleted or never existed in the first place
                // Drop it, or show UI to ask user to anchor the content anew
                break;
            case LocateAnchorStatus.NotLocated:
                /*Debug.Log("Anchor not located");
                debugText.text = "Anchor not located";*/
                // The anchor hasn't been found given the location data
                // The user might in the wrong location, or maybe more data will help
                // Show UI to tell user to keep looking around
                break;
        }
    }

    void OnLocateAnchorsCompleted(object sender, LocateAnchorsCompletedEventArgs args)
    {
       /* Debug.Log("Anchor location completed");
        debugText.text = "Anchor location completed";*/
        args.Watcher.Stop();
    }

    async Task OnCreateCubeAsync()
    {
        Debug.Log("Creating cube async");
        debugText.text = "Creating cube async";
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        cube.transform.position = MixedRealityToolkit.InputSystem.GazeProvider.HitPosition;
        cube.GetComponent<Renderer>().material = this.cubeMaterial;
        this.cubes.Add(cube);

        var worldAnchor = cube.AddComponent<WorldAnchor>();

        var cloudSpatialAnchor = new CloudSpatialAnchor();
        cloudSpatialAnchor.LocalAnchor = worldAnchor.GetNativeSpatialAnchorPtr();

        await this.WaitForSessionReadyToCreateAsync();

        Debug.Log("Session ready, creating anchor");
        debugText.text = "Session ready, creating anchor";

        try
        {
            await this.cloudSpatialAnchorSession.CreateAnchorAsync(cloudSpatialAnchor);
        }
        catch (Exception ex)
        {
            Debug.Log("Creating anchor failed: " + ex.Message.ToString());
            this.debugText.text = "Creating anchor failed: " + ex.Message.ToString();
            return;
        }

        cube.name = cloudSpatialAnchor.Identifier;
        Debug.Log("Cube created async");
        this.debugText.text = $"Created a cloud anchor with ID={cloudSpatialAnchor.Identifier}";
    }

    async Task OnReloadCubesAsync()
    {
        Debug.Log("Reloading cubes");
        debugText.text = "Reloading cubes";
        if (this.cubes.Count > 0)
        {
            var identifiers = this.cubes.Select(c => c.name).ToArray();
            string all_ids = "";

            foreach (string id in identifiers)
            {
                all_ids += id + "\n";
            }

            await this.OnClearCubesAsync();

            Debug.Log("Creating watchers: " + all_ids);
            debugText.text = "Creating watchers: " + all_ids;
            var watcher = this.cloudSpatialAnchorSession.CreateWatcher(
                new AnchorLocateCriteria()
                {
                    Identifiers = identifiers,
                    BypassCache = true,
                    RequestedCategories = AnchorDataCategory.Spatial,
                    Strategy = LocateStrategy.AnyStrategy
                }
            );
            Debug.Log("Watchers created");
            debugText.text = "Watchers created";
        }
    }

    async Task OnClearCubesAsync()
    {
        Debug.Log("Clearing cubes " + this.cubes.Count().ToString());
        debugText.text = "Clearing cubes " + this.cubes.Count().ToString();
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
            var status = await this.cloudSpatialAnchorSession.GetSessionStatusAsync();
            debugText.text = "Waiting for session ready, " + status.ReadyForCreateProgress.ToString();
            await Task.Delay(250);

            if (status.ReadyForCreateProgress >= 1.0f)
            {
                break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting speech recognition");
        debugText.text = "Starting speech recognition";

        this.cubes = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
