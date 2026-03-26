using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages.Users;

public class IndexModel(IUserService userService, IRoleService roleService, ITeamService teamService) : PageModel
{
    public IReadOnlyCollection<User> Users { get; private set; } = [];
    public IReadOnlyDictionary<Guid, IReadOnlyCollection<string>> RoleNamesByUserId { get; private set; } =
        new Dictionary<Guid, IReadOnlyCollection<string>>();
    public IReadOnlyDictionary<Guid, IReadOnlyCollection<string>> TeamNamesByUserId { get; private set; } =
        new Dictionary<Guid, IReadOnlyCollection<string>>();

    public IReadOnlyCollection<string> AvailableRoles { get; private set; } = [];
    public IReadOnlyCollection<string> AvailableTeams { get; private set; } = [];
    public IReadOnlyCollection<int> PageSizeOptions { get; } = [10, 25, 50];

    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }
    public int ActiveUserCount { get; private set; }
    public int InactiveUserCount { get; private set; }
    public int LockedUserCount { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Role { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Team { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "Name";

    [BindProperty(SupportsGet = true)]
    public string SortDirection { get; set; } = "asc";

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            AvailableRoles = (await roleService.GetAllAsync(cancellationToken))
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToArray();

            AvailableTeams = (await teamService.GetAllAsync(cancellationToken))
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToArray();

            var allUsers = await userService.GetAllAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var normalizedSearch = Search.Trim();
                allUsers = allUsers
                    .Where(x =>
                        x.FirstName.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                        x.LastName.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                        x.Email.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }

            var userIds = allUsers.Select(x => x.Id).ToArray();
            var roleNamesByUserId = await userService.GetRoleNamesByUserIdsAsync(userIds, cancellationToken);
            var teamNamesByUserId = await userService.GetTeamNamesByUserIdsAsync(userIds, cancellationToken);

            if (!string.IsNullOrWhiteSpace(Role))
            {
                allUsers = allUsers
                    .Where(x => roleNamesByUserId.TryGetValue(x.Id, out var roleNames) && roleNames.Contains(Role))
                    .ToArray();
            }

            if (!string.IsNullOrWhiteSpace(Team))
            {
                allUsers = allUsers
                    .Where(x => teamNamesByUserId.TryGetValue(x.Id, out var teamNames) && teamNames.Contains(Team))
                    .ToArray();
            }

            if (!PageSizeOptions.Contains(PageSize))
            {
                PageSize = 10;
            }

            var sortedUsers = (SortBy, SortDirection.ToLowerInvariant()) switch
            {
                ("Email", "desc") => allUsers.OrderByDescending(x => x.Email),
                ("Email", _) => allUsers.OrderBy(x => x.Email),
                ("Status", "desc") => allUsers.OrderByDescending(x => x.Status).ThenBy(x => x.FirstName).ThenBy(x => x.LastName),
                ("Status", _) => allUsers.OrderBy(x => x.Status).ThenBy(x => x.FirstName).ThenBy(x => x.LastName),
                (_, "desc") => allUsers.OrderByDescending(x => x.FirstName).ThenByDescending(x => x.LastName),
                _ => allUsers.OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
            };

            var sortedArray = sortedUsers.ToArray();
            TotalCount = sortedArray.Length;
            TotalPages = Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));

            ActiveUserCount = sortedArray.Count(x => x.Status == UserStatus.Active);
            InactiveUserCount = sortedArray.Count(x => x.Status == UserStatus.Inactive);
            LockedUserCount = sortedArray.Count(x => x.Status == UserStatus.Locked);

            if (Page < 1)
            {
                Page = 1;
            }

            if (Page > TotalPages)
            {
                Page = TotalPages;
            }

            Users = sortedArray
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToArray();

            var visibleUserIds = Users.Select(x => x.Id).ToArray();
            RoleNamesByUserId = await userService.GetRoleNamesByUserIdsAsync(visibleUserIds, cancellationToken);
            TeamNamesByUserId = await userService.GetTeamNamesByUserIdsAsync(visibleUserIds, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Users = [];
            RoleNamesByUserId = new Dictionary<Guid, IReadOnlyCollection<string>>();
            TeamNamesByUserId = new Dictionary<Guid, IReadOnlyCollection<string>>();
            AvailableRoles = [];
            AvailableTeams = [];
            TotalCount = 0;
            TotalPages = 0;
            ActiveUserCount = 0;
            InactiveUserCount = 0;
            LockedUserCount = 0;
        }
    }
}
