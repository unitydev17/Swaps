public class RecursiveFlushWorker : IFlushWorker
{
    private const int MinFlushCount = 3;

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

    private int _counter;

    private void ProcessElements(Flushes flushes)
    {
        for (var index = 0; index <= _board.items.Count - MinFlushCount; index++)
        {
            if (_board.items[index] == TileType.Empty) continue;

            _counter = 1;
            FindMatches(index, false);
            AddMatched(flushes, index, false);

            _counter = 1;
            FindMatches(index, true);
            AddMatched(flushes, index, true);
        }
    }

    private void AddMatched(Flushes flushes, int index, bool isColumns)
    {
        if (_counter < MinFlushCount) return;
        for (var i = 0; i < _counter; i++) flushes.Add(index + i * (isColumns ? _board.width : 1));
    }

    private void FindMatches(int index, bool isVerticalSearch)
    {
        if (IsOutBounds(index, isVerticalSearch)) return;

        var nextIndex = index + (isVerticalSearch ? _board.width : 1);
        var matched = _board.items[nextIndex] == _board.items[index];
        if (!matched) return;

        _counter++;
        FindMatches(nextIndex, isVerticalSearch);
    }

    private bool IsOutBounds(int index, bool isVerticalSearch)
    {
        if (isVerticalSearch) return index > _board.items.Count - _board.width - 1;

        return index > 0 && index % _board.width == 0;
    }
}