using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _root;
    
    private Level _level;
    private int _currLevel = 1;

    private LevelManager _levelManager;
    private IRepository _repo;

    [Inject]
    public void Construct(LevelManager levelManager, IRepository repo)
    {
        _levelManager = levelManager;
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
        
        _levelManager.SetBoard(board, _root);
        _levelManager.Activate();
    }
}