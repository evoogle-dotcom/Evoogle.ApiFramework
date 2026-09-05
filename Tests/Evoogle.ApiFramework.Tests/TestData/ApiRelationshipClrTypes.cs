// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.TestData;

#region One-To-One / One-To-Many Types
public class RelationshipUser
{
    public Ulid Id { get; set; }
    public string UserName { get; set; } = string.Empty;

    // TODO: Need a deep dive on how I want to handle "navigation properties"
    public RelationshipUserProfile? Profile { get; set; }   // navigational property for one-to-one tests

    // TODO: Need a deep dive on how I want to handle "navigation properties"
    // TODO: Should this be optional or required?
    public List<RelationshipPost> Posts { get; set; } = []; // navigational property for one-to-many tests

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var userName = this.UserName.SafeToString();
        var profile = this.Profile.SafeToString();
        var posts = this.Posts.SafeToString();

        return $"{nameof(RelationshipUser)} {{{nameof(this.Id)}={id}, {nameof(this.UserName)}={userName}, {nameof(this.Profile)}={profile}, {nameof(this.Posts)}={posts}}}";
    }
}

public class RelationshipUserProfile
{
    // Scalar FK target for simple dependent-end scalar key path tests.
    public Ulid UserId { get; set; }

    // Nested FK target for dependent-end nested key path tests.
    public RelationshipUserRef UserRef { get; set; } = new();

    public string DisplayName { get; set; } = string.Empty;

    // TODO: Need a deep dive on how I want to handle "navigation properties"
    // TODO: Should this be optional or required? Technically this should be required but what does that really mean?
    public RelationshipUser User { get; set; } = null!; // navigational property back to principal for one-to-one tests

    public override string ToString()
    {
        var userId = this.UserId.SafeToString();
        var userRef = this.UserRef.SafeToString();
        var displayName = this.DisplayName.SafeToString();

        return $"{nameof(RelationshipUserProfile)} {{{nameof(this.UserId)}={userId}, {nameof(this.UserRef)}={userRef}, {nameof(this.DisplayName)}={displayName}}}";
    }
}

public class RelationshipUserRef
{
    public Ulid UserId { get; set; }

    public override string ToString()
    {
        var userId = this.UserId.SafeToString();

        return $"{nameof(RelationshipUserRef)} {{{nameof(this.UserId)}={userId}}}";
    }
}

public class RelationshipPost
{
    public Ulid Id { get; set; }
    public Ulid AuthorUserId { get; set; } // scalar FK path candidate
    public RelationshipUserRef AuthorUserRef { get; set; } = new(); // nested FK path candidate
    public string Title { get; set; } = string.Empty;

    // TODO: Need a deep dive on how I want to handle "navigation properties"
    // TODO: Should this be optional or required?
    public List<RelationshipComment> Comments { get; set; } = [];

    // TODO: Need a deep dive on how I want to handle "navigation properties"
    // TODO: Should this be optional or required?
    public List<RelationshipTag> Tags { get; set; } = [];

    // TODO: Need a deep dive on how I want to handle "navigation properties"
    // TODO: Should this be optional or required? Technically this should be required but what does that really mean?
    public RelationshipUser User { get; set; } = null!; // navigational property back to principal for one-to-one tests

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var authorUserId = this.AuthorUserId.SafeToString();
        var authorUserRef = this.AuthorUserRef.SafeToString();
        var title = this.Title.SafeToString();
        var comments = this.Comments.SafeToString();
        var tags = this.Tags.SafeToString();

        return $"{nameof(RelationshipPost)} {{{nameof(this.Id)}={id}, {nameof(this.AuthorUserId)}={authorUserId}, {nameof(this.AuthorUserRef)}={authorUserRef}, {nameof(this.Title)}={title}, {nameof(this.Comments)}={comments}, {nameof(this.Tags)}={tags}}}";
    }
}

public class RelationshipPostRef
{
    public Ulid PostId { get; set; }

    public override string ToString()
    {
        var postId = this.PostId.SafeToString();

        return $"{nameof(RelationshipPostRef)} {{{nameof(this.PostId)}={postId}}}";
    }
}

public class RelationshipComment
{
    public Ulid Id { get; set; }
    public Ulid PostId { get; set; } // scalar FK path candidate
    public RelationshipPostRef PostRef { get; set; } = new(); // nested FK path candidate
    public string Body { get; set; } = string.Empty;

    // TODO: Need a deep dive on how I want to handle "navigation properties"
    // TODO: Should this be optional or required? Technically this should be required but what does that really mean?
    public RelationshipPost Post { get; set; } = null!; // navigational property back to principal for one-to-one tests

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var postId = this.PostId.SafeToString();
        var postRef = this.PostRef.SafeToString();
        var body = this.Body.SafeToString();

