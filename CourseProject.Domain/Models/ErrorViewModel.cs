using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace CourseProject.Domain.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

}