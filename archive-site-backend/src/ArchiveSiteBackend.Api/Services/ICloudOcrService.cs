using ArchiveSiteBackend.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveSiteBackend.Api.Services
{
    public interface ICloudOcrService
    {
        public Task<List<DocumentText>> ReadImage(FileStream fileStream);
    }
}
