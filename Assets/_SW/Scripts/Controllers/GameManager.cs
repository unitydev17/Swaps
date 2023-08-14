using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _root;

    private Level _level;
    private UserData _userData;

    private BoardController _boardController;
    private IRepository _levelStorage;
    private Configuration _cfg;
    private IUserRepository _userDataStorage;

    [Inject]
    public void Construct(Configuration cfg, BoardController boardController, IRepository levelStorage, IUserRepository userDataStorage)
    {
        _cfg = cfg;
        _boardController = boardController;
        _levelStorage = levelStorage;
        _userDataStorage = userDataStorage;
    }

    private void Awake()
    {
        Input.multiTouchEnabled = false;
    }


    private void Start()
    {
        _userData = _userDataStorage.Load();
        NextLevel();
    }

    public void LevelCompleted()
    {
        _userData.currentLevel++;
        NextLevel();
    }

    private void NextLevel()
    {
        var levelIndex = _cfg.GetLevelIndex(_userData.currentLevel);
        _level = _levelStorage.Load(levelIndex);
        var board = LevelToBoardMapper.Map(_level);

        _boardController.SetBoard(board, _root);
        _boardController.Activate();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            _userDataStorage.Save(_userData);
        }
    }
}