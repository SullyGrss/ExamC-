using System;

namespace ExamenCSharp.Middleware
{
    public class ProblemDetails
    {
        public required string Type { get; set; } 
        public required string Title { get; set; } 
        public int Status { get; set; } 
        public required string Detail { get; set; } 
        public required string Instance { get; set; } 
    }
}
