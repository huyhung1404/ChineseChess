using System;
using System.Collections.Generic;
using ChessPiece;
using Net;
using Net.NetMessage;
using TMPro;
using UnityEngine;

namespace Managers
{
    public enum CameraAngle
    {
        MENU = 0,
        RED_TEAM = 1,
        BLACK_TEAM = 2
    }

    public class GameManager : Singleton<GameManager>
    {
        private readonly struct ChessPiece
        {
            public ChessPiece(GameObject chessPiece, Vector3 position)
            {
                this.chessPiece = chessPiece;
                this.position = position;
            }
            public GameObject chessPiece { get; }
            public Vector3 position { get; }
        }
    
        public Server server;
        public Client client;
        [SerializeField] private TextMeshProUGUI textGameOver;
        [SerializeField] private Animator _animatorUI;
        [SerializeField] private TMP_InputField addressInput;
        [SerializeField] private GameObject[] cameraAngles;
        private List<ChessPiece> _listChessPieces;
        public Action<bool> SetLocalGame;
    
        private void Awake()
        {
            RegisterEvents();
        }

        private void Start()
        {
            _listChessPieces = new List<ChessPiece>();
            List<GameObject> chessPieces = new List<GameObject>(GameObject.FindGameObjectsWithTag("ChessPiece"));
            chessPieces.AddRange(new List<GameObject>(GameObject.FindGameObjectsWithTag("King")));
            foreach (GameObject o in chessPieces)
            {
                _listChessPieces.Add(new ChessPiece(o,o.transform.position));
            }
        }

        #region GameUi
    
        //Cameras
        public void ChangeCamera(CameraAngle index)
        {
            foreach (GameObject o in cameraAngles)
            {
                o.SetActive(false);
            }
            cameraAngles[(int)index].SetActive(true);
        }
    
        public void RotationChessPiece()
        {
            foreach (ChessPiece chessPiece in _listChessPieces)
            {
                chessPiece.chessPiece.transform.GetChild(0).transform.localRotation *= Quaternion.Euler(0, 180, 0);
            }
        }
        public void GameOver(TEAM team)
        {
            ChangeCamera(CameraAngle.MENU);
            textGameOver.color = team == TEAM.BLACK_TEAM ? Color.red : Color.black;
            textGameOver.text = team == TEAM.BLACK_TEAM ? "Red Team Win" : "Black Team Win";
            _animatorUI.SetTrigger("EndGame");
        }

        public void ResetChessboard()
        {
            foreach (ChessPiece chessPiece in _listChessPieces)
            {
                chessPiece.chessPiece.transform.position = chessPiece.position;
                chessPiece.chessPiece.SetActive(true);
            }
        }

        public void OnRematchButton()
        {
            _animatorUI.SetTrigger("Rematch");
        }
        public void BackButtonGameOver()
        {
            _animatorUI.SetTrigger("EndGameBack");
        }

        public void LocalGameButton()
        {
            _animatorUI.SetTrigger("LocalGame");
            SetLocalGame?.Invoke(true);
            server.Init(8007);
            client.Init("127.0.0.1",8007);
        }

        public void OnlineGameButton()
        {
            _animatorUI.SetTrigger("OnlineMenu");
        }

        public void OnOnlineHostButton()
        {
            SetLocalGame?.Invoke(false);
            server.Init(8007);
            client.Init("127.0.0.1",8007);
            _animatorUI.SetTrigger("HostMenu");
        }
        public void OnOnlineConnectButton()
        {
            SetLocalGame?.Invoke(false);
            client.Init(addressInput.text,8007);
        }
        public void OnOnlineBackButton()
        {
            _animatorUI.SetTrigger("OnlineBack");
        }
        public void OnHostBackButton()
        {
            server.Shutdown();
            client.Shutdown();
            _animatorUI.SetTrigger("HostBack");
        }
        #endregion
    
        #region Event
        private void RegisterEvents()
        {
            NetUtility.C_START_GAME += OnStartGameClient;
        }
        private void UnRegisterEvents()
        {
            NetUtility.C_START_GAME -= OnStartGameClient;
        }

        private void OnStartGameClient(NetMessage obj)
        {
            _animatorUI.SetTrigger("LocalGame");
        }

        private void OnDisable()
        {
            UnRegisterEvents();
        }

        #endregion
    }
}