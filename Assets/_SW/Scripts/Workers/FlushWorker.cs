public class FlushWorker : IWorker<Flushes>
{
    private Board _board;

    public void Setup(Board board)
    {
        _board = board;
    }

    public Flushes Work()
    {
        var flushes = new Flushes();

        ProcessRows(flushes);
        ProcessColumns(flushes);

        return flushes;
    }

    private void ProcessRows(Flushes flushes)
    {
        for (var i = 0; i < _board.height; i++)
        {
            var currType = _board.GetTileType(0, i);
            var beginIndex = _board.GetIndex(0, i);
            var endIndex = beginIndex;

            for (var j = 1; j < _board.width; j++)
            {
                var nextType = _board.GetTileType(j, i);

                if (nextType == currType)
                {
                    endIndex = _board.GetIndex(j, i);

                    if (IsLastRowElement(j)) CheckAddRowFlushes(currType, endIndex, beginIndex);
                }
                else
                {
                    CheckAddRowFlushes(currType, endIndex, beginIndex);

                    currType = nextType;
                    beginIndex = _board.GetIndex(j, i);
                    endIndex = beginIndex;
                }
            }
        }

        void AddRowFlushes(int endIndex, int beginIndex)
        {
            var size = endIndex - beginIndex;
            if (size < 2) return;
            for (var k = beginIndex; k <= endIndex; k++) flushes.Add(k);
        }

        bool IsLastRowElement(int j)
        {
            return (j + 1) % _board.width == 0;
        }

        void CheckAddRowFlushes(TileType currModel, int endIndex, int beginIndex)
        {
            if (currModel != TileType.Empty)
            {
                AddRowFlushes(endIndex, beginIndex);
            }
        }
    }

    private void ProcessColumns(Flushes flushes)
    {
        for (var i = 0; i < _board.width; i++)
        {
            var currType = _board.GetTileType(i, 0);
            var beginIndex = _board.GetIndex(i, 0);
            var endIndex = beginIndex;

            for (var j = 1; j < _board.height; j++)
            {
                var nextType = _board.GetTileType(i, j);

                if (nextType == currType)
                {
                    endIndex = _board.GetIndex(i, j);
                    if (IsLastColumnElement(j)) CheckAddColumnFlushes(currType, endIndex, beginIndex);
                }
                else
                {
                    CheckAddColumnFlushes(currType, endIndex, beginIndex);

                    currType = nextType;
                    beginIndex = _board.GetIndex(i, j);
                    endIndex = beginIndex;
                }
            }
        }

        void AddColumnFlushes(int endIndex, int beginIndex)
        {
            var size = (endIndex - beginIndex) / _board.width;
            if (size < 2) return;
            for (var k = beginIndex; k <= endIndex; k += _board.width) flushes.Add(k);
        }

        bool IsLastColumnElement(int j)
        {
            return (j + 1) % _board.height == 0;
        }

        void CheckAddColumnFlushes(TileType currType, int endIndex, int beginIndex)
        {
            if (currType != TileType.Empty)
            {
                AddColumnFlushes(endIndex, beginIndex);
            }
        }
    }
}