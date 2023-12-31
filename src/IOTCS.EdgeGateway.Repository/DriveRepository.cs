﻿using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Repository
{
    public class DriveRepository : IDriveRepository
    {
        private readonly IFreeSql _freeSql;

        public DriveRepository(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }
        public async Task<bool> Create(DriveModel driveModel)
        {
            var currRow = await _freeSql.Select<DriveModel>()
              .Where(d => d.DriveName.Equals(driveModel.DriveName))
              .ToListAsync().ConfigureAwait(false);
            if (currRow != null && currRow.Count > 0)
            {
                return false;
            }
            var affrows = _freeSql.Insert(driveModel).ExecuteAffrows();
            bool result = affrows > 0;
            return await Task.FromResult<bool>(result);
        }

        public async Task<IEnumerable<DriveModel>> GetAllDrive()
        {
            List<DriveModel> result = new List<DriveModel>();
            var affrows = await _freeSql.Select<DriveModel>().ToListAsync().ConfigureAwait(false);
            if (affrows != null)
                result.AddRange(affrows);
            return result;
        }

        public async Task<bool> Update(DriveModel driveModel)
        {
            var currRow = await _freeSql.Select<DriveModel>()
             .Where(d => d.DriveName.Equals(driveModel.DriveName))
             .ToListAsync().ConfigureAwait(false);
            if (currRow != null && currRow.Count > 1)
            {
                return false;
            }
            var result = false;

            var affrows = _freeSql.Update<DriveModel>().SetSource(driveModel).ExecuteAffrows();

            result = affrows > 0 ? true : false;

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> Delete(DriveModel driveModel)
        {

            var result = false;
            var drives = await _freeSql.Select<DeviceModel>().Where(d => d.DriveId == driveModel.Id).ToListAsync().ConfigureAwait(false);
            if (drives != null && drives.Count > 0)
            {
                return false;
            }
            var affrows = _freeSql.Delete<DriveModel>().Where(d => d.Id == driveModel.Id).ExecuteAffrows();

            result = affrows > 0 ? true : false;

            return await Task.FromResult<bool>(result);
        }

        public async Task<DriveModel> GetDriveByDeviceId(string deviceId)
        {
            var affrows = await _freeSql.Select<DeviceModel, DriveModel>()
                .LeftJoin((a, b) => a.DriveId == b.Id).Where(d => d.t1.Id == deviceId).ToListAsync((a, b) => b).ConfigureAwait(false);

            if (affrows != null && affrows.Count > 0)
            {
                return affrows[0];
            }
            return null;
        }

        public async Task<DriveModel> GetDriveByGroupId(string groupId)
        {
            var group = await _freeSql.Select<DeviceModel>().Where(d => d.Id == groupId).ToListAsync().ConfigureAwait(false);

            if (group != null && group.Count > 0)
            {
                var deviceId = group[0].ParentId;
                var affrows = await _freeSql.Select<DeviceModel, DriveModel>()
                     .LeftJoin((a, b) => a.DriveId == b.Id).Where(d => d.t1.Id == deviceId).ToListAsync((a, b) => b).ConfigureAwait(false);

                if (affrows != null && affrows.Count > 0)
                {
                    return affrows[0];
                }
                return null;
            }
            return null;
        }
    }
}
