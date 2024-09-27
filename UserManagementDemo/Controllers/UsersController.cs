using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UserManagementDemo.ViewModels;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
  private readonly UserManager<IdentityUser> _userManager;

  public UsersController(UserManager<IdentityUser> userManager)
  {
    _userManager = userManager;
  }
  /// <summary>
  /// 獲取用戶列表
  /// </summary>
  /// <returns>用戶列表</returns>
  /// <response code="200">返回用戶列表</response>
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<IdentityUser>), 200)]
  public async Task<ActionResult<IEnumerable<IdentityUser>>> GetUsers()
  {
    var users = _userManager.Users.ToList();
    return Ok(users);
  }

  /// <summary>
  /// 獲取用戶信息
  /// </summary>
  /// <param name="id">用戶ID</param>
  /// <returns>用戶結果</returns>
  /// <response code="200">返回用戶信息</response>
  /// <response code="404">用戶未找到</response>
  [HttpGet("{id}")]
  [ProducesResponseType(typeof(IdentityUser), 200)]
  [ProducesResponseType(404)]
  public async Task<ActionResult<IdentityUser>> GetUser(string id)
  {
    var user = await _userManager.FindByIdAsync(id);
    if (user == null)
    {
      return NotFound();
    }
    return Ok(user);
  }

  /// <summary>
  /// 更新用戶信息
  /// </summary>
  /// <param name="id">用戶ID</param>
  /// <param name="model">用戶更新模型</param>
  /// <returns>操作結果</returns>
  /// <response code="200">更新成功</response>
  /// <response code="400">請求無效</response>
  /// <response code="404">用戶未找到</response>
  [HttpPut("{id}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(404)]
  public async Task<ActionResult> EditUser(string id, [FromBody] EditUserViewModel model)
  {
    if (id != model.Id)
    {
      return BadRequest("User ID mismatch");
    }

    if (ModelState.IsValid)
    {
      var user = await _userManager.FindByIdAsync(model.Id);
      if (user == null)
      {
        return NotFound();
      }

      // 簡單屬性更新
      user.UserName = model.UserName;
      user.PhoneNumber = model.PhoneNumber;

      // 需要額外邏輯的屬性更新
      var emailResult = await _userManager.SetEmailAsync(user, model.Email);
      if (!emailResult.Succeeded)
      {
        foreach (var error in emailResult.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
        return BadRequest(ModelState);
      }

      var updateResult = await _userManager.UpdateAsync(user);
      if (updateResult.Succeeded)
      {
        return Ok();
      }
      else
      {
        foreach (var error in updateResult.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
        return BadRequest(ModelState);
      }
    }

    return BadRequest(ModelState);
  }

  /// <summary>
  /// 創建用戶
  /// </summary>
  /// <param name="model">用戶創建模型</param>
  /// <returns>操作結果</returns>
  /// <response code="201">創建成功</response>
  /// <response code="400">請求無效</response>
  [HttpPost]
  [ProducesResponseType(201)]
  [ProducesResponseType(400)]
  public async Task<ActionResult> CreateUser([FromBody] CreateUserViewModel model)
  {
    if (ModelState.IsValid)
    {
      var user = new IdentityUser
      {
        UserName = model.UserName,
        Email = model.Email,
        PhoneNumber = model.PhoneNumber
      };

      var result = await _userManager.CreateAsync(user, model.Password);
      if (result.Succeeded)
      {
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
      }
      else
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
        return BadRequest(ModelState);
      }
    }

    return BadRequest(ModelState);
  }

  /// <summary>
  /// 刪除用戶
  /// </summary>
  /// <param name="id">用戶ID</param>
  /// <returns>操作結果</returns>
  /// <response code="204">刪除成功</response>
  /// <response code="404">用戶未找到</response>
  /// <response code="400">請求無效</response>
  [HttpDelete("{id}")]
  [ProducesResponseType(204)]
  [ProducesResponseType(404)]
  [ProducesResponseType(400)]
  public async Task<ActionResult> DeleteUser(string id)
  {
    var user = await _userManager.FindByIdAsync(id);
    if (user == null)
    {
      return NotFound();
    }

    var result = await _userManager.DeleteAsync(user);
    if (result.Succeeded)
    {
      return NoContent();
    }
    else
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError(string.Empty, error.Description);
      }
      return BadRequest(ModelState);
    }
  }
}
