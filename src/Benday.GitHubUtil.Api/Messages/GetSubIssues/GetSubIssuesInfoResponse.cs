﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Benday.GitHubUtil.Api.Messages.GetSubIssues;
public class GetSubIssuesInfoResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("repository_url")]
    public string RepositoryUrl { get; set; } = string.Empty;

    [JsonPropertyName("labels_url")]
    public string LabelsUrl { get; set; } = string.Empty;

    [JsonPropertyName("comments_url")]
    public string CommentsUrl { get; set; } = string.Empty;

    [JsonPropertyName("events_url")]
    public string EventsUrl { get; set; } = string.Empty;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public User User { get; set; } = new();

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("locked")]
    public bool Locked { get; set; }

    [JsonPropertyName("assignee")]
    public string Assignee { get; set; } = string.Empty;

    [JsonPropertyName("milestone")]
    public string Milestone { get; set; } = string.Empty;

    [JsonPropertyName("comments")]
    public int Comments { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; set; } = string.Empty;

    [JsonPropertyName("closed_at")]
    public string ClosedAt { get; set; } = string.Empty;

    [JsonPropertyName("author_association")]
    public string AuthorAssociation { get; set; } = string.Empty;

    [JsonPropertyName("sub_issues_summary")]
    public SubIssuesSummary SubIssuesSummary { get; set; } = new();

    [JsonPropertyName("active_lock_reason")]
    public string ActiveLockReason { get; set; } = string.Empty;

    [JsonPropertyName("repository")]
    public Repository Repository { get; set; } = new();

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    

    [JsonPropertyName("timeline_url")]
    public string TimelineUrl { get; set; } = string.Empty;

    [JsonPropertyName("performed_via_github_app")]
    public string PerformedViaGithubApp { get; set; } = string.Empty;

    [JsonPropertyName("state_reason")]
    public string StateReason { get; set; } = string.Empty;

}


public class User
{
    [JsonPropertyName("login")]
    public string Login { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;

    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; } = string.Empty;

    [JsonPropertyName("gravatar_id")]
    public string GravatarId { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    [JsonPropertyName("followers_url")]
    public string FollowersUrl { get; set; } = string.Empty;

    [JsonPropertyName("following_url")]
    public string FollowingUrl { get; set; } = string.Empty;

    [JsonPropertyName("gists_url")]
    public string GistsUrl { get; set; } = string.Empty;

    [JsonPropertyName("starred_url")]
    public string StarredUrl { get; set; } = string.Empty;

    [JsonPropertyName("subscriptions_url")]
    public string SubscriptionsUrl { get; set; } = string.Empty;

    [JsonPropertyName("organizations_url")]
    public string OrganizationsUrl { get; set; } = string.Empty;

    [JsonPropertyName("repos_url")]
    public string ReposUrl { get; set; } = string.Empty;

    [JsonPropertyName("events_url")]
    public string EventsUrl { get; set; } = string.Empty;

    [JsonPropertyName("received_events_url")]
    public string ReceivedEventsUrl { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("user_view_type")]
    public string UserViewType { get; set; } = string.Empty;

    [JsonPropertyName("site_admin")]
    public bool SiteAdmin { get; set; }

}


public class SubIssuesSummary
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("completed")]
    public int Completed { get; set; }

    [JsonPropertyName("percent_completed")]
    public int PercentCompleted { get; set; }

}


