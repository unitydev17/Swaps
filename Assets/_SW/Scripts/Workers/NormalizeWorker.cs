using System;
using UnityEngine;

public class NormalizeWorker : IWorker<Moves>
{
    private Board _normBoard;

    public void Setup(Board board)
    {
        _normBoard = new Board(board);
    }

    public Moves Work()
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
                    if (_normBoard.GetType(currPos) == TileType.Empty) continue;

                    var nextPos = currPos;
                    var targetPos = currPos;
                    var hasEmpties = false;

                    for (var y = currPos.y - 1; y >= 0; y--)
                    {
                        nextPos.y = y;
                        var nextItem = _normBoard.GetType(nextPos);

                        if (nextItem == TileType.Empty)
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