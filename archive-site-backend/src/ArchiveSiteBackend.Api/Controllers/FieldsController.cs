using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ArchiveSite.Data;
using ArchiveSiteBackend.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArchiveSiteBackend.Api.Controllers {
    [ApiController]
    [Route("api/odata/[controller]")]
    public class FieldsController : EntityControllerBase<ArchiveDbContext, Field> {
        public FieldsController(ArchiveDbContext context) : base(context) {
        }

        [HttpPost("parse-rtp-xml")]
        [AllowAnonymous]
        public async Task<IActionResult> ParseRtpXml(
            [FromForm] IFormFile rtpXml,
            CancellationToken cancellationToken) {

            await using var rtpXmlStream = rtpXml.OpenReadStream();
            var rtpDocument = await XDocument.LoadAsync(rtpXmlStream, LoadOptions.SetLineInfo, cancellationToken);

            var columns = rtpDocument.Element("indexFile")?.Element("columns");
            if (columns == null) {
                return BadRequest(new ProblemDetails {
                    Type = "archive-site:validation-error/invalid-rtp-xml",
                    Title = "The specified file does not contain valid RTP XML.",
                    Detail =
                        rtpDocument.Element("indexFile") == null ?
                            "The required element <indexFile> was not found." :
                            "The required element <columns> was not found."
                });
            }

            try {
                var fieldList = new List<Field>();
                var warnings = new List<String>();
                String commentText = null;
                foreach (var child in columns.Nodes()) {
                    if (child is XComment comment) {
                        commentText = comment.Value;
                    } else if (child is XElement element && element.Name.LocalName == "column") {
                        fieldList.Add(RtpHelper.ParseField(commentText, element));
                    } else if (child is XElement unknownElement) {
                        var message =
                            $"Skipped unexpected element: {unknownElement.Name.LocalName}";
                        if (unknownElement is IXmlLineInfo lineInfo) {
                            message += $" ({lineInfo.LineNumber}:{lineInfo.LinePosition})";
                        }

                        warnings.Add(message);
                    } else {
                        var message =
                            $"Skipped unexpected node: {child.NodeType}";
                        if (child is IXmlLineInfo lineInfo) {
                            message += $" ({lineInfo.LineNumber}:{lineInfo.LinePosition})";
                        }

                        warnings.Add(message);
                    }
                }

                return Ok(new {
                    Fields = fieldList,
                    Warnings = warnings
                });
            } catch (XmlException ex) {
                return BadRequest(new ProblemDetails {
                    Type = "archive-site:validation-error/invalid-rtp-xml",
                    Title = "The specified file does not contain valid RTP XML.",
                    Detail = ex.Message
                });
            }
        }
    }
}
