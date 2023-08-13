using System.Threading.Tasks;

public class NormalizeWorker : IWorker<Moves>
{
    private Board _board;

    public void SetBoard(Board board)
    {
        _board = board;
    }

    public async Task<Moves> Work()
    {
        var moves = new Moves();

        var normBoard = new Board(_board);
        bool hadMoves;

        do
        {
            hadMoves = false;

            for (var i = normBoard.width; i < normBoard.items.Count; i++)
            {
                var currPos = normBoard.GetPos(i);
                if (normBoard.GetItemModel(currPos) is EmptyModel) continue;

                var nextPos = currPos;
                var targetPos = currPos;
                var hasEmpties = false;

                for (var y = currPos.y - 1; y >= 0; y--)
                {
                    nextPos.y = y;
                    var nextItem = normBoard.GetItemModel(nextPos);

                    if (nextItem is EmptyModel)
                    {
                        hasEmpties = true;
                        targetPos = nextPos;
                    }
                    else break;
                }

                if (!hasEmpties) continue;

                var targetIndex = normBoard.GetIndex(targetPos);
                normBoard.MoveSilent(i, targetIndex);

                hadMoves = true;
                moves.Add((i, targetIndex));

                await Task.Yield();
            }
        } while (hadMoves);

        return moves;
    }
}