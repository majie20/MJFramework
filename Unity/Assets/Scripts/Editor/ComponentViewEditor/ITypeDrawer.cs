using System;

public interface ITypeDrawer
{
    bool HandlesType(Type type);

    object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target);
}