public class Repository
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("private")]
    public bool Private { get; set; }

    [JsonPropertyName("owner")]
    public Owner Owner { get; set; } = new();

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("fork")]
    public bool Fork { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("forks_url")]
    public string ForksUrl { get; set; } = string.Empty;

    [JsonPropertyName("keys_url")]
    public string KeysUrl { get; set; } = string.Empty;

    [JsonPropertyName("collaborators_url")]
    public string CollaboratorsUrl { get; set; } = string.Empty;

    [JsonPropertyName("teams_url")]
    public string TeamsUrl { get; set; } = string.Empty;

    [JsonPropertyName("hooks_url")]
    public string HooksUrl { get; set; } = string.Empty;

    [JsonPropertyName("issue_events_url")]
    public string IssueEventsUrl { get; set; } = string.Empty;

    [JsonPropertyName("events_url")]
    public string EventsUrl { get; set; } = string.Empty;

    [JsonPropertyName("assignees_url")]
    public string AssigneesUrl { get; set; } = string.Empty;

    [JsonPropertyName("branches_url")]
    public string BranchesUrl { get; set; } = string.Empty;

    [JsonPropertyName("tags_url")]
    public string TagsUrl { get; set; } = string.Empty;

    [JsonPropertyName("blobs_url")]
    public string BlobsUrl { get; set; } = string.Empty;

    [JsonPropertyName("git_tags_url")]
    public string GitTagsUrl { get; set; } = string.Empty;

    [JsonPropertyName("git_refs_url")]
    public string GitRefsUrl { get; set; } = string.Empty;

    [JsonPropertyName("trees_url")]
    public string TreesUrl { get; set; } = string.Empty;

    [JsonPropertyName("statuses_url")]
    public string StatusesUrl { get; set; } = string.Empty;

    [JsonPropertyName("languages_url")]
    public string LanguagesUrl { get; set; } = string.Empty;

    [JsonPropertyName("stargazers_url")]
    public string StargazersUrl { get; set; } = string.Empty;

    [JsonPropertyName("contributors_url")]
    public string ContributorsUrl { get; set; } = string.Empty;

    [JsonPropertyName("subscribers_url")]
    public string SubscribersUrl { get; set; } = string.Empty;

    [JsonPropertyName("subscription_url")]
    public string SubscriptionUrl { get; set; } = string.Empty;

    [JsonPropertyName("commits_url")]
    public string CommitsUrl { get; set; } = string.Empty;

    [JsonPropertyName("git_commits_url")]
    public string GitCommitsUrl { get; set; } = string.Empty;

    [JsonPropertyName("comments_url")]
    public string CommentsUrl { get; set; } = string.Empty;

    [JsonPropertyName("issue_comment_url")]
    public string IssueCommentUrl { get; set; } = string.Empty;

    [JsonPropertyName("contents_url")]
    public string ContentsUrl { get; set; } = string.Empty;

    [JsonPropertyName("compare_url")]
    public string CompareUrl { get; set; } = string.Empty;

    [JsonPropertyName("merges_url")]
    public string MergesUrl { get; set; } = string.Empty;

    [JsonPropertyName("archive_url")]
    public string ArchiveUrl { get; set; } = string.Empty;

    [JsonPropertyName("downloads_url")]
    public string DownloadsUrl { get; set; } = string.Empty;

    [JsonPropertyName("issues_url")]
    public string IssuesUrl { get; set; } = string.Empty;

    [JsonPropertyName("pulls_url")]
    public string PullsUrl { get; set; } = string.Empty;

    [JsonPropertyName("milestones_url")]
    public string MilestonesUrl { get; set; } = string.Empty;

    [JsonPropertyName("notifications_url")]
    public string NotificationsUrl { get; set; } = string.Empty;

    [JsonPropertyName("labels_url")]
    public string LabelsUrl { get; set; } = string.Empty;

    [JsonPropertyName("releases_url")]
    public string ReleasesUrl { get; set; } = string.Empty;

    [JsonPropertyName("deployments_url")]
    public string DeploymentsUrl { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; set; } = string.Empty;

    [JsonPropertyName("pushed_at")]
    public string PushedAt { get; set; } = string.Empty;

    [JsonPropertyName("git_url")]
    public string GitUrl { get; set; } = string.Empty;

    [JsonPropertyName("ssh_url")]
    public string SshUrl { get; set; } = string.Empty;

    [JsonPropertyName("clone_url")]
    public string CloneUrl { get; set; } = string.Empty;

    [JsonPropertyName("svn_url")]
    public string SvnUrl { get; set; } = string.Empty;

    [JsonPropertyName("homepage")]
    public string Homepage { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("stargazers_count")]
    public int StargazersCount { get; set; }

    [JsonPropertyName("watchers_count")]
    public int WatchersCount { get; set; }

    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    [JsonPropertyName("has_issues")]
    public bool HasIssues { get; set; }

    [JsonPropertyName("has_projects")]
    public bool HasProjects { get; set; }

    [JsonPropertyName("has_downloads")]
    public bool HasDownloads { get; set; }

    [JsonPropertyName("has_wiki")]
    public bool HasWiki { get; set; }

    [JsonPropertyName("has_pages")]
    public bool HasPages { get; set; }

    [JsonPropertyName("has_discussions")]
    public bool HasDiscussions { get; set; }

    [JsonPropertyName("forks_count")]
    public int ForksCount { get; set; }

    [JsonPropertyName("mirror_url")]
    public string MirrorUrl { get; set; } = string.Empty;

    [JsonPropertyName("archived")]
    public bool Archived { get; set; }

    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; }

    [JsonPropertyName("open_issues_count")]
    public int OpenIssuesCount { get; set; }

    [JsonPropertyName("license")]
    public string License { get; set; } = string.Empty;

    [JsonPropertyName("allow_forking")]
    public bool AllowForking { get; set; }

    [JsonPropertyName("is_template")]
    public bool IsTemplate { get; set; }

    [JsonPropertyName("web_commit_signoff_required")]
    public bool WebCommitSignoffRequired { get; set; }

    [JsonPropertyName("visibility")]
    public string Visibility { get; set; } = string.Empty;

    [JsonPropertyName("forks")]
    public int Forks { get; set; }

    [JsonPropertyName("open_issues")]
    public int OpenIssues { get; set; }

    [JsonPropertyName("watchers")]
    public int Watchers { get; set; }

    [JsonPropertyName("default_branch")]
    public string DefaultBranch { get; set; } = string.Empty;

    [JsonPropertyName("permissions")]
    public Permissions Permissions { get; set; } = new();

}


