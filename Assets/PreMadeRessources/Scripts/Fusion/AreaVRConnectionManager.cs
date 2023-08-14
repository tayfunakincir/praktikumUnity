using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/**
     * 
     * Handles:
     * - connexion launch
     * - user representation spawn on connection
     * 
     **/

    public class AreaVRConnectionManager : MonoBehaviour, IAvrNetworkRunnerCallbacks
    {
        [Header("Room configuration")]
        public GameMode mode = GameMode.Shared;
        public string roomName = "SampleFusionVR";
        public bool connectOnStart = false;

        [Header("Fusion settings")]
        [Tooltip("Fusion runner. Automatically created if not set")]
        public NetworkRunner runner;
        public INetworkSceneManager sceneManager;

        [Header("Local user spawner")]
        public NetworkObject userPrefab;

        [Header("Event")]
        public UnityEvent onWillConnect = new UnityEvent();

        [Header("Misc")] public string startingScene;
        public GameObject loadingOverlay;
        [Networked] public int playerCount { get; set; }
        public bool alreadyJoined;

        private string _localIP;
        private void Awake()
        {
            if (loadingOverlay != null)
            {
                DontDestroyOnLoad(loadingOverlay);
            }
            
            // Check if a runner exist on the same game object
            if (runner == null) runner = GetComponent<NetworkRunner>();

            // Create the Fusion runner and let it know that we will be providing user input
            if (runner == null) runner = gameObject.AddComponent<NetworkRunner>();
            runner.ProvideInput = true;
        }

        private void Start()
        {
            SetupSession();
        }

        public async Task Connect(string customSuffix)
        {
            // Create the scene manager if it does not exist
            if (sceneManager == null) sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

            if (onWillConnect != null) onWillConnect.Invoke();
            // Start or join (depends on gamemode) a session with a specific name
            var args = new StartGameArgs()
            {
                GameMode = mode,
                SessionName = roomName + " + " + customSuffix,
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = sceneManager
            };
            await runner.StartGame(args);
        }


        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (player == runner.LocalPlayer)
            {
                // Spawn the user prefab for the local user
                NetworkObject networkPlayerObject = runner.Spawn(userPrefab, position: transform.position, rotation: transform.rotation, player, (runner, obj) => {
                });
                runner.SetPlayerObject(player, networkPlayerObject);
            }

            //DelayConnection();
        }

        private async void DelayConnection()
        {
            if (runner.ActivePlayers.ToList().Count <= 1)
            {
                runner.SetActiveScene(startingScene);
            }
            await Task.Delay(2000);
            if (loadingOverlay != null)
            {
                loadingOverlay.SetActive(false);
            }
        }
        
        private string GetGlobalIPAddress(int index)
        {
            try
            {
                string url;
                switch (index)
                {
                    case 0:
                        url = "https://ipinfo.io/ip";
                        break;
                    case 1: 
                        url = "https://api.ipify.org";
                        break;
                    case 2:
                        url = "https://ifconfig.me/ip";
                        break;
                    default:
                        return "Default";
                }

                var request = WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var dataStream = response.GetResponseStream();

                using StreamReader reader = new StreamReader(dataStream);

                var ip = reader.ReadToEnd();
                reader.Close();
                return ip;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return "";
            }
           
        }
        

        private async void SetupSession()
        {
            var index = 0;
            var ipAdress = GetGlobalIPAddress(index);
            while (ipAdress == String.Empty || ipAdress.Length > 15) 
            {
                if (ipAdress != String.Empty && ipAdress.Length <= 15)
                {
                    break;
                }
                index++;
                ipAdress = GetGlobalIPAddress(index);
                await Task.Delay(2000);
            }
            if (!connectOnStart) return;
            await Connect(ipAdress);
        }
    }