        return $"{nameof(RelationshipComment)} {{{nameof(this.Id)}={id}, {nameof(this.PostId)}={postId}, {nameof(this.PostRef)}={postRef}, {nameof(this.Body)}={body}}}";
    }
}
#endregion

#region Many-To-Many Types
public class RelationshipTag
{
    public Ulid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // TODO: Need a deep dive on how I want to handle "navigation properties"
    // TODO: Should this be optional or required?
    public List<RelationshipPost> Posts { get; set; } = [];

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var name = this.Name.SafeToString();

        return $"{nameof(RelationshipTag)} {{{nameof(this.Id)}={id}, {nameof(this.Name)}={name}}}";
    }
}

// Association object type for many-to-many tests (Post <-> Tag).
public class RelationshipPostTag
{
    public Ulid PostId { get; set; } // dependent end A scalar path candidate
    public Ulid TagId { get; set; }  // dependent end B scalar path candidate

    public override string ToString()
    {
        var postId = this.PostId.SafeToString();
        var tagId = this.TagId.SafeToString();

        return $"{nameof(RelationshipPostTag)} {{{nameof(this.PostId)}={postId}, {nameof(this.TagId)}={tagId}}}";
    }
}
#endregion

#region Composite Key / Nested Key Path Types
public class RelationshipCatalogItem
{
    public string Sku { get; set; } = string.Empty;
    public int Revision { get; set; }
    public string Name { get; set; } = string.Empty;

    public override string ToString()
    {
        var sku = this.Sku.SafeToString();
        var revision = this.Revision.SafeToString();
        var name = this.Name.SafeToString();

        return $"{nameof(RelationshipCatalogItem)} {{{nameof(this.Sku)}={sku}, {nameof(this.Revision)}={revision}, {nameof(this.Name)}={name}}}";
    }
}

public class RelationshipCatalogKey
{
    public string Sku { get; set; } = string.Empty;
    public int Revision { get; set; }

    public override string ToString()
    {
        var sku = this.Sku.SafeToString();
        var revision = this.Revision.SafeToString();

        return $"{nameof(RelationshipCatalogKey)} {{{nameof(this.Sku)}={sku}, {nameof(this.Revision)}={revision}}}";
    }
}

public class RelationshipOrderLine
{
    public Ulid OrderId { get; set; }
    public int LineNumber { get; set; }

    // Scalar key paths to composite principal identity leaves (Sku + Revision).
    public string ProductSku { get; set; } = string.Empty;
    public int ProductRevision { get; set; }

    // Nested key path target to composite principal identity leaves (Sku + Revision).
    public RelationshipCatalogKey ProductKey { get; set; } = new();

    public override string ToString()
    {
        var orderId = this.OrderId.SafeToString();
        var lineNumber = this.LineNumber.SafeToString();
        var productKey = this.ProductKey.SafeToString();

        return $"{nameof(RelationshipOrderLine)} {{{nameof(this.OrderId)}={orderId}, {nameof(this.LineNumber)}={lineNumber}, {nameof(this.ProductKey)}={productKey}}}";
    }
}
#endregion

#region Owner Key Path / Self-Reference Types
public class RelationshipOrder
{
    public Ulid Id { get; set; }
    public List<RelationshipOwnedLine> Lines { get; set; } = [];

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var lines = this.Lines.SafeToString();

        return $"{nameof(RelationshipOrder)} {{{nameof(this.Id)}={id}, {nameof(this.Lines)}={lines}}}";
    }
}

// Useful for owner-key-path scenarios where line identity can include owner identity implicitly.
public class RelationshipOwnedLine
{
    public int LineNumber { get; set; }
    public string? Notes { get; set; }

    public override string ToString()
    {
        var lineNumber = this.LineNumber.SafeToString();
        var notes = this.Notes.SafeToString();

        return $"{nameof(RelationshipOwnedLine)} {{{nameof(this.LineNumber)}={lineNumber}, {nameof(this.Notes)}={notes}}}";
    }
}

// Self-referential relationship tests (one-to-many or one-to-one against same CLR type).
public class RelationshipOrgUnit
{
    public Ulid Id { get; set; }
    public Ulid? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<RelationshipOrgUnit> Children { get; set; } = [];

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var parentId = this.ParentId.SafeToString();
        var name = this.Name.SafeToString();
        var children = this.Children.SafeToString();

        return $"{nameof(RelationshipOrgUnit)} {{{nameof(this.Id)}={id}, {nameof(this.ParentId)}={parentId}, {nameof(this.Name)}={name}, {nameof(this.Children)}={children}}}";
    }
}
#endregion
