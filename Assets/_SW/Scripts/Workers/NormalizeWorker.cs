using System;
using System.Threading.Tasks;
using UnityEngine;

public class NormalizeWorker : IWorker<Moves>
{
    private Board _normBoard;

    public void Setup(Board board)
    {
        _normBoard = new Board(board);
    }

    public async Task<Moves> Work()
    {
        var moves = new Moves();
        try
        {
            bool hadMoves;

            do
            {
                hadMoves = false;

                for (var i = _normBoard.width; i < _normBoard.items.Count; i++)
                {
                    var currPos = _normBoard.GetPos(i);
                    if (_normBoard.GetItemModel(currPos) is EmptyModel) continue;

                    var nextPos = currPos;
                    var targetPos = currPos;
                    var hasEmpties = false;

                    for (var y = currPos.y - 1; y >= 0; y--)
                    {
                        nextPos.y = y;
                        var nextItem = _normBoard.GetItemModel(nextPos);

                        if (nextItem is EmptyModel)
                        {
                            hasEmpties = true;
                            targetPos = nextPos;
                        }
                        else break;
                    }

                    if (!hasEmpties) continue;

                    var targetIndex = _normBoard.GetIndex(targetPos);
                    _normBoard.Move(i, targetIndex);

                    hadMoves = true;
                    moves.Add((i, targetIndex));

                    await Task.Yield();
                }
            } while (hadMoves);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        return moves;
    }
}