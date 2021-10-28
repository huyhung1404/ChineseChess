using ChessPiece;
using Managers;
using Net;
using Net.NetMessage;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    [RequireComponent(typeof(ClientMoveChess))]
    public class NetworkPlayer : MonoBehaviour
    {
        [SerializeField] private Transform rematchIndicator;
        [SerializeField] private Button buttonRematch;     
        private bool[] playerRematch;
        private int playerCount = -1;
        
        // #region Network
        private ClientMoveChess clientMoveChess;

        private void Start()
        {
            RegisterEvents();
            playerRematch = new bool[2];
            clientMoveChess = GetComponent<ClientMoveChess>();
        }
        
        private void RegisterEvents()
        {
            NetUtility.S_WELCOME += OnWelcomeServer;
            NetUtility.S_MAKE_MOVE += OnMakeMoveServer;
            NetUtility.S_REMATCH += OnRematchServer;
        
            NetUtility.C_WELCOME += OnWelcomeClient;
            NetUtility.C_START_GAME += OnStartGameClient;
            NetUtility.C_MAKE_MOVE += OnMakeMoveClient;
            NetUtility.C_REMATCH += OnRematchClient;
        
            GameManager.Instance.SetLocalGame += OnSetLocalGame;
        }
        private void OnDisable()
        {
            UnRegisterEvents();
        }
        private void UnRegisterEvents()
        {
            NetUtility.S_WELCOME -= OnWelcomeServer;
            NetUtility.S_MAKE_MOVE -= OnMakeMoveServer;
            NetUtility.S_REMATCH -= OnRematchServer;
        
            NetUtility.C_WELCOME -= OnWelcomeClient;
            NetUtility.C_START_GAME -= OnStartGameClient;
            NetUtility.C_MAKE_MOVE -= OnMakeMoveClient;
            NetUtility.C_REMATCH -= OnRematchClient;
        
            GameManager.Instance.SetLocalGame -= OnSetLocalGame;
        }
        
        //Server
        private void OnWelcomeServer(NetMessage _netMessage, NetworkConnection _connection)
        {
            if (!(_netMessage is NetWelcome _netWelcome)) return;
            _netWelcome.AssignedTeam = ++playerCount;
            Server.Instance.SendToClient(_connection, _netWelcome);
            if (playerCount == 1)
            {
                Server.Instance.Broadcast(new NetStartGame());
            }
        }
        
        private void OnMakeMoveServer(NetMessage msg, NetworkConnection cnn)
        {
            NetMakeMove mm = msg as NetMakeMove;
            Server.Instance.Broadcast(mm);
        }
        
        private void OnRematchServer(NetMessage msg, NetworkConnection cnn)
        {
            Server.Instance.Broadcast(msg);
        }
        
        //Client
        private void OnWelcomeClient(NetMessage msg)
        {
            if (msg is NetWelcome netWelcome) SelectChess.CurrentTeam = netWelcome.AssignedTeam;
            if (SelectChess.IsLocalGame && SelectChess.CurrentTeam == 0)
            {
                Server.Instance.Broadcast(new NetStartGame());
            }
        }
        
        private void OnStartGameClient(NetMessage msg)
        {
            GameManager.Instance.ChangeCamera(SelectChess.CurrentTeam == 0 ? CameraAngle.RED_TEAM : CameraAngle.BLACK_TEAM);
            if (SelectChess.CurrentTeam == 1)
                GameManager.Instance.RotationChessPiece();
        }
        
        private void OnMakeMoveClient(NetMessage msg)
        {
            if (!(msg is NetMakeMove makeMove)) return;
            if (makeMove.teamId == SelectChess.CurrentTeam) return;
            if (!Physics.Raycast(new Vector3(makeMove.originalX, 5, makeMove.originalZ),
                Vector3.down, out RaycastHit hit, 5, 1 << 8)) return;
            StartCoroutine(clientMoveChess.MakeMove( hit.collider.gameObject,
                    new Vector3(makeMove.destinationX, 0.2f, makeMove.destinationZ)));
            SelectChess.LastTeamSelected = makeMove.teamId;
        }
        
        private void OnRematchClient(NetMessage msg)
        {
            if (msg is NetRematch netRematch)
            {
                playerRematch[netRematch.teamId] = netRematch.wantRematch == 1;

                if (netRematch.teamId != SelectChess.CurrentTeam)
                {
                    rematchIndicator.transform.GetChild(netRematch.wantRematch == 1 ? 0 : 1).gameObject.SetActive(true);
                    if (netRematch.wantRematch != 1)
                    {
                        buttonRematch.interactable = false;
                    }
                }
            }

            if (playerRematch[0] && playerRematch[1])
            {
                GameManager.Instance.ResetChessboard();
                GameManager.Instance.OnRematchButton();
                GameManager.Instance.ChangeCamera(SelectChess.CurrentTeam == 0 ? CameraAngle.RED_TEAM : CameraAngle.BLACK_TEAM);
                SelectChess.LastTeamSelected = 1;
                playerRematch[0] = playerRematch[1] = false;
                rematchIndicator.transform.GetChild(0).gameObject.SetActive(false);
                rematchIndicator.transform.GetChild(1).gameObject.SetActive(false);
                buttonRematch.interactable = true;
            }
        }
        
        private void OnSetLocalGame(bool _localGame)
        {
            playerCount = -1;
            SelectChess.CurrentTeam = -1;
            SelectChess.IsLocalGame = _localGame;
        }
        
        public void RematchButton()
        {
            if (SelectChess.IsLocalGame)
            {
                NetRematch wrm = new NetRematch {teamId = 0, wantRematch = 1};
                Client.Instance.SendToServer(wrm);
        
                NetRematch brm = new NetRematch {teamId = 1, wantRematch = 1};
                Client.Instance.SendToServer(brm);
                return;
            }
        
            NetRematch rm = new NetRematch {teamId = SelectChess.CurrentTeam, wantRematch = 1};
            Client.Instance.SendToServer(rm);
        }
        
        public void OnBackButtonRematch()
        {
            NetRematch rm = new NetRematch {teamId = SelectChess.CurrentTeam, wantRematch = 0};
            Client.Instance.SendToServer(rm);
            GameManager.Instance.BackButtonGameOver();
            GameManager.Instance.ResetChessboard();
            Invoke(nameof(ShutDownDelay), 1f);
            playerCount = -1;
            SelectChess.CurrentTeam = -1;
            SelectChess.LastTeamSelected = 1;
        }
        
        public void SendMakeMove(Vector3 original, Vector3 destination)
        {
            NetMakeMove netMakeMove = new NetMakeMove
            {
                originalX = Mathf.Round(original.x),
                originalZ = Mathf.Round(original.z),
                destinationX = Mathf.Round(destination.x),
                destinationZ = Mathf.Round(destination.z),
                teamId = SelectChess.CurrentTeam
            };
            Client.Instance.SendToServer(netMakeMove);
        }
        
        private void ShutDownDelay()
        {
            Client.Instance.Shutdown();
            Server.Instance.Shutdown();
            buttonRematch.interactable = true;
            rematchIndicator.transform.GetChild(0).gameObject.SetActive(false);
            rematchIndicator.transform.GetChild(1).gameObject.SetActive(false);
        }
        
        public void OnButtonSurrender()
        {
            GameManager.Instance.GameOver((TEAM) SelectChess.CurrentTeam);
        }
    }
}
