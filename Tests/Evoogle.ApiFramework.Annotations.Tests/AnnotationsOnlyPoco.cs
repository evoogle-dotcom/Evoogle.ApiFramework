namespace Evoogle.ApiFramework;

[ApiObject(ApiName = "AnnotatedPoco")]
[ApiKey(ApiName = "AnnotatedPocoKey", ClrPath = nameof(Id))]
[ApiRelationshipDefinition
(
    ApiName = "AnnotatedPocoDependent",
    PrincipalType = typeof(AnnotationsOnlyPoco),
    DependentType = typeof(AnnotationsOnlyDependent),
    Kind = ApiRelationshipKind.OneToMany,
    DeleteBehavior = ApiRelationshipDeleteBehavior.Delete
)]
[ApiManyToManyRelationshipDefinition
(
    ApiName = "AnnotatedPocoAssociations",
    PrincipalTypeA = typeof(AnnotationsOnlyPoco),
    PrincipalTypeB = typeof(AnnotationsOnlyDependent),
    AssociationType = typeof(AnnotationsOnlyAssociation)
)]
internal sealed class AnnotationsOnlyPoco
{
    [ApiKey]
    [ApiProperty(ApiName = "id", IsRequired = true)]
    public int Id { get; set; }

    [ApiIgnore]
    public string Ignored { get; set; } = string.Empty;

    [ApiRelationship
    (
        ApiName = "AnnotatedPocoNavigation",
        Kind = ApiRelationshipKind.OneToOne,
        DeleteBehavior = ApiRelationshipDeleteBehavior.None
    )]
    public AnnotationsOnlyDependent? Dependent { get; set; }

    [ApiManyToManyRelationship
    (
        ApiName = "AnnotatedPocoManyToMany",
        AssociationType = typeof(AnnotationsOnlyAssociation),
        OtherPrincipalType = typeof(AnnotationsOnlyDependent)
    )]
    public List<AnnotationsOnlyDependent> Dependents { get; } = [];
}

[ApiScalar(ApiName = "AnnotatedScalar")]
internal readonly record struct AnnotationsOnlyScalar(int Value);

[ApiEnum(ApiName = "AnnotatedEnum")]
internal enum AnnotationsOnlyEnum
{
    [ApiEnumValue(ApiName = "first")]
    First
}

internal sealed class AnnotationsOnlyDependent;

internal sealed class AnnotationsOnlyAssociation;
