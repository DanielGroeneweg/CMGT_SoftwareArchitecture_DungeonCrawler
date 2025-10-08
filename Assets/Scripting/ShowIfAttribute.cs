using System;
using UnityEngine;

public enum ComparisonTypes
{
    Equals,
    NotEqual,
    Greater,
    Less,
    GreaterOrEqual,
    LessOrEqual
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ShowIfAttribute : PropertyAttribute
{
    public string fieldName;
    public ComparisonTypes comparison;
    public object compareValue;

    public ShowIfAttribute(string fieldName, ComparisonTypes comparison, object compareValue)
    {
        this.fieldName = fieldName;
        this.comparison = comparison;
        this.compareValue = compareValue;
    }
}