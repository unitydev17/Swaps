using System;
using System.Threading.Tasks;
using UnityEngine;

public class FlushWorker : IWorker<Flushes>
{
    private Board _board;

    public void Setup(Board board)
    {
        _board = board;
    }

    public async Task<Flushes> Work()
    {
        var flushes = new Flushes();

        try
        {
            ProcessRows(flushes);
            ProcessColumns(flushes);

            await Task.Yield();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        return flushes;
    }

    private void ProcessRows(Flushes flushes)
    {
        for (var i = 0; i < _board.height; i++)
        {
            var currModel = _board.GetItemModel(0, i);
            var beginIndex = _board.GetIndex(0, i);
            var endIndex = beginIndex;

            for (var j = 1; j < _board.width; j++)
            {
                var nextModel = _board.GetItemModel(j, i);

                if (ItemModel.SameType(nextModel, currModel))
                {
                    endIndex = _board.GetIndex(j, i);

                    if (IsLastRowElement(j)) CheckAddRowFlushes(currModel, endIndex, beginIndex);
                }
                else
                {
                    CheckAddRowFlushes(currModel, endIndex, beginIndex);

                    currModel = nextModel;
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

        void CheckAddRowFlushes(ItemModel currModel, int endIndex, int beginIndex)
        {
            if (ItemModel.IsNotEmpty(currModel))
            {
                AddRowFlushes(endIndex, beginIndex);
            }
        }
    }

    private void ProcessColumns(Flushes flushes)
    {
        for (var i = 0; i < _board.width; i++)
        {
            var currModel = _board.GetItemModel(i, 0);
            var beginIndex = _board.GetIndex(i, 0);
            var endIndex = beginIndex;

            for (var j = 1; j < _board.height; j++)
            {
                var nextModel = _board.GetItemModel(i, j);

                if (ItemModel.SameType(nextModel, currModel))
                {
                    endIndex = _board.GetIndex(i, j);
                    if (IsLastColumnElement(j)) CheckAddColumnFlushes(currModel, endIndex, beginIndex);
                }
                else
                {
                    CheckAddColumnFlushes(currModel, endIndex, beginIndex);

                    currModel = nextModel;
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

        void CheckAddColumnFlushes(ItemModel currModel, int endIndex, int beginIndex)
        {
            if (ItemModel.IsNotEmpty(currModel))
            {
                AddColumnFlushes(endIndex, beginIndex);
            }
        }
    }
}