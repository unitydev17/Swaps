using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _pivot;

    private Level _level;
    private UserData _userData;

    private BalloonManager _balloonManager;
    private BoardController _boardController;
    private IRepository _levelStorage;
    private Configuration _cfg;
    private IUserRepository _userDataStorage;
    private UiController _ui;
    private AppModel _appModel;

    [Inject]
    public void Construct(
        Configuration cfg,
        BoardController boardController,
        IRepository levelStorage,
        IUserRepository userDataStorage,
        UiController ui,
        BalloonManager balloonManager,
        AppModel appModel)
    {
        _cfg = cfg;
        _boardController = boardController;
        _levelStorage = levelStorage;
        _userDataStorage = userDataStorage;
        _ui = ui;
        _balloonManager = balloonManager;
        _appModel = appModel;
    }

    private void Awake()
    {
        Input.multiTouchEnabled = false;
        Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {
        BoardController.OnLevelCompleted += LevelCompleted;
        UiController.OnRetry += RetryLevel;
        UiController.OnForceNextLevel += ForceNextLevel;
    }

    private void OnDisable()
    {
        BoardController.OnLevelCompleted -= LevelCompleted;
        UiController.OnRetry -= RetryLevel;
        UiController.OnForceNextLevel -= ForceNextLevel;
    }


    private void Start()
    {
        _userData = _userDataStorage.Load();
        NextLevel();

        _balloonManager.Setup(Camera.main);
        _balloonManager.Activate();

        _appModel.gameActive = true;
    }

    private void ForceNextLevel()
    {
        _boardController.Clear();
        LevelCompleted();
        _ui.DisappearRetryButton();
    }

    private void RetryLevel()
    {
        _boardController.Clear();
        NextLevel();
    }

    private void LevelCompleted()
    {
        _userData.currentLevel++;

        _ui.FadeUnfade();
        NextLevel();
    }

    private void NextLevel()
    {
        var levelIndex = _cfg.GetLevelIndex(_userData.currentLevel);
        _level = _levelStorage.Load(levelIndex);
        var board = LevelToBoardMapper.Map(_level);
        board.minFlushCount = _cfg.MinFlushCount;

        _boardController.SetBoard(board, _pivot, Camera.main);
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