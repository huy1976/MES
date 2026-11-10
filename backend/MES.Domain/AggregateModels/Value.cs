namespace MesMicroservice.Domain.AggregateModels;
public class Value : ValueObject
{
    public Value(string valueString, EValueType? valueType)
    {
        ValueString = valueString;
        ValueType = valueType;
    }

    public string ValueString { get; private set; }
    public EValueType? ValueType { get; private set; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ValueString;
        yield return ValueType;
    }

    public object GetValue()
    {
        return ValueType switch
        {
            EValueType.Boolean => bool.Parse(ValueString),
            EValueType.Integer => int.Parse(ValueString),
            EValueType.Decimal => double.Parse(ValueString),
            _ => ValueString,
        };
    }

    public void SetValue(bool value)
    {
        if (ValueType == EValueType.Boolean)
        {
            ValueString = value.ToString();
        }
        else
        {
            throw new InvalidOperationException("Value type is not boolean");
        }
    }

    public void SetValue(int value)
    {
        if (ValueType == EValueType.Integer)
        {
            ValueString = value.ToString();
        }
        else
        {
            throw new InvalidOperationException("Value type is not integer");
        }
    }

    public void SetValue(decimal value)
    {
        if (ValueType == EValueType.Decimal)
        {
            ValueString = value.ToString();
        }
        else
        {
            throw new InvalidOperationException("Value type is not decimal");
        }
    }

    public void SetValue(string value)
    {
        ValueString = value;
    }
}
