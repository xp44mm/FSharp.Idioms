module FSharp.Idioms.ClassType

open System.Reflection
open System

/// <summary>
/// Gets the mmbr's value on the object.
/// </summary>
/// <param name="mmbr">The mmbr.</param>
/// <param name="target">The target object.</param>
/// <returns>The mmbr's value on the object.</returns>
let ReadMemberValue(mmbr:MemberInfo) (target:obj) =
    match mmbr.MemberType with
    | MemberTypes.Field ->
        (mmbr:?>FieldInfo).GetValue(target);
    | MemberTypes.Property ->
        try
            (mmbr:?>PropertyInfo).GetValue(target, null);
        with :? TargetParameterCountException as e ->
            raise <| new ArgumentException("MemberInfo '{0}' has index parameters", e);
     | _ ->
        raise <| new ArgumentException("MemberInfo '{0}' is not of type FieldInfo or PropertyInfo", nameof(mmbr));

/// <summary>
/// Sets the mmbr's value on the target object.
/// </summary>
/// <param name="mmbr">The mmbr.</param>
/// <param name="target">The target.</param>
/// <param name="value">The value.</param>
let WriteMemberValue (mmbr:MemberInfo) (target:obj) (value:obj) =
    match mmbr.MemberType with
    | MemberTypes.Field ->
        (mmbr:?>FieldInfo).SetValue(target,value)
    | MemberTypes.Property ->
        try
            (mmbr:?>PropertyInfo).SetValue(target, value, null)
        with :? TargetParameterCountException as e ->
            raise <| new ArgumentException("MemberInfo '{0}' has index parameters", e)
     | _ ->
        raise <| new ArgumentException("MemberInfo '{0}' is not of type FieldInfo or PropertyInfo", nameof(mmbr))


/// <summary>
/// Determines whether the specified MemberInfo can be read.
/// </summary>
/// <param name="mmbr">The MemberInfo to determine whether can be read.</param>
/// <returns>
/// 	<c>true</c> if the specified MemberInfo can be read; otherwise, <c>false</c>.
/// </returns>
let CanReadMemberValue (mmbr:MemberInfo) =
    match mmbr.MemberType with
    | MemberTypes.Field ->
        let fieldInfo = mmbr:?>FieldInfo
        fieldInfo.IsPublic
    | MemberTypes.Property ->
        let propertyInfo = mmbr:?>PropertyInfo
        propertyInfo.CanRead

     | _ -> false

/// <summary>
/// Determines whether the specified MemberInfo can be set.
/// </summary>
/// <param name="mmbr">The MemberInfo to determine whether can be set.</param>
/// <returns>
/// 	<c>true</c> if the specified MemberInfo can be set; otherwise, <c>false</c>.
/// </returns>
let CanWriteMemberValue (mmbr:MemberInfo) =
    match mmbr.MemberType with
    | MemberTypes.Field ->
        let fieldInfo = mmbr:?>FieldInfo
        fieldInfo.IsPublic && not fieldInfo.IsLiteral && not fieldInfo.IsInitOnly
    | MemberTypes.Property ->
        let propertyInfo = mmbr:?>PropertyInfo
        propertyInfo.CanWrite 
     | _ -> false

//let getMember (ty:Type) (name:string) =
//    let method = 
//        ty.GetMember(name, Array.empty, BindingFlags.Public ||| BindingFlags.Instance)
//        |> Array.exactlyOne

