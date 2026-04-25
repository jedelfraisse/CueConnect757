using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CueConnect757.Services;

public class UserInfoService
{
	private readonly AuthenticationStateProvider _authenticationStateProvider;

	public UserInfoService(AuthenticationStateProvider authenticationStateProvider)
	{
		_authenticationStateProvider = authenticationStateProvider;
	}

	public async Task<UserInfo> GetUserInfoAsync()
	{
		var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
		var user = authState.User;

		if (!user.Identity?.IsAuthenticated ?? true)
		{
			return new UserInfo
			{
				IsAuthenticated = false
			};
		}

		return new UserInfo
		{
			IsAuthenticated = true,
			Name = user.FindFirst(ClaimTypes.Name)?.Value ?? user.FindFirst("name")?.Value,
			Email = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("preferred_username")?.Value,
			UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value,
			PreferredUsername = user.FindFirst("preferred_username")?.Value,
			GivenName = user.FindFirst(ClaimTypes.GivenName)?.Value ?? user.FindFirst("given_name")?.Value,
			Surname = user.FindFirst(ClaimTypes.Surname)?.Value ?? user.FindFirst("family_name")?.Value,
			Claims = user.Claims.Select(c => new ClaimInfo { Type = c.Type, Value = c.Value }).ToList()
		};
	}

	public async Task<string?> GetUserNameAsync()
	{
		var userInfo = await GetUserInfoAsync();
		return userInfo.Name;
	}

	public async Task<string?> GetUserEmailAsync()
	{
		var userInfo = await GetUserInfoAsync();
		return userInfo.Email;
	}

	public async Task<bool> IsAuthenticatedAsync()
	{
		var userInfo = await GetUserInfoAsync();
		return userInfo.IsAuthenticated;
	}
}

public class UserInfo
{
	public bool IsAuthenticated { get; set; }
	public string? Name { get; set; }
	public string? Email { get; set; }
	public string? UserId { get; set; }
	public string? PreferredUsername { get; set; }
	public string? GivenName { get; set; }
	public string? Surname { get; set; }
	public List<ClaimInfo> Claims { get; set; } = new();
}

public class ClaimInfo
{
	public string Type { get; set; } = string.Empty;
	public string Value { get; set; } = string.Empty;
}
