using System.Collections.Generic;

public class RecursiveFlushWorker : IFlushWorker
{
    private Board _board;

    public void Setup(Board board)
    {
        _board = board;
    }

    public Flushes Work()
    {
        var flushes = new Flushes();

        ProcessElements(flushes);

        return flushes;
    }


    private void ProcessElements(Flushes flushes)
    {
        for (var index = 0; index <= _board.items.Count - _board.minFlushCount; index++)
        {
            if (_board.items[index] == TileType.Empty) continue;

            var counter = 1;
            FindMatches(index, ref counter);
            flushes.UnionWith(GetMatched(index, counter, false));


            counter = 1;
            FindMatches(index, ref counter, true);
            flushes.UnionWith(GetMatched(index, counter, true));
        }
    }

    private IEnumerable<int> GetMatched(int index, int counter, bool isVerticalSearch)
    {
        if (counter < _board.minFlushCount) yield break;

        for (var i = 0; i < counter; i++)
        {
            yield return index + i * (isVerticalSearch ? _board.width : 1);
        }
    }

    private void FindMatches(int index, ref int counter, bool isVerticalSearch = false)
    {
        var nextIndex = index + (isVerticalSearch ? _board.width : 1);
        if (IsOutBounds(nextIndex, isVerticalSearch)) return;


        var matched = _board.items[nextIndex] == _board.items[index];
        if (!matched) return;

        counter++;
        FindMatches(nextIndex, ref counter, isVerticalSearch);
    }

    private bool IsOutBounds(int index, bool isVerticalSearch)
    {
        if (isVerticalSearch) return index > _board.items.Count - 1;

        return index > 0 && index % _board.width == 0;
    }
}