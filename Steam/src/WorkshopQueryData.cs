using System;

[Flags]
public enum WorkshopQueryData {
    Statistics = 32,
    LongDescription = 16,
    AdditionalPreviews = 8,
    Children = 4,
    Metadata = 2,
    PreviewURL = 1,
    Details = 0,
    TotalOnly = -1
}
