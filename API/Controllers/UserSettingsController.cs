using System;
using System.Threading.Tasks;
using AsyncQueryProviderDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AsyncQueryProviderDemo.Models;

namespace AsyncQueryProviderDemo.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [ApiController]
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/usersettings")]
    public class UserSettingsController : Controller
    {
        private readonly IUserSettingService _userSettingService;

        public UserSettingsController(IUserSettingService userSettingService)
        {
            _userSettingService = userSettingService;
        }

        /// <summary>
        /// Adds a user's setting to database
        /// </summary>
        /// <param name="userSettingModel">userSettingModel</param>
        /// <returns>UserSettingModel</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUserSettingAsync([FromBody] UserSettingModel userSetting)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState.ToString());
                }
                userSetting.UserId = HttpContext.User.Identity.Name;
                return Ok(await _userSettingService.AddUserSettingAsync(userSetting).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// Deletes a user's setting to database
        /// </summary>
        /// <param name="userSettingId">userSettingId</param>
        /// <returns>UserSettingModel</returns>
        [HttpDelete("{userSettingId}")]
        [Authorize]
        public async Task<IActionResult> DeleteUserSettingAsync([FromRoute] int userSettingId)
        {
            try
            {
                if (userSettingId < 1)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Invalid user setting id");
                }

                await _userSettingService.DeleteUserSettingAsync(userSettingId).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Gets the UserSetting
        /// </summary>
        /// <param name="userSettingId">userSettingId</param>
        /// <returns>UserSettingModel</returns>
        [HttpGet("{userSettingId}")]
        public async Task<IActionResult> GetUserSettingsByUserIdAsync([FromQuery]string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid user id");
            }

            try
            {
                return Ok(await _userSettingService.GetUserSettingsByUserIdAsync(userId).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}