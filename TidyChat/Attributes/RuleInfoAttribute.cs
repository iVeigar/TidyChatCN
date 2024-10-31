using System;

namespace TidyChat.Attributes;

[AttributeUsage(AttributeTargets.Field)]
internal class RuleInfoAttribute(string description, string helpMark = "") : Attribute
{
    public string Description { get; } = description;
    public string HelpMessage { get; } = helpMark;
}

