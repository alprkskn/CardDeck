using System;

public struct CardInfo
{
    public CardKinds Kind;
    public CardValues Value;

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
}
