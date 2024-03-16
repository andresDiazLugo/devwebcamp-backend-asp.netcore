using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class JwtAuthorizeAttribute : Attribute
{
}