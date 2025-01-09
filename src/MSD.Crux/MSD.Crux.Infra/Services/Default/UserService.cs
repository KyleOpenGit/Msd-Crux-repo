using MSD.Crux.API.Helpers;
using MSD.Crux.Core.Models;
using MSD.Crux.Core.Repositories;
using MSD.Crux.Core.Services;

namespace MSD.Crux.Infra.Services;

/// <summary>
/// ISuerService 디폴트 구현체.
/// user 등록과 정보수정, 검색 로직
/// </summary>
/// <param name="_userRepo">DI로 주입되는 user 레포지토리</param>
/// <param name="_employeeRepo">DI로 주입되는 employee 레포지토리</param>
public class UserService(IUserRepo _userRepo, IEmployeeRepo _employeeRepo) : IUserService
{
    public async Task<UserRegiRspDto> RegisterUserAsync(UserRegiReqDto request)
    {
        // Employee 존재 확인
        var employee = await _employeeRepo.GetByEmployeeNumberAsync(request.EmployeeNumber);
        if (employee == null)
        {
            throw new InvalidOperationException($"사원번호 {request.EmployeeNumber}에 해당하는 직원이 존재하지 않습니다.");
        }

        // 중복 유저 확인
        User? existingUser = await _userRepo.GetByEmployeeNumberAsync(request.EmployeeNumber);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"사원번호 {request.EmployeeNumber}에 해당하는 유저가 이미 등록되어 있습니다.");
        }

        // 새로운 유저 등록
        var user = new User { EmployeeNumber = request.EmployeeNumber, Name = request.Name, Roles = request.Roles };

        await _userRepo.AddAsync(user);

        return new UserRegiRspDto { Id = user.Id, EmployeeNumber = user.EmployeeNumber, Name = user.Name, Roles = user.Roles };
    }

    public async Task UpdateUserByIdAsync(int id, UserUpdateReqDto updateReqDto)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException($"ID {id}에 해당하는 유저가 존재하지 않습니다.");
        }

        ApplyUserUpdates(user, updateReqDto);
        await _userRepo.UpdateAsync(user);
    }

    public async Task UpdateUserByEmployeeNumberAsync(int employeeNumber, UserUpdateReqDto updateReqDto)
    {
        var user = await _userRepo.GetByEmployeeNumberAsync(employeeNumber);
        if (user == null)
        {
            throw new InvalidOperationException($"사원번호 {employeeNumber}에 해당하는 유저가 존재하지 않습니다.");
        }

        ApplyUserUpdates(user, updateReqDto);
        await _userRepo.UpdateAsync(user);
    }

    public async Task<IEnumerable<UserInfoRspDto>> GetAllUsersAsync()
    {
        IEnumerable<User>? users = await _userRepo.GetAllAsync();
        return users.Select(u => new UserInfoRspDto { Id = u.Id, EmployeeNumber = u.EmployeeNumber, Name = u.Name, Roles = u.Roles, LoginId = u.LoginId });
    }

    public async Task<UserInfoRspDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        return new UserInfoRspDto { Id = user.Id, EmployeeNumber = user.EmployeeNumber, Name = user.Name, Roles = user.Roles, LoginId = user.LoginId };
    }

    public async Task<UserInfoRspDto?> GetUserByEmployeeNumberAsync(int employeeNumber)
    {
        var user = await _userRepo.GetByEmployeeNumberAsync(employeeNumber);
        if (user == null)
        {
            return null;
        }

        return new UserInfoRspDto { Id = user.Id, EmployeeNumber = user.EmployeeNumber, Name = user.Name, Roles = user.Roles, LoginId = user.LoginId };
    }

    public async Task DeleteUserAsync(int id)
    {
        await _userRepo.DeleteAsync(id);
    }

    public async Task<IEnumerable<UserRoleApplyRspDto>> GetUserRoleApplicationsAsync()
    {
        IEnumerable<Employee>? employees = await _employeeRepo.GetAllAsync();
        return employees.Where(e => !string.IsNullOrEmpty(e.ApplyUserRoles)).Select(e => new UserRoleApplyRspDto
        {
            EmployeeNumber = GenericHelper.ConvertToEmployeeNumber(e.Year, e.Gender, e.Sequence),
            Name = e.Name,
            ApplyUserRoles = e.ApplyUserRoles
        });
    }

    /// <summary>
    /// user 레코드를 수정한다.
    /// 단, ID/PW 초기화 요청이 포함된다면 LoginId, LoginPw, Salt는 세개가 모두 정보가 있거나 모두 정보가 없어야만 수정가능.
    /// </summary>
    /// <param name="user">변경될 user 객체 소스</param>
    /// <param name="updateReqDto">직원이름, 권한, ID/PW 초기화 신청 여부</param>
    private static void ApplyUserUpdates(User user, UserUpdateReqDto updateReqDto)
    {
        // ResetLoginCredentials가 true이면 로그인 정보 초기화
        if (updateReqDto.ResetLoginCredentials)
        {
            user.LoginId = null;
            user.LoginPw = null;
            user.Salt = null;
        }

        // Name 업데이트
        if (!string.IsNullOrWhiteSpace(updateReqDto.Name) && updateReqDto.Name != user.Name)
        {
            user.Name = updateReqDto.Name;
        }

        // Roles 업데이트
        if (!string.IsNullOrWhiteSpace(updateReqDto.Roles) && updateReqDto.Roles != user.Roles)
        {
            user.Roles = updateReqDto.Roles;
        }
    }
}
