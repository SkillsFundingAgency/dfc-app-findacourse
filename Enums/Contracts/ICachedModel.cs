using System;

namespace DFC.App.FindACourse.Data.Contracts
{
    public interface ICachedModel
    {
        string? Title { get; set; }

        Uri? Url { get; set; }
    }
}
