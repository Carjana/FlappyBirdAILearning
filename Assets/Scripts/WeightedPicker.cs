using System;
using System.Collections.Generic;
using JohaToolkit.UnityEngine.Extensions;
using UnityEngine;
using Random = System.Random;

public class WeightedPicker<TItem>
{
    private readonly Random _random = new();

    private readonly List<TItem> _contents = new();
    private readonly List<float> _probabilities = new();
    private readonly List<float> _probabilitiesSums = new();
    private bool _isDirty;

    public TItem Pick()
    {
        if (_contents.Count == 0 || _probabilities.Count == 0 || _probabilitiesSums.Count == 0)
            throw new InvalidOperationException("Can't pick; 0 Items in the picker!");

        if (_isDirty)
            SetProbabilitiesSums();

        float randomNumber = (float)_random.NextDouble() * _probabilitiesSums[^1];

        int index = _probabilitiesSums.BinarySearch(randomNumber);
        if (index < 0)
            index = ~index;

        return _contents[index];
    }

    public void Add(TItem item, float probability)
    {
        if (item == null)
            throw new ArgumentNullException($"{nameof(item)}: Item cannot be null!");
        if (probability < 0)
            throw new ArgumentOutOfRangeException($"{nameof(probability)}Probability cannot be less than 0!");

        _contents.Add(item);
        _probabilities.Add(probability);
        _probabilitiesSums.Add((_probabilitiesSums.Count == 0 ? 0 : _probabilitiesSums[^1]) + probability);
    }

    /// <Summary>
    /// Removes an item from the picker. Returns false if the item can't be removed.
    /// </Summary>
    public bool Remove(TItem item)
    {
        int index = _contents.IndexOf(item);
        if (index < 0)
            return false;

        _contents.RemoveAt(index);
        _probabilities.RemoveAt(index);
        _probabilitiesSums.RemoveAt(index);

        _isDirty = true;
        return true;
    }

    private void SetProbabilitiesSums()
    {
        _probabilitiesSums.Clear();

        _probabilitiesSums.Add(_probabilities[0]);

        for (int i = 1; i < _contents.Count; i++)
        {
            _probabilitiesSums.Add(_probabilitiesSums[^1] + _probabilities[i]);
        }
        _isDirty = false;
    }

}