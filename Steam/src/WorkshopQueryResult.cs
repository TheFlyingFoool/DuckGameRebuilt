using System;

public class WorkshopQueryResult {
    public WorkshopQueryResult() {
    }

    public WorkshopQueryResultDetails details;

    public string previewURL;

    public string metadata;

    public WorkshopItem[] fileList;

    public WorkshopQueryResultAdditionalPreview[] additionalPreviews;

    public uint[] statistics;
}
