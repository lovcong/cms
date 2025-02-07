﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadAudioController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsUploadAudio))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.AudioUploadDirectoryName = request.AudioUploadDirectoryName;
            site.AudioUploadDateFormatString = request.AudioUploadDateFormatString;
            site.IsAudioUploadChangeFileName = request.IsAudioUploadChangeFileName;
            site.AudioUploadExtensions = request.AudioUploadExtensions.Replace("|", ",");
            site.AudioUploadTypeMaxSize = request.AudioUploadTypeMaxSize;

            if (_settingsManager.IsSafeMode)
            {
                site.AudioUploadExtensions = Constants.DefaultAudioUploadExtensions;
            }

            await _siteRepository.UpdateAsync(site);

            await _authManager.AddSiteLogAsync(request.SiteId, "修改音频上传设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}