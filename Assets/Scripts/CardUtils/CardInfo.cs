using System;
using UnityEngine;

public struct CardInfo : IEquatable<CardInfo>
{
    public CardKinds Kind;
    public CardValues Value;

    public int Id
    {
        get { return (int)Kind * CardUtils.ValueCount + (int)Value; }
    }

    public int CardScore
    {
        get
        {
            return Mathf.Min((int)Value + 1, 10);
        }
    }

    public CardInfo(CardKinds kind, CardValues value)
    {
        Kind = kind;
        Value = value;
    }

    public static CardInfo FromId(int id)
    {
        if(id < 0 || id >= 52)
        {
            throw new Exception("id (" + id + ") is out of range");
        }

        return new CardInfo(CardUtils.GetKindById(id), CardUtils.GetValueById(id));
    }

    public override string ToString()
    {
        return Value + " of " + Kind;
    }

    public bool Equals(CardInfo other)
    {
        return Kind == other.Kind && Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
