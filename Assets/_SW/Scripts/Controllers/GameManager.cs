using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _root;

    private Level _level;
    private int _currLevel = 1;

    private BoardManager _boardManager;
    private IRepository _repo;

    [Inject]
    public void Construct(BoardManager boardManager, IRepository repo)
    {
        _boardManager = boardManager;
        _repo = repo;
    }

    private void Start()
    {
        NextLevel();
    }

    private void NextLevel()
    {
        _level = _repo.Load(_currLevel);
        var board = LevelToBoardMapper.Map(_level);

        _boardManager.SetBoard(board, _root);
        _boardManager.Activate();
    }
}