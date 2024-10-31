using System;
using TidyChat.Rules;

namespace TidyChat.Attributes;

[AttributeUsage(AttributeTargets.Field)]
internal class CateAttribute(Cate tab, SubCate header = SubCate.None) : Attribute
{
    public Cate Tab { get; } = tab;
    public SubCate CollapsingHeader { get; } = header;
}
