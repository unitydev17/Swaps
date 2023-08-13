using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _root;

    private Level _level;
    private int _currLevel = 1;

    private BoardController _boardController;
    private IRepository _repo;
    private Configuration _cfg;

    [Inject]
    public void Construct(Configuration cfg, BoardController boardController, IRepository repo)
    {
        _cfg = cfg;
        _boardController = boardController;
        _repo = repo;
    }


    private void Start()
    {
        NextLevel();
    }

    public void LevelCompleted()
    {
        _currLevel++;
        NextLevel();
    }

    private void NextLevel()
    {
        var levelIndex = _cfg.GetLevelIndex(_currLevel);
        _level = _repo.Load(levelIndex);
        var board = LevelToBoardMapper.Map(_level);

        _boardController.SetBoard(board, _root);
        _boardController.Activate();
    }

   
}