public class Owner
{
    [JsonPropertyName("login")]
    public string Login { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;

    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; } = string.Empty;

    [JsonPropertyName("gravatar_id")]
    public string GravatarId { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    [JsonPropertyName("followers_url")]
    public string FollowersUrl { get; set; } = string.Empty;

    [JsonPropertyName("following_url")]
    public string FollowingUrl { get; set; } = string.Empty;

    [JsonPropertyName("gists_url")]
    public string GistsUrl { get; set; } = string.Empty;

    [JsonPropertyName("starred_url")]
    public string StarredUrl { get; set; } = string.Empty;

    [JsonPropertyName("subscriptions_url")]
    public string SubscriptionsUrl { get; set; } = string.Empty;

    [JsonPropertyName("organizations_url")]
    public string OrganizationsUrl { get; set; } = string.Empty;

    [JsonPropertyName("repos_url")]
    public string ReposUrl { get; set; } = string.Empty;

    [JsonPropertyName("events_url")]
    public string EventsUrl { get; set; } = string.Empty;

    [JsonPropertyName("received_events_url")]
    public string ReceivedEventsUrl { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("user_view_type")]
    public string UserViewType { get; set; } = string.Empty;

    [JsonPropertyName("site_admin")]
    public bool SiteAdmin { get; set; }

}


public class Permissions
{
    [JsonPropertyName("admin")]
    public bool Admin { get; set; }

    [JsonPropertyName("maintain")]
    public bool Maintain { get; set; }

    [JsonPropertyName("push")]
    public bool Push { get; set; }

    [JsonPropertyName("triage")]
    public bool Triage { get; set; }

    [JsonPropertyName("pull")]
    public bool Pull { get; set; }

}



