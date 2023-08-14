using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _pivot;

    private Level _level;
    private UserData _userData;

    private BoardController _boardController;
    private IRepository _levelStorage;
    private Configuration _cfg;
    private IUserRepository _userDataStorage;
    private UiController _ui;

    [Inject]
    public void Construct(Configuration cfg, BoardController boardController, IRepository levelStorage, IUserRepository userDataStorage, UiController ui)
    {
        _cfg = cfg;
        _boardController = boardController;
        _levelStorage = levelStorage;
        _userDataStorage = userDataStorage;
        _ui = ui;
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

    public void ForceNextLevel()
    {
        _boardController.Clear();
        LevelCompleted();
    }

    public void LevelCompleted()
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

        _boardController.SetBoard(board, _pivot);
        _boardController.Activate();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            _userDataStorage.Save(_userData);
        }
    }

    private void Update()
    {
        _boardController?.CorrectRootScale();
    }
}